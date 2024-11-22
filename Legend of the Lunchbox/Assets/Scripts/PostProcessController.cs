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
    private ColorAdjustments ca;
    private LensDistortion ld;
    
    private void Awake()
    {
        volumeProfile.TryGet(out dof);
        volumeProfile.TryGet(out ca);
        volumeProfile.TryGet(out ld);
        SubscribeToEvents();
        
        Init();
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
        
        Init();
    }

    private void SubscribeToEvents()
    {
        // TrialHandler.OnObjectSpawnedEvent += ObjectSpawned;
        GameEngine.ShowingEnemyStartedEvent += ShowingEnemy;
        GameEngine.EndingEncounterStartedEvent += EndingEncounter;
        GameEngine.LostEncounterStartedEvent += LostEncounter;
    }

    private void UnsubscribeFromEvents()
    {
        // TrialHandler.OnObjectSpawnedEvent -= ObjectSpawned;
        GameEngine.ShowingEnemyStartedEvent -= ShowingEnemy;
        GameEngine.EndingEncounterStartedEvent -= EndingEncounter;
        GameEngine.LostEncounterStartedEvent -= LostEncounter;
    }

    // protected virtual void ObjectSpawned(Transform property)
    // {
    //     dof.focusDistance.Override(property.position.z);
    // }

    protected virtual void ShowingEnemy()
    {
        dof.focalLength.Override(35f);
    }

    protected virtual void EndingEncounter()
    {
        dof.focalLength.Override(25f);
    }

    protected virtual void LostEncounter()
    {
        StartCoroutine(ShockWave());
    }

    private IEnumerator ShockWave()
    {
        float duration = .5f;
        float startTime = Time.realtimeSinceStartup;
        float x = 0;

        while (x < 1)
        {
            x = (Time.realtimeSinceStartup - startTime) / duration;

            ld.intensity.Override((1-x) * Mathf.Sin(x * 6 * Mathf.PI));
            
            yield return null;
        }

        ld.intensity.Override(0);
        ca.saturation.Override(ca.saturation.value-20f);
        
    }

    private void Init()
    {
        EndingEncounter();
        ca.saturation.Override(0);
        ld.intensity.Override(0);
    }
}
