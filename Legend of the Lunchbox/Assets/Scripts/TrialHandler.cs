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
using Random = UnityEngine.Random;

public class TrialHandler : MonoBehaviour
{
  private readonly List<Encounter> encounters = new List<Encounter>();
  private int encounterCounter = 0;
  
  public delegate void SpawnEvent(Transform property);

  private int GetId(string name)
  {
    return (name.Aggregate(0, (current, c) => (current * 31) + c));
  }

  private void Awake()
  {
    LoadEncounters();

    SubscribeToEvents();
  }

  private void OnDestroy()
  {
    UnSubscribeToEvents();
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
        if (encounter.Name == "encounter")
          CreateEncounter(objectRoot, encounter);
        else if (encounter.Name == "break")
          CreateBreak(objectRoot, encounter);
      }

      Logger.Log($"Finished loading successfully, {encounters.Count} encounters loaded");
    }
    catch (Exception ex)
    {
      Logger.Log(ex.ToString());
      Logger.Log("Failed to load all encounters");
    }
  }

  private void CreateEncounter(Transform objectRoot, XmlNode encounter)
  {
    Encounter enc = objectRoot.AddComponent<Encounter>();
    int health = int.Parse(encounter.Attributes["health"].Value);
    int enemyId = GetId(encounter.Attributes["enemy"].Value);
    int waitTimeMillis = int.Parse(encounter.Attributes["wait"].Value);
    List<int> propertyIds = new List<int>();
    List<bool> correctProperty = new List<bool>();
    List<PropertyType> propertyTypes = new List<PropertyType>();
    foreach (XmlNode node in encounter.ChildNodes)
    {
      //print(node.InnerText);
      propertyIds.Add(GetId(node.InnerText));
      correctProperty.Add(Convert.ToBoolean(node.Attributes["correct"].Value));
      propertyTypes.Add((PropertyType)int.Parse(node.Attributes["type"].Value));
    }
    enc.Init(enemyId, health, propertyIds.ToArray(), correctProperty.ToArray(), propertyTypes.ToArray(), waitTimeMillis/1000f);
    Logger.Log($"Loaded encounter: {Environment.NewLine}{enc.ToString()}");
    encounters.Add(enc);
  }
  
  private void CreateBreak(Transform objectRoot, XmlNode encounter)
  {
    Encounter enc = objectRoot.AddComponent<Encounter>();
    int health = int.Parse(encounter.Attributes["health"].Value);
    int waitTimeMillis = int.Parse(encounter.Attributes["wait"].Value);
    enc.Init(health, waitTimeMillis/1000f);
    Logger.Log($"Loaded break");
    encounters.Add(enc);
  }

  private GameObject GetModel(int id)
  {
    foreach (var propobj in GameEngine.PropertiesAndObjects)
    {
      if (GetId(propobj.name) == id)
        return propobj;
    }

    return null;
  }

  private Dictionary<int, Transform> objectDictionary = new Dictionary<int, Transform>();
  private void PrepareModels()
  {
    void SpawnAddToDictionary(GameObject obj, int id)
    {
      Transform spawnedPrefab = Instantiate(obj, transform).transform;
      spawnedPrefab.gameObject.SetActive(false);
      objectDictionary.Add(id, spawnedPrefab);
    }
    
    // Clear previous
    if (objectDictionary.Count > 0)
    {
      foreach (var pair in objectDictionary)
      {
        Destroy(pair.Value.gameObject);
      }
      objectDictionary = new Dictionary<int, Transform>();
    }
    
    // Add object gameobject
    SpawnAddToDictionary(GetModel(GetCurrentEncounterId()), GetCurrentEncounterId());

    // Add property gameobjects
    foreach (var propertyId in encounters[encounterCounter].GetAllPropertyIds())
    {
      SpawnAddToDictionary(GetModel(propertyId), propertyId);
    }
  }
  
  public static event SpawnEvent OnObjectSpawnedEvent;
  
  private void StartEncounter()
  {
    Transform obj = objectDictionary[GetCurrentEncounterId()];
    obj.gameObject.SetActive(true);
    obj.position = LocationHolder.PropertyLocation.position;
    OnObjectSpawnedEvent?.Invoke(obj);
  }

  public float GetCurrentWaitTime()
  {
    return encounters[encounterCounter].GetWaitTime();
  }

  public float GetTotalWaitTime()
  {
    float wt = 0;
    foreach (var encounter in encounters)
    {
      wt += encounter.GetWaitTime();
    }

    return wt;
  }

  public enum PropertyType
  {
    ACTION,
    SOUND,
    WORD
  }

  public PropertyType GetCurrentEncounterType()
  {
    return encounters[encounterCounter].GetCurrentPropertyType();
  }

  public bool EncounterIsObject()
  {
    return encounters[encounterCounter].IsObject;
  }

  private int GetCurrentEncounterId()
  {
    return encounters[encounterCounter].GetEnemyId();
  }

  private string GetCurrentPropertyInfo()
  {
    return encounters[encounterCounter].CurrentPropertyInfo();
  }
  
  public static event SpawnEvent OnPropertySpawnedEvent;
  
  private void SpawnProperty()
  {
    int propid = encounters[encounterCounter].GetCurrentPropertyId();
    Transform property = objectDictionary[propid];
    
    OnPropertySpawnedEvent?.Invoke(property);
  }

  /// <summary>
  /// Returns true if input was correct and destroys property
  /// </summary>
  /// <param name="used"></param>
  /// <param name="input"></param>
  /// <returns></returns>
  public bool EvaluateProperty(InputHandler.InputState input)
  {
    // StartCoroutine(DeSpawnProperty());
    return encounters[encounterCounter].EvaluateInput(input == InputHandler.InputState.USING);
  }

  private void SkipProperty()
  {
    // StartCoroutine(DeSpawnProperty());
    encounters[encounterCounter].SkipProperty();
  }

  public void DamageEncounter()
  {
    encounters[encounterCounter].DealDamage();
  }

  public bool EncounterOver => encounters[encounterCounter].EncounterOver;

  /// <summary>
  /// Return true if encounter was won
  /// </summary>
  /// <returns></returns>
  public bool WonEncounter()
  {
    return encounters[encounterCounter].EndEncounter();
  }
  
  private void KillEncounter()
  {
    Transform obj = objectDictionary[GetCurrentEncounterId()];
    obj.gameObject.SetActive(false);
    encounterCounter++;
  }

  public bool LevelOver => encounterCounter >= encounters.Count;

  //-------------------------------------------------------

  private void SubscribeToEvents()
  {
    GameEngine.StartingBreakStartedEvent += StartingBreak;
    GameEngine.StartingEncounterStartedEvent += StartingEncounter;
    GameEngine.ShowingPropertyStartedEvent += ShowingProperty;
    GameEngine.TimedOutStartedEvent += TimedOut;
    GameEngine.AnswerCorrectStartedEvent += AnswerCorrect;
    GameEngine.EndingEncounterStartedEvent += EndingEncounter;
  }
  
  private void UnSubscribeToEvents()
  {
    GameEngine.StartingBreakStartedEvent -= StartingBreak;
    GameEngine.StartingEncounterStartedEvent -= StartingEncounter;
    GameEngine.ShowingPropertyStartedEvent -= ShowingProperty;
    GameEngine.TimedOutStartedEvent -= TimedOut;
    GameEngine.AnswerCorrectStartedEvent -= AnswerCorrect;
    GameEngine.EndingEncounterStartedEvent -= EndingEncounter;
  }

  protected virtual void StartingBreak()
  {
    encounterCounter++;
  }

  protected virtual void StartingEncounter()
  {
    PrepareModels();
    StartEncounter();
  }

  protected virtual void ShowingProperty(Action<InputHandler.InputState> callback)
  {
    SpawnProperty();
  }

  protected virtual void TimedOut(InputHandler.InputState input)
  {
    if (input == InputHandler.InputState.NONE) 
      SkipProperty();
  }

  protected virtual void AnswerCorrect()
  {
    DamageEncounter();
  }

  protected virtual void EndingEncounter()
  {
    KillEncounter();
  }
}
