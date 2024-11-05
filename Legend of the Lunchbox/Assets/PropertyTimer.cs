using System;
using System.Collections;
using System.Collections.Generic;
using Assets;
using UnityEngine;
using UnityEngine.Serialization;

public class PropertyTimer : MonoBehaviour
{
    [FormerlySerializedAs("flickerWaitRatio")] [SerializeField] private float waitRatio = .8f;
    [SerializeField] private int stops = 10;
    [SerializeField] private float stopRatio = .1f;
    
    private void Awake()
    {
        SubscribeToEvents();
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    private void SubscribeToEvents()
    {
        TrialHandler.OnPropertySpawnedEvent += OnPropertySpawned;
        GameEngine.ShowingPropertyStartedEvent += ShowingProperty;
        GameEngine.TimedOutStartedEvent += TimedOut;
        GameEngine.EvaluatingInputStartedEvent += EvaluatingInput;
    }

    private void UnsubscribeFromEvents()
    {
        TrialHandler.OnPropertySpawnedEvent -= OnPropertySpawned;
        GameEngine.ShowingPropertyStartedEvent -= ShowingProperty;
        GameEngine.TimedOutStartedEvent -= TimedOut;
        GameEngine.EvaluatingInputStartedEvent -= EvaluatingInput;
    }

    private Coroutine flickerRoutine;
    private Transform currentProperty;
    private float propertyDuration;

    private IEnumerator Flicker()
    {
        // Wait for a while before starting to flicker
        float visibleRatio = 1f - waitRatio;
        
        yield return new WaitForSecondsRealtime((visibleRatio * propertyDuration));
        
        float stopTime = propertyDuration * waitRatio * stopRatio / stops;
        float visibleTimeTotal = propertyDuration * waitRatio * (1 - stopRatio);

        for (int i = stops; i > 0; i--)
        {
            currentProperty.gameObject.SetActive(false);
            yield return new WaitForSecondsRealtime(stopTime);
            currentProperty.gameObject.SetActive(true);
            float goTime = (Mathf.Pow(2, i) - Mathf.Pow(2, i - 1)) / Mathf.Pow(2, stops) * visibleTimeTotal;
            yield return new WaitForSecondsRealtime(goTime);
        }
    }

    protected virtual void ShowingProperty(float duration, Action<InputHandler.InputState> callback)
    {
        propertyDuration = duration;
        flickerRoutine = StartCoroutine(Flicker());
    }

    protected virtual void OnPropertySpawned(Transform property)
    {
        currentProperty = property;
    }

    protected virtual void TimedOut()
    {
        StopCoroutine(flickerRoutine);
    }

    protected virtual void EvaluatingInput()
    {
        StopCoroutine(flickerRoutine);
    }
}
