using System;
using System.Collections;
using System.Collections.Generic;
using Assets;
using UnityEngine;
using GameEngine = Assets.GameEngine;

public class LightingController : MonoBehaviour
{
    private void Awake()
    {
        SubscribeToEvents();
    }

    private void OnDestroy()
    {
        UnsubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
        GameEngine.ShowingEnemyInMindStartedEvent += ShowingEnemyInMind;
        GameEngine.EvaluatingEncounterStartedEvent += EvaluatingEncounter;
    }

    private void UnsubscribeToEvents()
    {
        
    }

    protected virtual void ShowingEnemyInMind()
    { 
        RenderSettings.ambientIntensity = 0f;
    }

    protected virtual void EvaluatingEncounter()
    {
        RenderSettings.ambientIntensity = 1.0f;
    }
}
