using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class Logger
{
  private static StreamWriter output;
  private static string folderName;

  // TODO IMPLEMENT FEATURE IN GAME SETUP TO SELECT FOLDER FOR THE RESULTS

  public static void Awake(string name)
  {
    folderName = name;
    string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\" + folderName;
    string filename = DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss");
    output = new StreamWriter($"{path}\\{filename}.log");
  }

  public static void Log(string data)
  {
    string time = Time.realtimeSinceStartupAsDouble.ToString().PadRight(24);
    string[] lines = data.Split(Environment.NewLine);
    string line = $"{time}: {lines[0]}";
    string padding = new string(' ', 26);
    for (int i = 1; i < lines.Length; i++)
    {
      line += $"{Environment.NewLine}{padding}{lines[i]}";
    }
    output.WriteLine(line);
    Console.WriteLine(line);
  }

  public static void OnDestroy()
  {
    output.Close();
  }
}
