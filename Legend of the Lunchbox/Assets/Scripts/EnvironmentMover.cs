using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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
        stopwatch.Start();
    }

    protected virtual void StartingEncounter(float encounterStartTime)
    {
        moving = false;
        stopwatch.Stop();
    }

    private readonly Stopwatch stopwatch = new Stopwatch();
    private void Update()
    {
        if (!moving)
            return;
        environmentMaterial.SetVector("_Offset", new Vector2(0, stopwatch.ElapsedMilliseconds * -0.0001f));
    }
}
