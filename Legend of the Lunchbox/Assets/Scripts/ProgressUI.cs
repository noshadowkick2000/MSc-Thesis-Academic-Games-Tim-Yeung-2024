using System;
using Assets;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ProgressUI : MonoBehaviour
{
    [SerializeField] private Slider progressSlider;

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
        GameEngine.OnRailStartedEvent += OnRail;
        GameEngine.StartingEncounterStartedEvent += StartingEncounter;
    }

    private void UnsubscribeToEvents()
    {
        GameEngine.OnRailStartedEvent -= OnRail;
        GameEngine.StartingEncounterStartedEvent -= StartingEncounter;
    }

    private void OnRail()
    {
        
    }

    private void StartingEncounter()
    {
        
    }
}