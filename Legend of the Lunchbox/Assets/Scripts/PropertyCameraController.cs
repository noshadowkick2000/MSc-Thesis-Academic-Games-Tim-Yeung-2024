using System;
using System.Collections;
using System.Collections.Generic;
using Assets;
using UnityEngine;

public class PropertyCameraController : CameraController
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
        GameEngine.ShowingPropertyStartedEvent += ShowingProperty;
    }
  
    private void UnSubscribeToEvents()
    {
        GameEngine.StartingEncounterStartedEvent -= StartingEncounter;
        GameEngine.ShowingPropertyStartedEvent -= ShowingProperty;
    }

    protected virtual void StartingEncounter(float duration)
    {
        ImmediateToObject(start);
    }

    protected virtual void ShowingProperty(float duration, Action<InputHandler.InputState> callback)
    {
        ImmediateToObject(start);
        SmoothToObject(LocationHolder.MindCameraLocation, duration);
    }
}
