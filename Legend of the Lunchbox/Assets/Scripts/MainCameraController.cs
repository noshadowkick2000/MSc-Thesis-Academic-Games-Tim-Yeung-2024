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
  private IEnumerator BobbingCameraCoroutine()
  {
    Vector3 basePosition = mainObject.position;

    float startTime = Time.time;
    bool negativeMovement = false;
    while (true)
    {
      float x = (Time.time - startTime) / bobbingTime;
      if (x >= 1f)
      {
        startTime = Time.time;
        x = 0;
        negativeMovement = !negativeMovement;
      }

      float y = negativeMovement ? 1 - UtilsT.EasedT(x) : Mathf.Sin(x * .5f * MathF.PI);
      
      mainObject.position = basePosition + new Vector3(0, bobbingOffset * y, 0);
      yield return null;
    }
  }

  // private readonly float shakingTime = 0.2f;
  // private IEnumerator ShakeRoutine()
  // {
  //   Vector3 basePosition = mainObject.position;
  //   Vector3 offset = Vector3.zero;
  //
  //   float startTime = Time.time;
  //   float x = 0;
  //   
  //   while (x < 1f)
  //   {
  //     x = (Time.time - startTime) / shakingTime;
  //     offset = new Vector3((Mathf.PerlinNoise1D(x) - .5f) * 2, (Mathf.PerlinNoise1D(x / 2) - .5f) * 2, 0);
  //     mainObject.position = basePosition + offset;
  //     yield return null;
  //   }
  //   
  //   // mainObject.position = basePosition;
  // }

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
    GameEngine.StartingBreakStartedEvent += StartingBreak;
    GameEngine.StartingEncounterStartedEvent += StartingEncounter;
    GameEngine.SettingUpMindStartedEvent += SettingUpMind;
    GameEngine.EvaluatingEncounterStartedEvent += EvaluatingEncounter;
    GameEngine.LostEncounterStartedEvent += LostEncounter;
    GameEngine.EndingEncounterStartedEvent += EndingEncounter;
    GameEngine.EndingBreakStartedEvent += EndingBreak;
    GameEngine.LevelOverStartedEvent += LevelOver;
  }
  
  private void UnSubscribeToEvents()
  {
    GameEngine.OnRailStartedEvent -= OnRail;
    GameEngine.StartingBreakStartedEvent -= StartingBreak;
    GameEngine.StartingEncounterStartedEvent -= StartingEncounter;
    GameEngine.SettingUpMindStartedEvent -= SettingUpMind;
    GameEngine.EvaluatingEncounterStartedEvent -= EvaluatingEncounter;
    GameEngine.LostEncounterStartedEvent -= LostEncounter;
    GameEngine.EndingEncounterStartedEvent -= EndingEncounter;
    GameEngine.EndingBreakStartedEvent -= EndingBreak;
    GameEngine.LevelOverStartedEvent -= LevelOver;
  }

  protected virtual void OnRail()
  {
    ImmediateToObject(LocationHolder.BaseCameraLocation);
    StartBobbingCamera();
  }

  protected virtual void StartingBreak()
  {
    StopBobbingCamera();
    // ImmediateToObject(LocationHolder.DiscoverableCameraLocation);
    SmoothToObject(LocationHolder.EnemyCameraLocation, GameEngine.StaticTimeVariables.EncounterStartDuration, true);
  }

  protected virtual void EndingBreak()
  {
    SmoothToObject(LocationHolder.BaseCameraLocation, GameEngine.StaticTimeVariables.EncounterEndDuration, true);
  }

  protected virtual void StartingEncounter()
  {
    StopBobbingCamera();
    // ImmediateToObject(LocationHolder.DiscoverableCameraLocation);
    SmoothToObject(LocationHolder.EnemyCameraLocation, GameEngine.StaticTimeVariables.EncounterStartDuration, true);
  }

  protected virtual void SettingUpMind()
  {
    // ImmediateToObject(LocationHolder.MindCameraLocation);
    // OrthoCamera();
    
    SmoothToObject(LocationHolder.DiscoverableLocation, GameEngine.StaticTimeVariables.EncounterTrialStartDuration, true);
  }

  protected virtual void EvaluatingEncounter()
  {
    SmoothToObject(LocationHolder.EnemyCameraLocation, GameEngine.StaticTimeVariables.EncounterEndDuration, true);
  }

  protected virtual void LostEncounter()
  {
    // StartCoroutine(ShakeRoutine());
  }

  protected virtual void EndingEncounter()
  {
    SmoothToObject(LocationHolder.BaseCameraLocation, GameEngine.StaticTimeVariables.EncounterEndDuration, true);
  }

  protected virtual void LevelOver()
  {
    OnRail();
  }
}
