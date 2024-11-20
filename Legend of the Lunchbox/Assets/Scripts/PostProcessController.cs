using System;
using System.Collections;
using System.Collections.Generic;
using Assets;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessController : MonoBehaviour
{
    [SerializeField] private VolumeProfile volumeProfile;
    private DepthOfField dof;
    
    private void Awake()
    {
        volumeProfile.TryGet(out dof);
        SubscribeToEvents();
        
        EndingEncounter();
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    private void SubscribeToEvents()
    {
        TrialHandler.OnObjectSpawnedEvent += ObjectSpawned;
        GameEngine.EndingEncounterStartedEvent += EndingEncounter;
    }

    private void UnsubscribeFromEvents()
    {
        TrialHandler.OnObjectSpawnedEvent -= ObjectSpawned;
        GameEngine.EndingEncounterStartedEvent -= EndingEncounter;
    }

    private void ObjectSpawned(Transform property)
    {
        dof.focusDistance.Override(property.position.z);
        dof.focalLength.Override(35f);
    }

    private void EndingEncounter()
    {
        dof.focalLength.Override(25f);
    }
}
