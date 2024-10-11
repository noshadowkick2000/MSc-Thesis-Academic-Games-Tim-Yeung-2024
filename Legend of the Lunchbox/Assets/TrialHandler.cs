using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Windows;

public class TrialHandler : MonoBehaviour
{
  [SerializeField] private string pathToTrials;
  [SerializeField] private int levelId = 0;
  private List<Encounter> encounters = new List<Encounter>();

  private int GetId(string name)
  {
    return (name.Aggregate(0, (current, c) => (current * 31) + c));
  }

  private void Awake()
  {
    XmlDocument doc = new XmlDocument();
    doc.Load(@pathToTrials);

    XmlNode level = doc.DocumentElement.ChildNodes[levelId];

    Transform objectRoot = new GameObject("EncounterHolder").transform;
    objectRoot.parent = transform;
    
    try
    {
      foreach (XmlNode encounter in level.ChildNodes)
      {
        Encounter enc = objectRoot.AddComponent<Encounter>();
        int health = int.Parse(encounter.Attributes["health"].Value);
        int enemyId = GetId(encounter.Attributes["enemy"].Value);
        List<int> propertyIds = new List<int>();
        List<bool> correctProperty = new List<bool>();
        foreach (XmlNode node in encounter.ChildNodes)
        {
          //print(node.InnerText);
          propertyIds.Add(GetId(node.InnerText));
          correctProperty.Add(Convert.ToBoolean(node.Attributes["correct"].Value));
        }
        enc.Init(enemyId, health, propertyIds.ToArray(), correctProperty.ToArray());
        Logger.Log($"Loaded encounter: {Environment.NewLine}{enc.ToString()}");
        encounters.Add(enc);
      }
      Logger.Log($"Finished loading successfully, {encounters.Count} encounters loaded");
    }
    catch (Exception ex) 
    { 
      Logger.Log(ex.ToString());
      Logger.Log("Failed to load all encounters");
    }
    
    
  }
}
