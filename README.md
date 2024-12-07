# Understanding Supply Chain Pollution

## Overview
This project is part of a Master's degree in Design and Multimedia for the Computational Design Lab at the University of Coimbra. It explores the environmental footprint of consumption habits through an interactive experience.

### Key Features
- **Environmental Footprint Quiz:** Estimates the trash generated over a lifetime based on user input.
- **Digital Reflection:** Generates 3D objects in a Unity scene that form a dynamic digital reflection tracking body movements.
- **Body Tracking:** Utilizes Python and MediaPipe with a standard webcam.

### Technology Stack
- **Unity + Python:** Implements the Google MediaPipe integration for body tracking. This implementation is based on an open-source project by [Ganesh Sar](https://github.com/ganeshsar/UnityPythonMediaPipeAvatar).

### Demo Video
[![Example Video](https://img.youtube.com/vi/L1JSLO5pwMg/0.jpg)](https://www.youtube.com/watch?v=L1JSLO5pwMg)

---

## How to Run

### Prerequisites

#### Body Tracking
1. Install Python from [python.org](https://www.python.org/).
2. Install the MediaPipe library: `pip install mediapipe`.

#### Quiz
1. Download and install Processing 4.3 from [processing.org](https://processing.org/download).

#### Unity
1. Install Unity Hub.
2. Use Unity version `2021.3.24f1`.

---

### Step-by-Step Instructions

#### 1. **Body Tracking**
- Navigate to the `body tracking` folder.
- Run the script:
  ```
  py main.py
  ```
  This will open a window displaying the webcam capture with body tracking.

#### 2. **Quiz**
- Open `quiz.pde` in Processing.
- Adjust the `answer.txt` file path in lines 60 and 178 of `quiz.pde`:
  ```java
  // For Unity project development:
  File file = new File(sketchPath("unity project/Assets/StreamingAssets/quiz/answer.txt"));

  // For the built Unity project:
  File file = new File(sketchPath("digital mirror_Data/StreamingAssets/quiz/answer.txt"));
  ```
- Save the `answer.txt` file in the `StreamingAssets/quiz` folder.

##### Running the Quiz in Fullscreen
To enable fullscreen mode, edit lines 144 and 145 of `quiz.pde`:
```java
// Windowed mode:
size(1000, 500);

// Fullscreen mode:
fullScreen(); // For the main monitor
fullScreen(2); // For the second monitor
```

#### 3. **Unity Project**
- Import the `unity project` folder into Unity Hub.
- Build the Unity project directly into the `understanding supply chain pollution` folder.
- Ensure the `answer.txt` file location in `quiz.pde` is correctly configured as described above.

---

### Running the Complete System
1. Start the body tracking script:
   ```
   py main.py
   ```
2. Run the quiz sketch in Processing.
3. Launch the Unity program.

---

## Acknowledgments
- This project integrates the [UnityPythonMediaPipeAvatar](https://github.com/ganeshsar/UnityPythonMediaPipeAvatar) for body tracking.

Enjoy exploring your environmental footprint and the digital reflection of your consumption habits!
