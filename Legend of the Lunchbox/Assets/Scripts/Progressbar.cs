using System;
using System.Collections;
using System.Collections.Generic;
using Assets;
using UnityEngine;
using UnityEngine.UI;

public class Progressbar : MonoBehaviour
{
    [SerializeField] private Slider progressSlider;
    [SerializeField] private Transform bGoal;

    private Vector3 baseSize = .5f * Vector3.one;

    private float total;
    private float progress;
    
    private void Start()
    {
        total = FindObjectOfType<TrialHandler>().GetTotalWaitTime();
        SubscribeToEvents();
        
        bGoal.localScale = baseSize;
    }

    private void OnDestroy()
    {
        UnsubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
        GameEngine.OnRailStartedEvent += OnRail;
    }

    private void UnsubscribeToEvents()
    {
        GameEngine.OnRailStartedEvent -= OnRail;
    }

    protected virtual void OnRail()
    {
        StartCoroutine(MoveSlider());
    }

    private IEnumerator MoveSlider()
    {
        float startTime = Time.realtimeSinceStartup;
        float x = 0;
        float startProgress = progress;
        float addedProgress = GameEngine.RailDuration / total;
            
        while (x < 1)
        {
            x = (Time.realtimeSinceStartup - startTime) / GameEngine.RailDuration;
            float y = x * addedProgress;

            progress = startProgress + y;
            progressSlider.value = progress;
            bGoal.localScale = progress * baseSize + baseSize;
            
            yield return null;
        }

        progress = startProgress + addedProgress;
    }
}
