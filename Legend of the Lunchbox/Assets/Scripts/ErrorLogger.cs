using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ErrorLogger : MonoBehaviour
{
    private string logFilePath;

    void Awake()
    {
        Application.logMessageReceived += HandleLog;

        // Define log file path
        logFilePath = Path.Combine(Application.streamingAssetsPath, "error_log_" + DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss") + ".txt");

        // Clear previous logs (optional)
        File.WriteAllText(logFilePath, "=== LotL ERROR LOG ===\n");
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (type == LogType.Error || type == LogType.Exception)
        {
            string logEntry = $"{logString}\n{stackTrace}\n\n";

            // Append to log file
            File.AppendAllText(logFilePath, logEntry);
        }
    }
}