using System;
using Assets;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class CameraController : MonoBehaviour
{
  [SerializeField] private Transform cameraTransform;
  
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

  private readonly float bobbingTime = 0.5f;
  private readonly float bobbingOffset = 0.05f;
  private Stopwatch stopwatch;
  private IEnumerator BobbingCameraCoroutine()
  {
    Vector3 basePosition = cameraTransform.position;
    stopwatch = Stopwatch.StartNew();
    
    while (true)
    {
      cameraTransform.position = basePosition + new Vector3(0, bobbingOffset * Mathf.Sin((2/bobbingTime) * Mathf.PI * (stopwatch.Elapsed.Milliseconds/1000f)), 0);
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

  private void SmoothToObject(Transform goal, float duration)
  {
    Vector3 position = goal.position;
    Quaternion rotation = goal.rotation;
    StartCoroutine(TransitionCamera(position, rotation, duration));
  }

  private void ImmediateToObject(Transform goal)
  {
    Vector3 position = goal.position;
    Quaternion rotation = goal.rotation;
    cameraTransform.position = position;
    cameraTransform.rotation = rotation;
  }

  private IEnumerator TransitionCamera(Vector3 position, Quaternion rotation, float duration)
  {
    Vector3 startingPos = cameraTransform.position;
    Quaternion startingRot = cameraTransform.rotation;

    float startTime = Time.realtimeSinceStartup;
    while (Time.realtimeSinceStartup < startTime + duration)
    {
      float x = (Time.realtimeSinceStartup - startTime) / duration;
      cameraTransform.position = Vector3.Lerp(startingPos, position, x);
      cameraTransform.rotation = Quaternion.Lerp(startingRot, rotation, x);
      yield return null;
    }
    cameraTransform.position = position;
    cameraTransform.rotation = rotation;
  }

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

  protected virtual void StartingEncounter(float encounterStartTime)
  {
    StopBobbingCamera();
    SmoothToObject(LocationHolder.EnemyCameraLocation, encounterStartTime);
  }

  protected virtual void SettingUpMind()
  {
    // ImmediateToObject(LocationHolder.MindCameraLocation);
    // OrthoCamera();
  }

  protected virtual void ThinkingOfProperty(bool encounterOver)
  {
    // if (!encounterOver) return;
    // ImmediateToObject(LocationHolder.EnemyCameraLocation);
  }

  protected virtual void EvaluatingEncounter()
  {
    // PerspectiveCamera();
  }

  protected virtual void EndingEncounter(float playerResetTime)
  {
    SmoothToObject(LocationHolder.BaseCameraLocation, playerResetTime);
  }
}
