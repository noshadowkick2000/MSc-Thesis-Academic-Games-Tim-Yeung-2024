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
        GameEngine.EndingEncounterStartedEvent += EndingEncounter;
    }

    private void OnDestroy()
    {
        GameEngine.LostEncounterStartedEvent -= LostEncounter;
        GameEngine.EndingEncounterStartedEvent -= EndingEncounter;
    }

    private void LostEncounter()
    {
        gameObject.SetActive(true);
    }

    private void EndingEncounter()
    {
        gameObject.SetActive(false);
    }
}
