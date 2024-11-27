using System;
using System.Collections;
using System.Collections.Generic;
using Assets;
using UnityEngine;

public class PropertyCameraController : ObjectMover
{
    [SerializeField] private Transform start;
    
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
        GameEngine.StartingEncounterStartedEvent += StartingEncounter;
        GameEngine.ThinkingOfPropertyStartedEvent += ShowingProperty;
        GameEngine.EvaluatingInputStartedEvent += EvaluatingInput;
    }
  
    private void UnSubscribeToEvents()
    {
        GameEngine.StartingEncounterStartedEvent -= StartingEncounter;
        GameEngine.ThinkingOfPropertyStartedEvent -= ShowingProperty;
        GameEngine.EvaluatingInputStartedEvent -= EvaluatingInput;
    }

    protected virtual void StartingEncounter()
    {
        ImmediateToObject(start);
    }

    private Coroutine cameraRoutine;
    protected virtual void ShowingProperty(bool encounterOver)
    {
        // ImmediateToObject(start);
        // cameraRoutine = SmoothToObject(LocationHolder.MindCameraLocation, GameEngine.EnemyTimeOut, true);
    }

    protected virtual void EvaluatingInput()
    {
        // StopCoroutine(cameraRoutine);
    }
}
