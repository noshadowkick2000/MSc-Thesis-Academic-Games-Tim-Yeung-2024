using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoggerComponent : MonoBehaviour
{
  [SerializeField] private int lineLimit = 20;
  [SerializeField] private TextMeshProUGUI ui;
  private string[] lines;
  
  private void Awake()
  {
    lines = new string[lineLimit];
    for(int i = 0; i < lines.Length; i++)
      lines[i] = "";
    
    Logger.Awake(this);
  }

  public void PrintToUI(string line)
  {
    UpdateLines(line);
    ui.text = string.Join(Environment.NewLine, lines);
  }

  private void UpdateLines(string line)
  {
    for (int i = lines.Length-1; i > 0; i--)
    {
      lines[i] = lines[i-1];
    }
    lines[0] = line;
  }

  private void OnDestroy()
  {
    Logger.OnDestroy();
  }
}
