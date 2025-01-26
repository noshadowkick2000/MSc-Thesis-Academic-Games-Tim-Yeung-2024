using System.Collections;
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
        GameEngine.ObjectDelayStartedEvent += SettingUpMind;
        GameEngine.StartingBreakStartedEvent += SettingUpMind;
        GameEngine.EndingEncounterStartedEvent += EndingEncounter;
        GameEngine.LostEncounterStartedEvent += LostEncounter;
        GameEngine.EndingBreakStartedEvent += EndingEncounter;
    }

    private void UnsubscribeFromEvents()
    {
        GameEngine.ObjectDelayStartedEvent += SettingUpMind;
        GameEngine.StartingBreakStartedEvent -= SettingUpMind;
        GameEngine.EndingEncounterStartedEvent -= EndingEncounter;
        GameEngine.LostEncounterStartedEvent -= LostEncounter;
        GameEngine.EndingBreakStartedEvent += EndingEncounter;
    }

    // private void ObjectSpawned(Transform property)
    // {
    //     dof.focusDistance.Override(property.position.z);
    // }

    private void SettingUpMind()
    {
        dof.focalLength.Override(35f);
    }

    private void EndingEncounter()
    {
        dof.focalLength.Override(26);
    }

    private void LostEncounter()
    {
        StartCoroutine(ShockWave());
    }

    private IEnumerator ShockWave()
    {
        float duration = .5f;
        float startTime = Time.time;
        float x = 0;
        float saturationStart = ca.saturation.value;
        float saturationGoal = saturationStart - 20f;

        while (x < 1)
        {
            x = (Time.time - startTime) / duration;

            ld.intensity.Override((1-x) * Mathf.Sin(x * 6 * Mathf.PI));
            ca.saturation.Override(x * saturationGoal + (1-x) * saturationStart);
            
            yield return null;
        }

        ld.intensity.Override(0);
        ca.saturation.Override(saturationGoal);
    }

    private void Init()
    {
        EndingEncounter();
        ca.saturation.Override(0);
        ld.intensity.Override(0);
    }
}
