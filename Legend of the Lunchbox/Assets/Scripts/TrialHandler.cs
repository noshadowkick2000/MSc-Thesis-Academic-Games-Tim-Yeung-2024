using Assets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using CsvHelper;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Windows;
using Random = UnityEngine.Random;

public class TrialHandler : MonoBehaviour
{
  [SerializeField] private string trialsFileName;
  
  private readonly List<EncounterData> encounters = new List<EncounterData>();
  private int encounterCounter = 0;
  
  public delegate void SpawnEvent(Transform property);

  private int GetId(string objectName)
  {
    return (objectName.Aggregate(0, (current, c) => (current * 31) + c));
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
  
  private class EncounterEntry
  {
    public float BlockDelay { get; set; }
    public string ObjectType { get; set; }
    public string StimulusObject { get; set; }
    public string PropertyType { get; set; }
    public string StimulusProperty { get; set; }
    public bool ValidProperty { get; set; }
    public float PropertyDelay { get; set; }
    public float ITI { get; set; }
  }
  
  private void LoadEncounters()
  {
    EncounterData.ObjectType ConvertStringObjectType(string objectType)
    {
      switch (objectType)
      {
        case "BREAK":
          return EncounterData.ObjectType.BREAK;
        case "IMAGE":
          return EncounterData.ObjectType.IMAGE;
        case "WORD":
          return EncounterData.ObjectType.WORD;
        default:
          return EncounterData.ObjectType.BREAK;
      }
    }

    EncounterData.PropertyType ConvertStringPropertyType(string propertyType)
    {
      switch (propertyType)
      {
        case "BREAK":
          return EncounterData.PropertyType.ACTION;
        case "IMAGE":
          return EncounterData.PropertyType.SOUND;
        case "WORD":
          return EncounterData.PropertyType.WORD;
        default: 
          return EncounterData.PropertyType.WORD;
      }
    }
    
    using (StreamReader reader = new StreamReader(Application.streamingAssetsPath+"/"+trialsFileName))
    using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture))
    {
      csv.Context.TypeConverterCache.AddConverter<float>(new UtilsT.MillisToSeconds());
      csv.Read(); // Reads the header row
      csv.ReadHeader(); // Sets the HeaderRecord property

      csv.Read();
      bool exit = false;

      while (!exit)
      {
        EncounterData lastEncounter = new EncounterData();
        EncounterEntry encounterEntry = csv.GetRecord<EncounterEntry>();

        lastEncounter.EncounterBlockDelay = encounterEntry.BlockDelay;
        lastEncounter.EncounterObjectType = ConvertStringObjectType(encounterEntry.ObjectType);

        if (lastEncounter.EncounterObjectType ==
            EncounterData.ObjectType.BREAK) // break: load and go back to next entry as new encounter
        {
          encounters.Add(lastEncounter);
          if (!csv.Read())
            break;
        }
        else // Not a break: load encounter general data and then iterate through properties
        {
          lastEncounter.StimulusObjectId = GetId(encounterEntry.StimulusObject);
          string lastStimulusObject = encounterEntry.StimulusObject;

          int propertyCounter = 0;
          
          while (lastStimulusObject == encounterEntry.StimulusObject)
          {
            EncounterData.PropertyTrial propertyTrial = new EncounterData.PropertyTrial();
            propertyTrial.PropertyId = GetId(encounterEntry.StimulusProperty);
            propertyTrial.PropertyType = ConvertStringPropertyType(encounterEntry.PropertyType);
            propertyTrial.ValidProperty = encounterEntry.ValidProperty;
            lastEncounter.PropertyTrials.Add(propertyTrial);

            if (!csv.Read())
            {
              exit = true;
              break;
            }

            lastStimulusObject = encounterEntry.StimulusObject;
            propertyCounter++;
            encounterEntry = csv.GetRecord<EncounterEntry>();
          }

          lastEncounter.Health = propertyCounter; // Health by default is number of properties -1? TEMP
          encounters.Add(lastEncounter);
        }
      }
                
      print(encounters.Count);
    }
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
    foreach (var propertyTrial in encounters[encounterCounter].PropertyTrials)
    {
      SpawnAddToDictionary(GetModel(propertyTrial.PropertyId), propertyTrial.PropertyId);
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
    return encounters[encounterCounter].EncounterBlockDelay;
  }

  public float GetTotalWaitTime()
  {
    float wt = 0;
    foreach (var encounter in encounters)
    {
      wt += encounter.EncounterBlockDelay;
    }

    return wt;
  }

  public EncounterData.PropertyType GetCurrentEncounterType()
  {
    return encounters[encounterCounter].GetCurrentPropertyType();
  }

  public bool EncounterIsObject()
  {
    return encounters[encounterCounter].EncounterObjectType != EncounterData.ObjectType.BREAK;
  }

  private int GetCurrentEncounterId()
  {
    return encounters[encounterCounter].StimulusObjectId;
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

  // public class TrialData()
  // {
  //   
  // }
}
