using System;
using System.Collections;
using System.Collections.Generic;
using Assets;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private GameObject mindUI;
    [SerializeField] private Image mindPanel;
    [SerializeField] private Color spotColor;
    [SerializeField] private GameObject spotLight;
    [SerializeField] private Material spotLightMaterial;
    [SerializeField] private GameObject thoughtUI;
    [SerializeField] private Sprite[] thoughtSprites;
    [SerializeField] private GameObject controlIndicatorUI;
    [SerializeField] private GameObject distractionUI;
    // [SerializeField] private Image timerUI;
    
    void Awake()
    {
        thoughtUI.SetActive(false);
        mindUI.SetActive(false);
        controlIndicatorUI.SetActive(false);
        spotLight.SetActive(false);
        
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
            StartCoroutine(AnimateCanvas(true, .5f)); 
        }
        thoughtUI?.SetActive(false);
    }

    private void StartMind(float duration)
    {
        distractionUI.SetActive(false);
        StartCoroutine(AnimateCanvas(false, duration));
    }
    
    private IEnumerator AnimateCanvas(bool inverse, float duration) 
    {
        if (!inverse)
        {
            mindUI.SetActive(true);
            // spotLight.SetActive(true);
        }

        // RectTransform mt = mindUI.GetComponent<RectTransform>();
        mindPanel.color = inverse ? Color.black : Color.clear;
        // mt.localScale = inverse ? Vector3.one : Vector3.zero;
        // spotLightMaterial.color = Color.clear;  
        
        float startTime = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup < startTime + duration)
        {
            float x = (Time.realtimeSinceStartup - startTime) / duration;
            if (inverse) { x = 1 - x; }
            // mt.localScale = new Vector3 (x, x, 1);
            mindPanel.color = new Color(0, 0,0 , x);
            // Color sc = new Color(spotColor.r, spotColor.g, spotColor.b, x);
            // spotLightMaterial.color = sc;
            yield return null;
        }

        mindPanel.color = inverse ? Color.clear : Color.black;
        // mt.localScale = inverse ? Vector3.zero : Vector3.one;
        spotLightMaterial.color = spotColor;
        if (inverse)
        {
            mindUI.SetActive(false);
            spotLight.SetActive(false);
        }
    }

    private Coroutine thoughtRoutine;

    private void StartThought(float duration) 
    { 
        //TODO spotlights searching around effect
        
        // thoughtUI.SetActive(true);
        controlIndicatorUI.SetActive(false);

        // thoughtRoutine = StartCoroutine(AnimateThought());
        StartCoroutine(DelayedSpotlight(duration));
    }

    private IEnumerator DelayedSpotlight(float duration)
    {
        yield return new WaitForSecondsRealtime(duration / 4f);
        spotLight.SetActive(true);
    }

    private IEnumerator AnimateThought()
    {
        Image img = thoughtUI.GetComponent<Image>();
        int counter = 0;

        while (true)
        {
            img.sprite = thoughtSprites[counter];
            counter++;
            if (counter == thoughtSprites.Length)
                counter = 0;
            
            yield return new WaitForSecondsRealtime(.2f);
        }
    }

    Coroutine timerRoutine;

    private void EndThought(float timeOut) 
    { 
        // thoughtUI.SetActive(false);
        controlIndicatorUI.SetActive(true);

        // StopCoroutine(thoughtRoutine);
        timerRoutine = StartCoroutine(AnimateTimer(timeOut));
    }

    private void ExitMind()
    {
        distractionUI.SetActive(true);
    }

    private void CancelTimer()
    {
        // timerUI.color = Color.clear;
        StopCoroutine(timerRoutine);
    }

    private IEnumerator AnimateTimer(float timeOut)
    {
        // float startTime = Time.realtimeSinceStartup;
        // timerUI.color = Color.white;
        // Color newColor = timerUI.color;
        // while (Time.realtimeSinceStartup < startTime + timeOut)
        // {
        //     newColor.a = (Time.realtimeSinceStartup - startTime) / timeOut;
        //     timerUI.color = newColor;
        //     yield return null;
        // }
        
        // Wait for a while before starting to flicker
        float waitRatio = .3f; // TODO implement that in game engine, the time for the timeout is the experimental max time, plus a certain margin that's a percentage of that time, and use that time here
        int stops = 4;
        float stopRatio = .1f;
        
        float visibleRatio = 1f - waitRatio;
        
        yield return new WaitForSecondsRealtime((visibleRatio * timeOut));
        
        float stopTime = timeOut * waitRatio * stopRatio / stops;
        float visibleTimeTotal = timeOut * waitRatio * (1 - stopRatio);

        for (int i = stops; i > 0; i--)
        {
            spotLight.SetActive(false);
            yield return new WaitForSecondsRealtime(stopTime);
            spotLight.SetActive(true);
            float goTime = (Mathf.Pow(2, i) - Mathf.Pow(2, i - 1)) / Mathf.Pow(2, stops) * visibleTimeTotal;
            yield return new WaitForSecondsRealtime(goTime);
        }
        
        spotLight.SetActive(false);
    }
    
    //-----------------------------------------------------
    
    private void SubscribeToEvents()
    {
        GameEngine.SettingUpMindStartedEvent += SettingUpMind;
        GameEngine.ThinkingOfPropertyStartedEvent += ThinkingOfProperty;
        GameEngine.ShowingPropertyStartedEvent += ShowingProperty;
        GameEngine.EvaluatingInputStartedEvent += EvaluatingInput;
        GameEngine.TimedOutStartedEvent += TimedOut;
        GameEngine.EndingEncounterStartedEvent += EndingEncounter;
    }

    private void UnsubscribeFromEvents()
    {
        GameEngine.SettingUpMindStartedEvent -= SettingUpMind;
        GameEngine.ThinkingOfPropertyStartedEvent -= ThinkingOfProperty;
        GameEngine.ShowingPropertyStartedEvent -= ShowingProperty;
        GameEngine.EvaluatingInputStartedEvent -= EvaluatingInput;
        GameEngine.TimedOutStartedEvent -= TimedOut;
        GameEngine.EndingEncounterStartedEvent -= EndingEncounter;
    }
    
    protected virtual void SettingUpMind()
    {
        StartMind(GameEngine.MindStartTime);
    }
    
    protected virtual void ThinkingOfProperty(bool encounterOver)
    {
        if (encounterOver)
            Idle(true);
        else
        {
            spotLight.SetActive(false);
            StartThought(GameEngine.PullingTime);
        }
    }

    protected virtual void ShowingProperty(Action<InputHandler.InputState> callback)
    {
        EndThought(GameEngine.EnemyTimeOut);
        // spotLight.SetActive(true);
    }

    protected virtual void EvaluatingInput()
    {
        spotLight.SetActive(false);
        CancelTimer();
    }

    protected virtual void TimedOut()
    {
        // spotLight.SetActive(false);
    }

    protected virtual void EndingEncounter()
    {
        ExitMind();
    }
}
