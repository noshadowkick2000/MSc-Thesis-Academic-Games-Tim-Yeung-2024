using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Assets;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnvironmentHandler : ObjectMover
{
    [SerializeField] private GameObject[] environmentTerrains;
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
    
    private void Awake()
    {
        SubscribeToEvents();

        SpawnTerrain(true);
    }

    private void OnDestroy()
    {
        UnSubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
        GameEngine.OnRailStartedEvent += OnRail;
        GameEngine.StartingEncounterStartedEvent += StartingEncounter;
        GameEngine.ShowingEnemyStartedEvent += ShowingEnemy;
        GameEngine.LevelOverStartedEvent += LevelOver;
    }
    
    private void UnSubscribeToEvents()
    {
        GameEngine.OnRailStartedEvent -= OnRail;
        GameEngine.StartingEncounterStartedEvent -= StartingEncounter;
        GameEngine.ShowingEnemyStartedEvent -= ShowingEnemy;
        GameEngine.LevelOverStartedEvent -= LevelOver;
    }
    
    protected virtual void OnRail()
    {
        moving = true;
        spawnedDiscoverable = Instantiate(discoverable, new Vector3(0, .2f, (GameEngine.RailDuration) * speedMultiplier + LocationHolder.EnemyLocation.position.z), Quaternion.identity, loadedTerrains.Last().transform);
    }

    protected virtual void StartingEncounter()
    {
        moving = false;
        mainObject = spawnedDiscoverable.transform;
        SmoothToObject(LocationHolder.EnemyLocation, GameEngine.EncounterStartTime, true);
    }

    protected virtual void ShowingEnemy()
    {
        Destroy(spawnedDiscoverable);
    }

    protected virtual void LevelOver()
    {
        moving = true;
    }

    private bool moving;
    public bool Moving => moving;

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
        if (!moving)
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
