using System;
using System.Collections;
using System.Collections.Generic;
using Assets;
using UnityEngine;

public class PropertyTimer : MonoBehaviour
{
    [SerializeField] private float flickerWaitRatio = .8f;
    
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
        yield return new WaitForSecondsRealtime((flickerWaitRatio * propertyDuration));

        float flickerHeight = (1 - flickerWaitRatio) * propertyDuration;
        float startTimer = Time.realtimeSinceStartup;
        while (true)
        {
            var x = (Time.realtimeSinceStartup - startTimer) / flickerHeight;
            var y = (-flickerHeight * Mathf.Pow(x, 2) + 1) * .1f;
            yield return new WaitForSecondsRealtime(currentProperty.gameObject.activeSelf ? y : y / 2);
            currentProperty.gameObject.SetActive(!currentProperty.gameObject.activeSelf);
        }
    }

    protected virtual void ShowingProperty(float duration, Action<InputHandler.InputState> callback)
    {
        propertyDuration = duration;
    }

    protected virtual void OnPropertySpawned(Transform property)
    {
        currentProperty = property;
        flickerRoutine = StartCoroutine(Flicker());
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
