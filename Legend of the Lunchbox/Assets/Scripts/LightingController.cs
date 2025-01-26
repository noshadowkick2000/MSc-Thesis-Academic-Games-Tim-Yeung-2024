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
        GameEngine.ObjectDelayStartedEvent += ObjectDelay;
        GameEngine.EvaluatingEncounterStartedEvent += EvaluatingEncounter;
    }

    private void UnsubscribeToEvents()
    {
        
    }

    private void ObjectDelay()
    { 
        RenderSettings.ambientIntensity = 0f;
    }

    private void EvaluatingEncounter()
    {
        RenderSettings.ambientIntensity = 1.0f;
    }
}
