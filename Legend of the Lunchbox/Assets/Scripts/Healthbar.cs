using System;
using System.Collections;
using System.Collections.Generic;
using Assets;
using UnityEngine;

public class Healthbar : MonoBehaviour
{
    [SerializeField] private GameObject healthBar;
    [SerializeField] private float maxFill = -1f;
    [SerializeField] private float minFill = 2.7f;
    [SerializeField] private Liquid liquidScript;
    
    private GameEngine gameEngine;
    // private float range;
    
    private void Awake()
    {
        // range = Mathf.Abs(maxFill - minFill);
        gameEngine = FindObjectOfType<GameEngine>();
        
        liquidScript.fillAmount = CalculateFill();
        
        SubscribeToEvents();
    }

    private void OnDestroy()
    {
        UnsubscribeToEvents();
    }
    
    private void SubscribeToEvents()
    {
        GameEngine.SettingUpMindStartedEvent += SettingUpMind;
        GameEngine.BreakingBadStartedEvent += BreakingBad;
        GameEngine.WonBreakStartedEvent += WonBreak;
        GameEngine.EvaluatingEncounterStartedEvent += EvaluatingEncounter;
        GameEngine.LostEncounterStartedEvent += LostEncounter;
    }

    private void UnsubscribeToEvents()
    {
        GameEngine.SettingUpMindStartedEvent -= SettingUpMind;
        GameEngine.BreakingBadStartedEvent -= BreakingBad;
        GameEngine.WonBreakStartedEvent -= WonBreak;
        GameEngine.EvaluatingEncounterStartedEvent -= EvaluatingEncounter;
        GameEngine.LostEncounterStartedEvent -= LostEncounter;
    }
    
    private float CalculateFill()
    {
        float ratio = ((float) gameEngine.TotalHealth / (float) gameEngine.MaxHealth);
        return ratio * maxFill + (1 - ratio) * minFill;
    }

    protected virtual void SettingUpMind()
    {
        healthBar.SetActive(false);
    }

    protected virtual void BreakingBad()
    {
        SettingUpMind();
    }

    protected virtual void EvaluatingEncounter()
    {
        healthBar.SetActive(true);
    }

    protected virtual void WonBreak()
    {
        healthBar.SetActive(true);
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

        float start = liquidScript.fillAmount;
        float goal = CalculateFill();

        Quaternion startPos = healthBar.transform.rotation;
        
        while (x < 1)
        {
            float y = MathT.EasedT(x);
            liquidScript.fillAmount = y * goal + (1-y) * start;
            healthBar.transform.rotation = startPos * Quaternion.Euler(0, 0, Mathf.PerlinNoise1D(Time.realtimeSinceStartup));
            x = (Time.realtimeSinceStartup - startTime) / duration;

            yield return null;
        }
        
        healthBar.transform.rotation = startPos;
        liquidScript.fillAmount = goal;
    }
}
