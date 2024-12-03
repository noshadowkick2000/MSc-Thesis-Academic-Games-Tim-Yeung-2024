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
        GameEngine.EndingEncounterStartedEvent += EndingEncounter;
        GameEngine.LostEncounterStartedEvent += LostEncounter;
    }

    private void UnsubscribeToEvents()
    {
        GameEngine.SettingUpMindStartedEvent -= SettingUpMind;
        GameEngine.EndingEncounterStartedEvent -= EndingEncounter;
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

    protected virtual void EndingEncounter()
    {
    }

    protected virtual void LostEncounter()
    {
        healthBar.SetActive(true);

        StartCoroutine(AnimateBar());
    }

    private IEnumerator AnimateBar()
    {
        yield return new WaitForSecondsRealtime(GameEngine.PlayerReset / 4);
        
        float startTime = Time.realtimeSinceStartup;
        float x = 0;

        float start = liquidScript.fillAmount;
        float goal = CalculateFill();

        Quaternion startPos = healthBar.transform.rotation;
        
        while (x < 1)
        {
            liquidScript.fillAmount = x * goal + (1-x) * start;
            healthBar.transform.rotation = startPos * Quaternion.Euler(0, 0, Mathf.PerlinNoise1D(Time.realtimeSinceStartup) * .1f);
            x = (Time.realtimeSinceStartup - startTime) / GameEngine.PlayerReset / 2;

            yield return null;
        }
        
        healthBar.transform.rotation = startPos;
        liquidScript.fillAmount = goal;
    }
}
