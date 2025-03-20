# MediaPipe Body
import mediapipe as mp
from mediapipe.tasks import python
from mediapipe.tasks.python import vision
from clientUDP import ClientUDP

import cv2
import threading
import time
import global_vars 
import struct

# The capture thread captures images from the WebCam on a separate thread (for performance)
class CaptureThread(threading.Thread):
    cap = None
    ret = None
    frame = None
    isRunning = False
    counter = 0
    timer = 0.0

    def run(self):
        self.cap = cv2.VideoCapture(global_vars.CAM_INDEX)  # Sometimes it can take a while for certain video captures
        if global_vars.USE_CUSTOM_CAM_SETTINGS:
            self.cap.set(cv2.CAP_PROP_FPS, global_vars.FPS)
            self.cap.set(cv2.CAP_PROP_FRAME_WIDTH, global_vars.WIDTH)
            self.cap.set(cv2.CAP_PROP_FRAME_HEIGHT, global_vars.HEIGHT)

        time.sleep(1)

        #print("Opened Capture @ %s fps" % str(self.cap.get(cv2.CAP_PROP_FPS)))
        while not global_vars.KILL_THREADS:
            self.ret, self.frame = self.cap.read()
            self.isRunning = True
            if global_vars.DEBUG:
                self.counter = self.counter + 1
                if time.time() - self.timer >= 3:
                    #print("Capture FPS: ", self.counter / (time.time() - self.timer))
                    self.counter = 0
                    self.timer = time.time()


# The body thread actually does the processing of the captured images and communication with Unity
class BodyThread(threading.Thread):
    data = ""
    dirty = True
    pipe = None
    timeSinceCheckedConnection = 0
    timeSincePostStatistics = 0
    user_in_frame = False  # Track whether the user is in the frame

    def run(self):
        mp_drawing = mp.solutions.drawing_utils
        mp_pose = mp.solutions.pose

        self.setup_comms()

        capture = CaptureThread()
        capture.start()

        with mp_pose.Pose(
            min_detection_confidence=0.80,
            min_tracking_confidence=0.5,
            model_complexity=global_vars.MODEL_COMPLEXITY,
            static_image_mode=False,
            enable_segmentation=True
        ) as pose:

            while not global_vars.KILL_THREADS and capture.isRunning == False:
                print("Waiting for camera and capture thread.")
                time.sleep(0.5)
            print("Beginning capture")

            while not global_vars.KILL_THREADS and capture.cap.isOpened():
                ti = time.time()

                # Fetch stuff from the capture thread
                ret = capture.ret
                image = capture.frame

                # Image transformations and stuff
                image = cv2.flip(image, 1)
                image.flags.writeable = global_vars.DEBUG

                # Detections
                results = pose.process(image)
                tf = time.time()

                # Rendering results
                if global_vars.DEBUG:
                    if time.time() - self.timeSincePostStatistics >= 1:
                        #print("Theoretical Maximum FPS: %f" % (1 / (tf - ti)))
                        self.timeSincePostStatistics = time.time()

                    if results.pose_landmarks:
                        mp_drawing.draw_landmarks(
                            image,
                            results.pose_landmarks,
                            mp_pose.POSE_CONNECTIONS,
                            mp_drawing.DrawingSpec(color=(255, 100, 0), thickness=2, circle_radius=4),
                            mp_drawing.DrawingSpec(color=(255, 255, 255), thickness=2, circle_radius=2),
                        )
                    cv2.imshow('Body Tracking', image)
                    cv2.waitKey(3)

                # Check if the user is in the frame
                if results.pose_landmarks:
                    if not self.user_in_frame:
                        print("User has re-entered the frame.")
                        self.user_in_frame = True

                    # Get the left and right hip landmarks
                    left_hip = results.pose_landmarks.landmark[mp_pose.PoseLandmark.LEFT_HIP]
                    right_hip = results.pose_landmarks.landmark[mp_pose.PoseLandmark.RIGHT_HIP]

                    # Calculate the center position (midpoint between left and right hips)
                    center_x = (left_hip.x + right_hip.x) / 2
                    center_y = (left_hip.y + right_hip.y) / 2

                    # Print the center position (optional, for debugging)
                    #if global_vars.DEBUG:
                        #print(f"User Position in Frame: X = {center_x:.2f}, Y = {center_y:.2f}")
                else:
                    if self.user_in_frame:
                        print("User has left the frame.")
                        self.user_in_frame = False

                # Set up data for relay
                self.data = ""
                i = 0
                if results.pose_world_landmarks:
                    hand_world_landmarks = results.pose_world_landmarks
                    for i in range(0, 33):
                        self.data += "{}|{}|{}|{}\n".format(
                            i, hand_world_landmarks.landmark[i].x, hand_world_landmarks.landmark[i].y, hand_world_landmarks.landmark[i].z
                        )
                if results.pose_landmarks:
                    self.data += f"user_position|{center_x}|{center_y}\n"
                    
                self.data += f"user_in_frame|{int(self.user_in_frame)}\n"

                self.send_data(self.data)

        self.pipe.close()
        capture.cap.release()
        cv2.destroyAllWindows()
        pass

    def setup_comms(self):
        if not global_vars.USE_LEGACY_PIPES:
            self.client = ClientUDP(global_vars.HOST, global_vars.PORT)
            self.client.start()
        else:
            print("Using Pipes for interprocess communication (not supported on OSX or Linux).")
        pass

    def send_data(self, message):
        if not global_vars.USE_LEGACY_PIPES:
            self.client.sendMessage(message)
            pass
        else:
            # Maintain pipe connection.
            if self.pipe == None and time.time() - self.timeSinceCheckedConnection >= 1:
                try:
                    self.pipe = open(r'\\.\pipe\UnityMediaPipeBody1', 'r+b', 0)
                except FileNotFoundError:
                    print("Waiting for Unity project to run...")
                    self.pipe = None
                self.timeSinceCheckedConnection = time.time()

            if self.pipe != None:
                try:
                    s = self.data.encode('utf-8')
                    self.pipe.write(struct.pack('I', len(s)) + s)
                    self.pipe.seek(0)
                except Exception as ex:
                    print("Failed to write to pipe. Is the unity project open?")
                    self.pipe = None
        pass