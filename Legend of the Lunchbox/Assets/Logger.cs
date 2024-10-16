using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class Logger
{
  private static StreamWriter output;
  private static string folderName;

  private static UIConsole uiConsole;
  private static bool uiUsed = false;

  // TODO IMPLEMENT FEATURE IN GAME SETUP TO SELECT FOLDER FOR THE RESULTS

  public static void Awake(string name, UIConsole? ui)
  {
    folderName = name;
    string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\" + folderName;
    string filename = DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss");
    output = new StreamWriter($"{path}\\{filename}.log");

    uiUsed = ui != null;
    if (uiUsed)
      uiConsole = ui;
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
    output.WriteLine(line);
    Console.WriteLine(line);

    if (uiUsed)
      uiConsole.PrintToUI(line);
  }

  

  public static void OnDestroy()
  {
    output.Close();
  }
}
