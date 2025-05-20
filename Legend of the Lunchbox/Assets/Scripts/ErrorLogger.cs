using System;
using System.IO;
using Assets;
using UnityEngine;
using UnityEngine.UI;

public class ErrorLogger : MonoBehaviour
{
    private string logFilePath;

    void Start()
    {
        Application.logMessageReceived += HandleLog;

        // Define log file path
        logFilePath = Path.Combine(Application.streamingAssetsPath, "error_log_" + DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss") + ".txt");
        File.AppendAllText(logFilePath, $"----LotL Log---- framerate margin = {GameEngine.StaticTimeVariables.FrameRateMargin.ToString()}\n\n");
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (type == LogType.Error || type == LogType.Exception)
        {
            string logEntry = $"-----------\n{logString}\n{stackTrace}\n-----------\n";

            // Append to log file
            File.AppendAllText(logFilePath, logEntry);
        }
    }
    
    private void Update()
    {
        float diff = Time.deltaTime - .016f;
        
        if (diff > GameEngine.StaticTimeVariables.FrameRateMargin)
        {
            File.AppendAllText(logFilePath, 
                $"{Time.realtimeSinceStartupAsDouble.ToString()} | Frames per second irregularity, frame difference {diff.ToString()} seconds more than 60fps\n");
        }
    }
}