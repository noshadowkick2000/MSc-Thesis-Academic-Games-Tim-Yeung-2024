using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Assets;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnvironmentHandler : MonoBehaviour
{
    [SerializeField] private GameObject[] environmentTerrains;
    [SerializeField] private GameObject discoverable;
    [SerializeField] private float speedMultiplier;
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
    }
    
    private void UnSubscribeToEvents()
    {
        GameEngine.OnRailStartedEvent -= OnRail;
        GameEngine.StartingEncounterStartedEvent -= StartingEncounter;
    }
    
    protected virtual void OnRail(float duration)
    {
        moving = true;
        spawnedDiscoverable = Instantiate(discoverable, new Vector3(Random.Range(-.3f, .3f), .2f, duration * speedMultiplier + LocationHolder.EnemyLocation.position.z), Quaternion.identity, loadedTerrains.Last().transform);
    }

    protected virtual void StartingEncounter(float encounterStartTime)
    {
        moving = false;
        
        // spawnedDiscoverable // TODO DISCOVERABLE DESPAWNS AND OBJECT RISES FROM OUT OF FRAME
    }

    private bool moving;

    private void SpawnTerrain(bool first)
    {
        int newId = lastId;
        while (newId == lastId && environmentTerrains.Length > 1)
            newId = Random.Range(0, environmentTerrains.Length);
        lastId = newId;
        
        if (first)
            loadedTerrains.Add(Instantiate(environmentTerrains[newId], spawnPosition + Vector3.back * spawnGap, Quaternion.identity, transform));
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
