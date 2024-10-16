using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class UIConsole : MonoBehaviour
{
  [SerializeField] private int lineLimit = 20;
  private string[] lines;
  private TextMeshProUGUI ui;

  private void Awake()
  {
    ui = GetComponent<TextMeshProUGUI>();
    lines = new string[lineLimit];
    for(int i = 0; i < lines.Length; i++)
      lines[i] = "";
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
}
