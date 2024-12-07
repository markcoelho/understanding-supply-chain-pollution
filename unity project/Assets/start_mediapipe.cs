using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System.IO;

public class start_mediapipe : MonoBehaviour
{
    void Start()
    {
        // Path to the batch file
        string batchFilePath = Path.Combine(Application.streamingAssetsPath, "mediapipeavatar", "START PY.bat");

        // Check if the batch file exists
        if (File.Exists(batchFilePath))
        {
            try
            {
                // Start the batch file
                ProcessStartInfo processInfo = new ProcessStartInfo("cmd.exe", "/c \"" + batchFilePath + "\"");
                processInfo.CreateNoWindow = true; // Hide the command prompt window
                processInfo.UseShellExecute = false;
                processInfo.RedirectStandardOutput = true;
                processInfo.RedirectStandardError = true;
                processInfo.WorkingDirectory = Path.Combine(Application.streamingAssetsPath, "mediapipeavatar"); // Set working directory

                using (Process process = Process.Start(processInfo))
                {
                    // Log the output and errors for debugging
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();
                    process.WaitForExit();

                    UnityEngine.Debug.Log("Batch file output: " + output);
                    UnityEngine.Debug.LogError("Batch file error: " + error);
                }
            }
            catch (System.Exception ex)
            {
                UnityEngine.Debug.LogError("Exception occurred while starting batch file: " + ex.Message);
            }
        }
        else
        {
            UnityEngine.Debug.LogError("Batch file not found: " + batchFilePath);
        }
    }

    void Update()
    {
        // Update logic here (if needed)
    }
}
