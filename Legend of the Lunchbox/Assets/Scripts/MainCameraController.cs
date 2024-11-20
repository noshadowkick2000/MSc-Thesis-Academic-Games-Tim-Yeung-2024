using System;
using Assets;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class MainCameraController : ObjectMover
{
  private void Awake()
  {
    SubscribeToEvents();
  }

  private void OnDestroy()
  {
    UnSubscribeToEvents();
  }

  private Coroutine bobbingCoroutine;
  private void StartBobbingCamera()
  {
    bobbingCoroutine = StartCoroutine(BobbingCameraCoroutine());
  }

  private void StopBobbingCamera()
  {
    StopCoroutine(bobbingCoroutine);
  }

  private readonly float bobbingTime = 0.2f;
  private readonly float bobbingOffset = 0.1f;
  private Stopwatch stopwatch;
  private IEnumerator BobbingCameraCoroutine()
  {
    Vector3 basePosition = mainObject.position;
    stopwatch = Stopwatch.StartNew();

    float startTime = Time.realtimeSinceStartup;
    bool negativeMovement = false;
    while (true)
    {
      float x = (Time.realtimeSinceStartup - startTime) / bobbingTime;
      if (x >= 1f)
      {
        startTime = Time.realtimeSinceStartup;
        x = 0;
        negativeMovement = !negativeMovement;
      }

      float y = negativeMovement ? 1 - MathT.EasedT(x) : Mathf.Sin(x * .5f * MathF.PI);
      
      mainObject.position = basePosition + new Vector3(0, bobbingOffset * y, 0);
      yield return null;
    }
  }

  // private void OrthoCamera()
  // {
  //   mainCamera.orthographic = true;
  // }
  //
  // private void PerspectiveCamera()
  // {
  //   mainCamera.orthographic = false;
  // }

  //-------------------------------------------------------

  private void SubscribeToEvents()
  {
    GameEngine.OnRailStartedEvent += OnRail;
    GameEngine.StartingEncounterStartedEvent += StartingEncounter;
    GameEngine.SettingUpMindStartedEvent += SettingUpMind;
    GameEngine.ThinkingOfPropertyStartedEvent += ThinkingOfProperty;
    GameEngine.EvaluatingEncounterStartedEvent += EvaluatingEncounter;
    GameEngine.EndingEncounterStartedEvent += EndingEncounter;
  }
  
  private void UnSubscribeToEvents()
  {
    GameEngine.OnRailStartedEvent -= OnRail;
    GameEngine.StartingEncounterStartedEvent -= StartingEncounter;
    GameEngine.SettingUpMindStartedEvent -= SettingUpMind;
    GameEngine.ThinkingOfPropertyStartedEvent -= ThinkingOfProperty;
    GameEngine.EvaluatingEncounterStartedEvent -= EvaluatingEncounter;
    GameEngine.EndingEncounterStartedEvent -= EndingEncounter;
  }

  protected virtual void OnRail()
  {
    ImmediateToObject(LocationHolder.BaseCameraLocation);
    StartBobbingCamera();
  }

  protected virtual void StartingEncounter()
  {
    StopBobbingCamera();
    SmoothToObject(LocationHolder.EnemyCameraLocation, GameEngine.EncounterStartTime, true);
  }

  protected virtual void SettingUpMind()
  {
    // ImmediateToObject(LocationHolder.MindCameraLocation);
    // OrthoCamera();
    
    SmoothToObject(LocationHolder.EnemyLocation, GameEngine.MindStartTime, true);
  }

  protected virtual void ThinkingOfProperty(bool encounterOver)
  {
    if (!encounterOver) return;
    SmoothToObject(LocationHolder.EnemyCameraLocation, .5f, true);
  }

  protected virtual void EvaluatingEncounter()
  {
    
  }

  protected virtual void EndingEncounter()
  {
    SmoothToObject(LocationHolder.BaseCameraLocation, GameEngine.playerReset, true);
  }
}
