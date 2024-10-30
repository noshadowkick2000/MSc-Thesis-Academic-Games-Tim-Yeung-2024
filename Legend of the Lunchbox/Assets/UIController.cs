using System;
using System.Collections;
using System.Collections.Generic;
using Assets;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private GameObject mindUI;
    [SerializeField] private GameObject thoughtUI;
    [SerializeField] private GameObject controlIndicatorUI;
    [SerializeField] private GameObject distractionUI;
    [SerializeField] private Slider timerUI;
    
    void Awake()
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

    private void Idle(bool encounterOver)
    {
        if (encounterOver) 
        {
            controlIndicatorUI.SetActive(false);
            StartCoroutine(AnimateCanvas(true)); 
        }
        thoughtUI?.SetActive(false);
    }

    private void StartMind()
    {
        distractionUI.SetActive(false);
        StartCoroutine(AnimateCanvas(false));
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

    private void StartThought() 
    { 
        thoughtUI.SetActive(true);
        controlIndicatorUI.SetActive(false);
    }

    Coroutine timerRoutine;

    private void EndThought(float timeOut) 
    { 
        thoughtUI.SetActive(false);
        controlIndicatorUI.SetActive(true);

        timerRoutine = StartCoroutine(AnimateTimer(timeOut));
    }

    private void ExitMind()
    {
        distractionUI.SetActive(true);
    }

    private void CancelTimer()
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
    
    //-----------------------------------------------------
    
    private void SubscribeToEvents()
    {
        GameEngine.SettingUpMindStartedEvent += SettingUpMind;
        GameEngine.ThinkingOfPropertyStartedEvent += ThinkingOfProperty;
        GameEngine.ShowingPropertyStartedEvent += ShowingProperty;
        GameEngine.EvaluatingInputStartedEvent += EvaluatingInput;
        GameEngine.EndingEncounterStartedEvent += EndingEncounter;
    }

    private void UnsubscribeFromEvents()
    {
        GameEngine.SettingUpMindStartedEvent -= SettingUpMind;
        GameEngine.ThinkingOfPropertyStartedEvent -= ThinkingOfProperty;
        GameEngine.ShowingPropertyStartedEvent -= ShowingProperty;
        GameEngine.EvaluatingInputStartedEvent -= EvaluatingInput;
        GameEngine.EndingEncounterStartedEvent -= EndingEncounter;
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

    protected virtual void EndingEncounter(float duration)
    {
        ExitMind();
    }
}
