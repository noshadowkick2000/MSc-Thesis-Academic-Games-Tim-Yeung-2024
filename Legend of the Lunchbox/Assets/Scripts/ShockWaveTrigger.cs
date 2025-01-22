using System;
using System.Collections;
using System.Collections.Generic;
using Assets;
using UnityEngine;

public class ShockWaveTrigger : MonoBehaviour
{
    private void Awake()
    {
        gameObject.SetActive(false);
        GameEngine.LostEncounterStartedEvent += LostEncounter;
        GameEngine.OnRailStartedEvent += OnRail;
    }

    private void OnDestroy()
    {
        GameEngine.LostEncounterStartedEvent -= LostEncounter;
        GameEngine.OnRailStartedEvent -= OnRail;
    }

    private void LostEncounter()
    {
        gameObject.SetActive(true);
    }

    private void OnRail()
    {
        gameObject.SetActive(false);
    }
}
