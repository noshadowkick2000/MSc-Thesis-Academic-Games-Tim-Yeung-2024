using System;
using System.Collections;
using System.Collections.Generic;
using Assets;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
  [SerializeField] private Material playerMat;
  [SerializeField] private Texture2D normalFace;
  [SerializeField] private Texture2D thinkingFace;
  [SerializeField] private GameObject mindUI;
  [SerializeField] private GameObject thoughtUI;
  [SerializeField] private GameObject controlIndicatorUI;
  [SerializeField] private Slider timerUI;
  [SerializeField] private Animator playerAnimator;

  private void Awake()
  {
    thoughtUI.SetActive(false);
    mindUI.SetActive(false);
    controlIndicatorUI.SetActive(false);

    SubscribeToEvents();
  }

  private void OnDestroy()
  {
    UnsubscribeFromEvents();
  }

  public void Idle(bool encounterOver)
  {
    if (encounterOver) 
    {
      controlIndicatorUI.SetActive(false);
      StartCoroutine(AnimateCanvas(true)); 
    }

    playerAnimator.SetTrigger("idle");
    thoughtUI?.SetActive(false);
    transform.rotation = LocationHolder.BasePlayerLocation.rotation;
    transform.position = LocationHolder.BasePlayerLocation.position;
    playerMat.mainTexture = normalFace;
  }

  public void StartMind()
  {
    playerAnimator.SetTrigger("think");
    StartCoroutine(AnimateCanvas(false));
    transform.rotation = LocationHolder.ThinkingPlayerLocation.rotation;
    transform.position = LocationHolder.ThinkingPlayerLocation.position;
    playerMat.mainTexture = thinkingFace;
  }

  private IEnumerator AnimateCanvas(bool inverse) 
  {
    if (!inverse)
      mindUI.SetActive(true);

    RectTransform mt = mindUI.GetComponent<RectTransform>();
    mt.localScale = inverse ? Vector3.one : Vector3.zero;

    float duration = 0.5f; //TODO make this dependent on start time GameEngine

    float startTime = Time.realtimeSinceStartup;
    while (Time.realtimeSinceStartup < startTime + duration)
    {
      float x = (Time.realtimeSinceStartup - startTime) / duration;
      if (inverse) { x = 1 - x; }
      mt.localScale = new Vector3 (x, x, 1);
      yield return null;
    }

    mt.localScale = inverse ? Vector3.zero : Vector3.one;
    if (inverse)
      mindUI.SetActive(false);
  }

  public void StartThought() 
  { 
    thoughtUI.SetActive(true);
    controlIndicatorUI.SetActive(false);
  }

  Coroutine timerRoutine;
  public void EndThought(float timeOut) 
  { 
    thoughtUI.SetActive(false);
    controlIndicatorUI.SetActive(true);

    timerRoutine = StartCoroutine(AnimateTimer(timeOut));
  }

  public void CancelTimer()
  {
    StopCoroutine(timerRoutine);
  }

  private IEnumerator AnimateTimer(float timeOut)
  {
    float startTime = Time.realtimeSinceStartup;
    while (Time.realtimeSinceStartup < startTime + timeOut)
    {
      float x = 1 - (Time.realtimeSinceStartup - startTime) / timeOut;
      timerUI.value = x;
      yield return null;
    }
  }
  
  //-------------------------------------------------------

  private void SubscribeToEvents()
  {
    GameEngine.ShowingEnemyStartedEvent += ShowingEnemy;
    GameEngine.SettingUpMindStartedEvent += SettingUpMind;
    GameEngine.ThinkingOfPropertyStartedEvent += ThinkingOfProperty;
    GameEngine.ShowingPropertyStartedEvent += ShowingProperty;
    GameEngine.EvaluatingInputStartedEvent += EvaluatingInput;
  }

  private void UnsubscribeFromEvents()
  {
    GameEngine.ShowingEnemyStartedEvent -= ShowingEnemy;
    GameEngine.SettingUpMindStartedEvent -= SettingUpMind;
    GameEngine.ThinkingOfPropertyStartedEvent -= ThinkingOfProperty;
    GameEngine.ShowingPropertyStartedEvent -= ShowingProperty;
    GameEngine.EvaluatingInputStartedEvent -= EvaluatingInput;
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
    else
      StartThought();
  }

  protected virtual void ShowingProperty(float enemyTimeOut, Action<InputHandler.InputState> callback)
  {
    EndThought(enemyTimeOut);
  }

  protected virtual void EvaluatingInput()
  {
    CancelTimer();
  }
}
