using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Assets;
using UnityEngine;
using CsvHelper;
using CsvHelper.Configuration;

public class Logger : MonoBehaviour
{
  private class LogEntry
  {
    public string DateTime { get; set; }
    public string Object { get; set; }
    public int InternalTime { get; set; }
    public string EventType { get; set; }
    public string Code { get; set; }
  }

  public enum Event
  {
    STIMULUS,
    PROMPT,
    FIXATE,
    INPUT,
    OTHER
  }

  public enum CodeTypes
  {
    OBJECT_IMAGE,
    OBJECT_WORD,
    OBJECT_STOP,
    PROPERTY_ACTION,
    PROPERTY_SOUND,
    PROPERTY_WORD,
    PROPERTY_STOP,
    INPUT_POSITIVE,
    INPUT_NEGATIVE,
    INPUT_TIMEOUT,
    FEEDBACK_POSITIVE,
    FEEDBACK_NEGATIVE,
  }
  
  private static StreamWriter _output;
  private static string _folderName;

  // private static LoggerComponent _loggerComponent;

  // TODO IMPLEMENT FEATURE IN GAME SETUP TO SELECT FOLDER FOR THE RESULTS

  public void Awake()
  {
    string path = Application.streamingAssetsPath + "/" + DateTime.Today.ToString("ddMMyyyy");
    Directory.CreateDirectory(path);
    string filename = DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss");
    _output = new StreamWriter($"{path}\\{filename}.log");
  }

  private static string ConvertSecondsToReadableTime(double seconds)
  {
    // Extract the whole number part (for hours, minutes, seconds)
    TimeSpan time = TimeSpan.FromSeconds(seconds);

    // Extract the microseconds from the fractional part of the seconds
    int milliseconds = (int)((seconds - Math.Floor(seconds)) * 1000000);

    // Return the formatted string (hours:minutes:seconds.microseconds)
    return $"{(int)time.TotalHours:D2}:{time.Minutes:D2}:{time.Seconds:D2}.{milliseconds:D3}";
  }

  public static void Log(string data)
  {
    string time = ConvertSecondsToReadableTime(Time.realtimeSinceStartupAsDouble).PadRight(24);
    string[] lines = data.Split(Environment.NewLine);
    string line = $"{time}: {lines[0]}";
    string padding = new string(' ', 26);
    for (int i = 1; i < lines.Length; i++)
    {
      line += $"{Environment.NewLine}{padding}{lines[i]}";
    }
    _output.WriteLine(line);
    Console.WriteLine(line);
  }

  public void OnDestroy()
  {
    _output.Close();
  }
}
