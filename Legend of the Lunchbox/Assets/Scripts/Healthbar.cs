using System;
using System.Collections;
using System.Collections.Generic;
using Assets;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    // [SerializeField] private GameObject healthBar;
    // [SerializeField] private float maxFill = -1f;
    // [SerializeField] private float minFill = 2.7f;
    [SerializeField] private Slider slider;
    
    private GameEngine gameEngine;
    // private float range;
    
    private void Awake()
    {
        // range = Mathf.Abs(maxFill - minFill);
        gameEngine = FindObjectOfType<GameEngine>();
        
        slider.value = CalculateFill();
        
        SubscribeToEvents();
    }

    private void OnDestroy()
    {
        UnsubscribeToEvents();
    }
    
    private void SubscribeToEvents()
    {
        GameEngine.WonBreakStartedEvent += WonBreak;
        GameEngine.LostEncounterStartedEvent += LostEncounter;
    }

    private void UnsubscribeToEvents()
    {
        GameEngine.WonBreakStartedEvent -= WonBreak;
        GameEngine.LostEncounterStartedEvent -= LostEncounter;
    }
    
    private float CalculateFill()
    {
        return ((float) gameEngine.TotalHealth / (float) gameEngine.MaxHealth);
    }

    protected virtual void WonBreak()
    {
        StartCoroutine(AnimateBar(0, GameEngine.WonBreakTime));
    }

    protected virtual void LostEncounter()
    {
        StartCoroutine(AnimateBar(GameEngine.EncounterStopTime / 4, GameEngine.EncounterStopTime * .75f));
    }

    private IEnumerator AnimateBar(float delay, float duration)
    {
        yield return new WaitForSecondsRealtime(delay);
        
        float startTime = Time.realtimeSinceStartup;
        float x = 0;

        float start = slider.value;
        float goal = CalculateFill();
        
        while (x < 1)
        {
            float y = MathT.EasedT(x);
            slider.value = y * goal + (1-y) * start;
            x = (Time.realtimeSinceStartup - startTime) / duration;

            yield return null;
        }

        slider.value = goal;
    }
}
