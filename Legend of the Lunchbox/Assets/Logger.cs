using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Logger : MonoBehaviour
{
  private StreamWriter output;
  [SerializeField] private string folderName;

  // TODO IMPLEMENT FEATURE IN GAME SETUP TO SELECT FOLDER FOR THE RESULTS

  private void Awake()
  {
    string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\" + folderName;
    string filename = DateTime.Now.ToString("dd-MM-yyyy_HH-mm-ss");
    output = new StreamWriter($"{path}\\{filename}.log");
  }

  public void Log(string data)
  {
    string line = $"{Time.realtimeSinceStartupAsDouble}s: {data}";
    output.WriteLine(line);
    print(line);
  }

  private void OnDestroy()
  {
    output.Close();
  }
}
