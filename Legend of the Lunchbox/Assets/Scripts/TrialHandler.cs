using Assets;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CsvHelper;
using UnityEngine;

public class TrialHandler : MonoBehaviour
{
    public static readonly string TrialsFileName = "LOTL_trials.csv";

    private readonly List<EncounterData> encounters = new List<EncounterData>();
    private int encounterCounter = 0;

    public static EncounterData CurrentEncounterData;
    public static EnvironmentHandler.EnvironmentType CurrentEnvironment;

    public delegate void SpawnEvent(Transform property);

    private void Awake()
    {
        LoadEncounters();

        SubscribeToEvents();
    }

    private void OnDestroy()
    {
        UnSubscribeToEvents();
    }

    public class EncounterEntry
    {
        public int Level { get; set; }
        public string Environment { get; set; }
        public float BlockDelay { get; set; }
        public string ObjectType { get; set; }
        public string StimulusObject { get; set; }
        public string PropertyType { get; set; }
        public string StimulusProperty { get; set; }
        public bool ValidProperty { get; set; }
        public float ITI { get; set; }
    }
    
    public static EncounterData.ObjectType ConvertStringObjectType(string objectType)
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
                throw new Exception($"{objectType} not a valid object type");
        }
    }

    public static EncounterData.PropertyType ConvertStringPropertyType(string propertyType)
    {
        switch (propertyType)
        {
            case "ACTION":
                return EncounterData.PropertyType.ACTION;
            case "SOUND":
                return EncounterData.PropertyType.SOUND;
            case "WORD":
                return EncounterData.PropertyType.WORD;
            default:
                throw new Exception($"{propertyType} is not a valid property type");
        }
    }

    public static EnvironmentHandler.EnvironmentType ConvertStringEnvironmentType(string environmentType)
    {
        switch (environmentType)
        {
            case "MEADOWS":
                return EnvironmentHandler.EnvironmentType.MEADOWS;
            case "LAKEBANK":
                return EnvironmentHandler.EnvironmentType.LAKEBANK;
            case "TOWER":
                return EnvironmentHandler.EnvironmentType.TOWER;
            default:
                throw new Exception($"{environmentType} not a valid environment type");
        }
    }

    private void LoadEncounters()
    {
        bool readEnvironment = false;

        using (StreamReader reader = new StreamReader(Application.streamingAssetsPath + "/" + TrialsFileName))
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

                if (encounterEntry.Level != LevelHandler.CurrentLevel)
                {
                    if (!csv.Read())
                    {
                        LevelHandler.LastLevel = encounterEntry.Level;
                        break;
                    }
                    continue;
                }

                if (!readEnvironment)
                {
                    readEnvironment = true;
                    CurrentEnvironment = ConvertStringEnvironmentType(encounterEntry.Environment);
                }

                lastEncounter.EncounterBlockDelay = encounterEntry.BlockDelay;
                lastEncounter.EncounterObjectType = ConvertStringObjectType(encounterEntry.ObjectType);

                if (lastEncounter.EncounterObjectType ==
                    EncounterData.ObjectType.BREAK) // break: load and go back to next entry as new encounter
                {
                    lastEncounter.StimulusObjectName = "Break";
                    EncounterData.PropertyTrial propertyTrial = new EncounterData.PropertyTrial();
                    propertyTrial.PropertyName = "None";
                    lastEncounter.PropertyTrials.Add(propertyTrial);
                    encounters.Add(lastEncounter);
                    if (!csv.Read())
                    {
                        LevelHandler.LastLevel = encounterEntry.Level;
                        break;
                    }
                }
                else // Not a break: load encounter general data and then iterate through properties
                {
                    lastEncounter.StimulusObjectId = UtilsT.GetId(encounterEntry.StimulusObject);
                    lastEncounter.StimulusObjectName = encounterEntry.StimulusObject;
                    string lastStimulusObject = encounterEntry.StimulusObject;

                    int propertyCounter = 0;
                    while (lastStimulusObject == encounterEntry.StimulusObject && LevelHandler.CurrentLevel == encounterEntry.Level)
                    {
                        EncounterData.PropertyTrial propertyTrial = new EncounterData.PropertyTrial();
                        propertyTrial.PropertyId = UtilsT.GetId(encounterEntry.StimulusProperty);
                        propertyTrial.PropertyName = encounterEntry.StimulusProperty;
                        propertyTrial.PropertyType = ConvertStringPropertyType(encounterEntry.PropertyType);
                        propertyTrial.Iti = encounterEntry.ITI;
                        propertyTrial.ValidProperty = encounterEntry.ValidProperty;
                        lastEncounter.PropertyTrials.Add(propertyTrial);

                        propertyCounter++;
                        
                        if (!csv.Read())
                        {
                            LevelHandler.LastLevel = encounterEntry.Level;
                            exit = true;
                            break;
                        }

                        lastStimulusObject = encounterEntry.StimulusObject;
                        encounterEntry = csv.GetRecord<EncounterEntry>();
                    }

                    lastEncounter.Health = propertyCounter; // Health by default is number of properties -1? TEMP
                    encounters.Add(lastEncounter);
                    print(encounterEntry.StimulusObject);
                    print(propertyCounter);
                }
            }

            print(encounters.Count);
        }
    }

    // private GameObject GetModel(int id)
    // {
    //   foreach (var propobj in GameEngine.PropertiesAndObjects)
    //   {
    //     if (UtilsT.GetId(propobj.name) == id)
    //       return propobj;
    //   }
    //
    //   return null;
    // }

    private Dictionary<int, Transform> objectDictionary = new Dictionary<int, Transform>();

    private void PrepareModels()
    {
        void SpawnAddToDictionary(GameObject obj, int id)
        {
            obj.SetActive(false);
            objectDictionary.Add(id, obj.transform);
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
        if (encounters[encounterCounter].EncounterObjectType != EncounterData.ObjectType.WORD)
            SpawnAddToDictionary(ExternalAssetLoader.GetAsset(GetCurrentEncounterObjectId()), GetCurrentEncounterObjectId());

        // Add property gameobjects
        foreach (var propertyTrial in encounters[encounterCounter].PropertyTrials)
        {
            if (propertyTrial.PropertyType != EncounterData.PropertyType.WORD)
                SpawnAddToDictionary(ExternalAssetLoader.GetAsset(propertyTrial.PropertyId), propertyTrial.PropertyId);
        }
    }

    public static event SpawnEvent OnObjectSpawnedEvent;

    private void StartEncounter()
    {
        Transform obj;
        if (GetCurrentEncounterObjectType() != EncounterData.ObjectType.WORD)
            obj = objectDictionary[GetCurrentEncounterObjectId()];
        else
            obj = ExternalAssetLoader.GetTextAsset(GetCurrentEncounterObjectName()).transform;
        obj.gameObject.SetActive(true);
        obj.position = LocationHolder.PropertyLocation.position;
        OnObjectSpawnedEvent?.Invoke(obj);
    }

    public float GetCurrentBlockDelay()
    {
        return encounters[encounterCounter].EncounterBlockDelay;
    }

    public float GetCurrentTrialDelay()
    {
        return encounters[encounterCounter].GetCurrentTrialDelay();
    }

    public float GetTotalBlockDelay()
    {
        float wt = 0;
        foreach (var encounter in encounters)
        {
            wt += encounter.EncounterBlockDelay;
        }

        return wt;
    }

    public EncounterData.PropertyType GetCurrentEncounterPropertyType()
    {
        return encounters[encounterCounter].GetCurrentPropertyType();
    }

    public bool EncounterIsObject()
    {
        return encounters[encounterCounter].EncounterObjectType != EncounterData.ObjectType.BREAK;
    }

    private int GetCurrentEncounterObjectId()
    {
        return encounters[encounterCounter].StimulusObjectId;
    }

    private EncounterData.ObjectType GetCurrentEncounterObjectType()
    {
        return encounters[encounterCounter].EncounterObjectType;
    }

    private string GetCurrentEncounterObjectName()
    {
        return encounters[encounterCounter].StimulusObjectName;
    }

    public static event SpawnEvent OnPropertySpawnedEvent;

    private void SpawnProperty()
    {
        Transform property;
        if (GetCurrentEncounterPropertyType() != EncounterData.PropertyType.WORD)
            property = objectDictionary[encounters[encounterCounter].GetCurrentPropertyId()];
        else
            property = ExternalAssetLoader.GetTextAsset(encounters[encounterCounter].GetCurrentPropertyName()).transform;

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

    public void IncreaseCounter() { encounters[encounterCounter].IncreaseCounter(); }

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
        encounterCounter++;
    }

    public bool LevelOver => encounterCounter == encounters.Count;

    //-------------------------------------------------------

    private void SubscribeToEvents()
    {
        GameEngine.OnRailStartedEvent += OnRail;
        GameEngine.StartingBreakStartedEvent += StartingBreak;
        GameEngine.StartingEncounterStartedEvent += StartingEncounter;
        GameEngine.ShowingPropertyStartedEvent += ShowingProperty;
        GameEngine.TimedOutStartedEvent += TimedOut;
        GameEngine.AnswerCorrectStartedEvent += AnswerCorrect;
        GameEngine.EndingEncounterStartedEvent += EndingEncounter;
        GameEngine.EndingBreakStartedEvent += EndingBreak;
    }

    private void UnSubscribeToEvents()
    {
        GameEngine.OnRailStartedEvent -= OnRail;
        GameEngine.StartingBreakStartedEvent -= StartingBreak;
        GameEngine.StartingEncounterStartedEvent -= StartingEncounter;
        GameEngine.ShowingPropertyStartedEvent -= ShowingProperty;
        GameEngine.TimedOutStartedEvent -= TimedOut;
        GameEngine.AnswerCorrectStartedEvent -= AnswerCorrect;
        GameEngine.EndingEncounterStartedEvent -= EndingEncounter;
        GameEngine.EndingBreakStartedEvent -= EndingBreak;
    }

    private void OnRail()
    {
        CurrentEncounterData = null;
    }

    private void StartingBreak()
    {
        CurrentEncounterData = encounters[encounterCounter];
    }

    private void EndingBreak()
    {
        encounterCounter++;
    }

    private void StartingEncounter()
    {
        CurrentEncounterData = encounters[encounterCounter];
        
        PrepareModels();
        StartEncounter();
    }

    private void ShowingProperty(Action<InputHandler.InputState> callback)
    {
        SpawnProperty();
    }

    private void TimedOut(InputHandler.InputState input)
    {
        if (input == InputHandler.InputState.NONE)
            SkipProperty();
    }

    private void AnswerCorrect()
    {
        DamageEncounter();
    }

    private void EndingEncounter()
    {
        KillEncounter();
    }

    // public class TrialData()
    // {
    //   
    // }
}