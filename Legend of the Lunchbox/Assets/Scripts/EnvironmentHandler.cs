using System.Collections.Generic;
using System.Linq;
using Assets;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnvironmentHandler : ObjectMover
{
    [System.Serializable]
    private struct EnvironmentPrefabs
    {
        public GameObject[] environmentPrefabs;
        public Material skyBoxMaterial;
        public float directionalLightIntensity;
        public Color fogColor;
    }
    
    [SerializeField] private EnvironmentPrefabs[] environmentTerrainSets;
    [SerializeField] private GameObject discoverable;
    [SerializeField] private float speedMultiplier;
    [SerializeField] private int maxTiles = 3;
    public float SpeedMultiplier => speedMultiplier;
    [SerializeField] private Vector3 spawnPosition;
    [SerializeField] private float spawnGap;
    [SerializeField] private float despawnZ;

    private int lastId;

    private List<GameObject> loadedTerrains = new List<GameObject>();
    private List<GameObject> despawnTerrains = new List<GameObject>();

    private GameObject spawnedDiscoverable;

    private GameObject[] environmentTerrains;

    public enum EnvironmentType
    {
        MEADOWS,
        LAKEBANK,
        TOWER
    }

    private void SetLevelEnvironment()
    {
        EnvironmentPrefabs ep = environmentTerrainSets[(int)TrialHandler.currentEnvironment];
        environmentTerrains = ep.environmentPrefabs;
        RenderSettings.skybox = ep.skyBoxMaterial;
        Light mainLight = FindObjectOfType<Light>();
        RenderSettings.sun = mainLight;
        RenderSettings.fogColor = ep.fogColor;
        mainLight.intensity = ep.directionalLightIntensity;
    }
    
    private void Awake()
    {
        SubscribeToEvents();

        SetLevelEnvironment();

        SpawnTerrain(true);
    }

    private void OnDestroy()
    {
        UnSubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
        GameEngine.OnRailStartedEvent += OnRail;
        GameEngine.StartingBreakStartedEvent += StartingBreak;
        GameEngine.BreakingBadStartedEvent += BreakingBad;
        GameEngine.StartingEncounterStartedEvent += StartingEncounter;
        GameEngine.SettingUpMindStartedEvent += SettingUpMind;
        GameEngine.LevelOverStartedEvent += LevelOver;
    }
    
    private void UnSubscribeToEvents()
    {
        GameEngine.OnRailStartedEvent -= OnRail;
        GameEngine.StartingBreakStartedEvent -= StartingBreak;
        GameEngine.BreakingBadStartedEvent -= BreakingBad;
        GameEngine.StartingEncounterStartedEvent -= StartingEncounter;
        GameEngine.SettingUpMindStartedEvent -= SettingUpMind;
        GameEngine.LevelOverStartedEvent -= LevelOver;
    }
    
    private void OnRail()
    {
        Moving = true;
        spawnedDiscoverable = Instantiate(discoverable, new Vector3(0, .2f, (GameEngine.CurrentRailDuration) * speedMultiplier + (LocationHolder.DiscoverableLocation.position.z)), Quaternion.identity, loadedTerrains.Last().transform);
    }

    private void StartingBreak()
    {
        StartingEncounter();
    }

    private void BreakingBad()
    {
        SettingUpMind();
    }
    
    private void StartingEncounter()
    {
        Moving = false;
        mainObject = spawnedDiscoverable.transform;
        SmoothToObject(LocationHolder.DiscoverableLocation, GameEngine.StaticTimeVariables.EncounterStartDuration, true);
    }

    private void SettingUpMind()
    {
        mainObject.parent = Camera.main.transform;
        StartCoroutine(GrowObject(GameEngine.StaticTimeVariables.EncounterTrialStartDuration, 1, 2, true));
    }

    private void LevelOver()
    {
        Moving = true;
    }

    public bool Moving { get; private set; }

    private void SpawnTerrain(bool first)
    {
        int newId = lastId;
        while (newId == lastId && environmentTerrains.Length > 1)
            newId = Random.Range(0, environmentTerrains.Length);
        lastId = newId;

        if (first)
        {
            for (int i = maxTiles; i > 0; i--)
            {
                loadedTerrains.Add(Instantiate(environmentTerrains[newId], spawnPosition + spawnGap * i * Vector3.back, Quaternion.identity, transform));
            }
        }
        loadedTerrains.Add(Instantiate(environmentTerrains[newId], loadedTerrains.Last().transform.position + Vector3.forward * spawnGap, Quaternion.identity, transform));
    }
    
    private void Update()
    {
        if (!Moving)
            return;
        
        foreach (var terrain in loadedTerrains)
        {
            terrain.transform.position += Time.deltaTime * speedMultiplier * Vector3.back;
            
            if (terrain.transform.position.z < despawnZ)
                despawnTerrains.Add(terrain);
        }
        
        if (Vector3.Distance(loadedTerrains.Last().transform.position, spawnPosition) >= spawnGap)
            SpawnTerrain(false);
    
        foreach (var despawnTerrain in despawnTerrains)
        {
            loadedTerrains.Remove(despawnTerrain);
            Destroy(despawnTerrain);
        }
        
        despawnTerrains.Clear();
    }
}
