using Assets;
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
  private List<Encounter> encounters = new List<Encounter>();
  private int encounterCounter = 0;

  private int GetId(string name)
  {
    return (name.Aggregate(0, (current, c) => (current * 31) + c));
  }

  public void Init()
  {
    LoadEncounters();
    PrepareModels();
  }

  private void LoadEncounters()
  {
    XmlDocument doc = new XmlDocument();
    doc.Load(@GameEngine.PathToTrials);

    XmlNode level = doc.DocumentElement.ChildNodes[GameEngine.LevelId];

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

  private Dictionary<int, Transform> objectDictionary = new Dictionary<int, Transform>();
  private void PrepareModels()
  {
    foreach (GameObject obj in GameEngine.PropertiesAndObjects)
    {
      Transform spawnedPrefab = Instantiate(obj, transform).transform;
      spawnedPrefab.gameObject.SetActive(false);
      objectDictionary.Add(GetId(obj.name), spawnedPrefab);
    }
  }


  /// <summary>
  /// Returns activated object and starts encounter
  /// </summary>
  /// <returns></returns>
  public Transform StartEncounter()
  {
    int curEncounter = encounters[encounterCounter].GetEnemyId();
    Transform obj = objectDictionary[curEncounter];
    obj.gameObject.SetActive(true);
    return obj;
  }

  public int GetCurrentEncounterId()
  {
    return encounters[encounterCounter].GetEnemyId();
  }

  public string GetCurrentPropertyInfo()
  {
    return encounters[encounterCounter].CurrentPropertyInfo();
  }

  /// <summary>
  /// Spawns property
  /// </summary>
  /// <returns></returns>
  public Transform SpawnProperty()
  {
    int propid = encounters[encounterCounter].GetCurrentPropertyId();
    Transform property = objectDictionary[propid];
    property.gameObject.SetActive(true);
    return property;
  }

  /// <summary>
  /// Returns true if input was correct and destroys property
  /// </summary>
  /// <param name="used"></param>
  /// <returns></returns>
  public bool EvaluateProperty(bool used)
  {
    StartCoroutine(DeActivateProperty(used ? GameEngine.InputState.Using : GameEngine.InputState.Discarding)); // Fix
    return encounters[encounterCounter].EvaluateInput(used);
  }

  public void SkipProperty()
  {
    encounters[encounterCounter].SkipProperty();
    StartCoroutine(DeActivateProperty(GameEngine.InputState.None));
  }

  private IEnumerator DeActivateProperty(GameEngine.InputState input)
  {
    int propid = encounters[encounterCounter].GetCurrentPropertyId();
    Transform property = objectDictionary[propid];

    float duration = 0.5f; // TODO DEPEND ON OTHER TIMINGS
    float movement = 0.02f;

    float startTime = Time.realtimeSinceStartup;
    while (Time.realtimeSinceStartup < startTime + duration)
    {
      float x = 1 - (Time.realtimeSinceStartup - startTime) / duration;
      property.localScale = new Vector3(x, x, x);
      if (input != GameEngine.InputState.None)
        property.position += new Vector3(input == GameEngine.InputState.Using ? movement : -movement, 0, 0);
      yield return null;
    }

    property.gameObject.SetActive(false);
  }

  public void DamageEncounter()
  {
    encounters[encounterCounter].DealDamage();
  }

  public bool EncounterOver => encounters[encounterCounter].EncounterOver;

  /// <summary>
  /// Kill encounter and return true if encounter was won
  /// </summary>
  /// <returns></returns>
  public bool KillEncounter()
  {
    bool won = encounters[encounterCounter].EndEncounter();
    int curEncounter = encounters[encounterCounter].GetEnemyId();
    Transform obj = objectDictionary[curEncounter];
    obj.gameObject.SetActive(false);
    encounterCounter++;

    return won;
  }

  public bool LevelOver => encounterCounter >= encounters.Count;

  //public Encounter GetEncounter()
  //{
  //  return encounters[encounterCounter];
  //}

  //public Transform ActivateObject(int id)
  //{
  //  objectDictionary[id].gameObject.SetActive(true);
  //  return objectDictionary[id];
  //}

  //public void CompleteEncounter(int id)
  //{
  //  objectDictionary[id].gameObject.SetActive(false);
  //  encounterCounter++;
  //}
}
