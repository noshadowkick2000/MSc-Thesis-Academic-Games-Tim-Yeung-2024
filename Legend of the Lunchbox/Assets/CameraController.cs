using System;
using Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour 
{
  
  private void Awake()
  {
    SubscribeToEvents();
  }

  private void OnDestroy()
  {
    UnSubscribeToEvents();
  }

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
    transform.position = position;
    transform.rotation = rotation;
  }

  private IEnumerator TransitionCamera(Vector3 position, Quaternion rotation, float duration)
  {
    Vector3 startingPos = transform.position;
    Quaternion startingRot = transform.rotation;

    float startTime = Time.realtimeSinceStartup;
    while (Time.realtimeSinceStartup < startTime + duration)
    {
      float x = (Time.realtimeSinceStartup - startTime) / duration;
      transform.position = Vector3.Lerp(startingPos, position, x);
      transform.rotation = Quaternion.Lerp(startingRot, rotation, x);
      yield return null;
    }
    transform.position = position;
    transform.rotation = rotation;
  }

  //-------------------------------------------------------

  private void SubscribeToEvents()
  {
    GameEngine.OnRailStartedEvent += OnRail;
    GameEngine.StartingEncounterStartedEvent += StartingEncounter;
    GameEngine.SettingUpMindStartedEvent += SettingUpMind;
    GameEngine.ThinkingOfPropertyStartedEvent += ThinkingOfProperty;
    GameEngine.EndingEncounterStartedEvent += EndingEncounter;
  }
  
  private void UnSubscribeToEvents()
  {
    GameEngine.OnRailStartedEvent -= OnRail;
    GameEngine.StartingEncounterStartedEvent -= StartingEncounter;
    GameEngine.SettingUpMindStartedEvent -= SettingUpMind;
    GameEngine.ThinkingOfPropertyStartedEvent -= ThinkingOfProperty;
    GameEngine.EndingEncounterStartedEvent -= EndingEncounter;
  }

  protected virtual void OnRail()
  {
    ImmediateToObject(LocationHolder.BaseCameraLocation);
  }

  protected virtual void StartingEncounter(float encounterStartTime)
  {
    SmoothToObject(LocationHolder.EnemyCameraLocation, encounterStartTime);
  }

  protected virtual void SettingUpMind()
  {
    ImmediateToObject(LocationHolder.MindCameraLocation);
  }

  protected virtual void ThinkingOfProperty(bool encounterOver)
  {
    if (encounterOver)
      ImmediateToObject(LocationHolder.EnemyCameraLocation);
  }

  protected virtual void EndingEncounter(float playerResetTime)
  {
    SmoothToObject(LocationHolder.BaseCameraLocation, playerResetTime);
  }
}
