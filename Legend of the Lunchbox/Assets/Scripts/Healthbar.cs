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
        healthBar.SetActive(true);
    }

    protected virtual void LostEncounter()
    {
        liquidScript.fillAmount = CalculateFill();
    }
}
