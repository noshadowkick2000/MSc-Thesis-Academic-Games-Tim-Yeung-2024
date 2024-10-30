using System;
using System.Collections;
using Assets;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
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

  private void Idle(bool encounterOver)
  {
    playerAnimator.SetTrigger("idle");
    transform.rotation = LocationHolder.BasePlayerLocation.rotation;
    transform.position = LocationHolder.BasePlayerLocation.position;
    playerMat.mainTexture = normalFace;
  }

  private void StartMind()
  {
    playerAnimator.SetTrigger("think");
    transform.rotation = LocationHolder.ThinkingPlayerLocation.rotation;
    transform.position = LocationHolder.ThinkingPlayerLocation.position;
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
    Idle(false);
  }

  protected virtual void SettingUpMind()
  {
    StartMind();
  }

  protected virtual void ThinkingOfProperty(bool encounterOver)
  {
    if (encounterOver)
      Idle(true);
  }
}
