using System;
using System.Collections;
using System.Collections.Generic;
using Assets;
using UnityEngine;

public class EnvironmentMover : MonoBehaviour
{
    [SerializeField] private Material environmentMaterial;

    private void Awake()
    {
        SubscribeToEvents();
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

    private bool moving = false;
    protected virtual void OnRail()
    {
        moving = true;
    }

    protected virtual void StartingEncounter(float encounterStartTime)
    {
        moving = false;
    }

    private void Update()
    {
        if (!moving)
            return;

        environmentMaterial.SetVector("_Offset", new Vector2(0, Time.time * -0.1f));
    }
}
