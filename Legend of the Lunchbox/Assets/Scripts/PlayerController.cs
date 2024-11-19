using System;
using System.Collections;
using Assets;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
  // [SerializeField] private Transform playerTransform;
  [SerializeField] private Material playerMat;
  [SerializeField] private Texture2D normalFace;
  [SerializeField] private Texture2D thinkingFace;
  [SerializeField] private Animator playerAnimator;

  private void Awake()
  {
    SubscribeToEvents();
  }

  private void OnDestroy()
  {
    UnsubscribeFromEvents();
  }

  private void Idle()
  {
    playerAnimator.SetTrigger("idle");
    // playerTransform.rotation = LocationHolder.BasePlayerLocation.rotation;
    // playerTransform.position = LocationHolder.BasePlayerLocation.position;
    playerMat.mainTexture = normalFace;
  }

  private void StartMind()
  {
    playerAnimator.SetTrigger("think");
    // playerTransform.rotation = LocationHolder.ThinkingPlayerLocation.rotation;
    // playerTransform.position = LocationHolder.ThinkingPlayerLocation.position;
    playerMat.mainTexture = thinkingFace;
  }
  
  //-------------------------------------------------------

  private void SubscribeToEvents()
  {
    GameEngine.ShowingEnemyStartedEvent += ShowingEnemy;
    GameEngine.SettingUpMindStartedEvent += SettingUpMind;
    GameEngine.ThinkingOfPropertyStartedEvent += ThinkingOfProperty;
  }

  private void UnsubscribeFromEvents()
  {
    GameEngine.ShowingEnemyStartedEvent -= ShowingEnemy;
    GameEngine.SettingUpMindStartedEvent -= SettingUpMind;
    GameEngine.ThinkingOfPropertyStartedEvent -= ThinkingOfProperty;
  }

  protected virtual void ShowingEnemy()
  {
    Idle();
  }

  protected virtual void SettingUpMind()
  {
    StartMind();
  }

  protected virtual void ThinkingOfProperty(bool encounterOver)
  {
    if (encounterOver)
      Idle();
  }
}
