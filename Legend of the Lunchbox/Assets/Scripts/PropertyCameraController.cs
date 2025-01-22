using System;
using System.Collections;
using System.Collections.Generic;
using Assets;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class PropertyCameraController : ObjectMover
{
    [SerializeField] private float minRotation;
    [SerializeField] private float maxRotation;

    public static Transform PropertyCamTransform { get; private set; }

    private void Awake()
    {
        PropertyCamTransform = mainObject;
        SubscribeToEvents();
    }

    private void OnDestroy()
    {
        UnSubscribeToEvents();
    }
    
    private void SubscribeToEvents()
    {
        GameEngine.StartingEncounterStartedEvent += StartingEncounter;
        GameEngine.MovingToPropertyStartedEvent += MovingToProperty;
        GameEngine.MovingToObjectStartedEvent += MovingToObject;
    }
  
    private void UnSubscribeToEvents()
    {
        GameEngine.StartingEncounterStartedEvent -= StartingEncounter;
        GameEngine.MovingToPropertyStartedEvent -= MovingToProperty;
        GameEngine.MovingToObjectStartedEvent -= MovingToObject;
    }

    private void StartingEncounter()
    {
        ImmediateToObject(LocationHolder.MindCameraLocation);
    }

    private Coroutine cameraRoutine;
    private void MovingToProperty(EncounterData.PropertyType propertyType)
    {
        cameraRoutine = SmoothToObject(mainObject.position + Vector3.forward, mainObject.rotation, GameEngine.StaticTimeVariables.ExplanationPromptDuration / 4, true);
    }

    private void MovingToObject()
    {
        SmoothToObject(LocationHolder.MindCameraLocation, GameEngine.StaticTimeVariables.TrialEndDuration, true);
    }
}
