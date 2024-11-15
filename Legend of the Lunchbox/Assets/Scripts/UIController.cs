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
    [SerializeField] private Image timerUI;
    
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

    private void StartThought() 
    { 
        //TODO spotlights searching around effect
        
        thoughtUI.SetActive(true);
        controlIndicatorUI.SetActive(false);

        thoughtRoutine = StartCoroutine(AnimateThought());
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
        thoughtUI.SetActive(false);
        controlIndicatorUI.SetActive(true);

        StopCoroutine(thoughtRoutine);
        timerRoutine = StartCoroutine(AnimateTimer(timeOut));
    }

    private void ExitMind()
    {
        distractionUI.SetActive(true);
    }

    private void CancelTimer()
    {
        timerUI.color = Color.clear;
        StopCoroutine(timerRoutine);
    }

    private IEnumerator AnimateTimer(float timeOut)
    {
        float startTime = Time.realtimeSinceStartup;
        timerUI.color = Color.white;
        Color newColor = timerUI.color;
        while (Time.realtimeSinceStartup < startTime + timeOut)
        {
            newColor.a = (Time.realtimeSinceStartup - startTime) / timeOut;
            timerUI.color = newColor;
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
    
    protected virtual void SettingUpMind(float duration)
    {
        StartMind(duration);
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
        spotLight.SetActive(true);
    }

    protected virtual void EvaluatingInput()
    {
        spotLight.SetActive(false);
        CancelTimer();
    }

    protected virtual void EndingEncounter(float duration)
    {
        ExitMind();
    }
}
