using System;
using System.Collections;
using System.Collections.Generic;
using Assets;
using UnityEngine;

public class SoundEngine : MonoBehaviour
{
    [SerializeField] private GameObject audioHolder;
    [SerializeField] private AudioSource[] ambientPlayers;
    [SerializeField] private AudioSource[] runPlayers;
    [SerializeField] private AudioSource[] trialAmbientPlayers;
    [SerializeField] private AudioClip enemyAppear;
    [SerializeField] private AudioClip enterMind;
    [SerializeField] private AudioClip spotLight;
    [SerializeField] private AudioClip worldDeform; 
    [SerializeField] private AudioSource oneShotPlayer;

    private void Init()
    {
        foreach (var ambientPlayer in ambientPlayers)
        {
            ambientPlayer.Play();
        }
        foreach (var runPlayer in runPlayers)
        {
            runPlayer.Play();
            runPlayer.Pause();
        }
        foreach (var trialAmbientPlayer in trialAmbientPlayers)
        {
            trialAmbientPlayer.Play();
            trialAmbientPlayer.Pause();
        }
    }
    
    private void Awake()
    {
        Init();
        
        SubscribeToEvents();
    }

    private void OnDestroy()
    {
        UnSubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
        GameEngine.OnRailStartedEvent += OnRail;
        GameEngine.StartingEncounterStartedEvent += StartingEncounter;
        GameEngine.SettingUpMindStartedEvent += SettingUpMind;
        GameEngine.ShowingPropertyStartedEvent += ShowingProperty;
        GameEngine.LostEncounterStartedEvent += WonEncounter;
        GameEngine.LostEncounterStartedEvent += LostEncounter;
        // GameEngine.EndingEncounterStartedEvent += EndingEncounter;
        GameEngine.EvaluatingEncounterStartedEvent += EvaluatingEncounter;
    }
  
    private void UnSubscribeToEvents()
    {
        GameEngine.OnRailStartedEvent -= OnRail;
        GameEngine.StartingEncounterStartedEvent -= StartingEncounter;
        GameEngine.SettingUpMindStartedEvent -= SettingUpMind;
        GameEngine.ShowingPropertyStartedEvent -= ShowingProperty;
        GameEngine.LostEncounterStartedEvent -= WonEncounter;
        GameEngine.LostEncounterStartedEvent -= LostEncounter;
        // GameEngine.EndingEncounterStartedEvent -= EndingEncounter;
        GameEngine.EvaluatingEncounterStartedEvent -= EvaluatingEncounter;
    }

    protected virtual void OnRail()
    {
        foreach (var runPlayer in runPlayers)
        {
            runPlayer.UnPause();
        }
    }

    protected virtual void StartingEncounter()
    {
        foreach (var runPlayer in runPlayers)
        {
            runPlayer.Pause();
        }
        
        oneShotPlayer.PlayOneShot(enemyAppear);
    }

    protected virtual void SettingUpMind()
    {
        foreach (var ambientPlayer in ambientPlayers)
        {
            ambientPlayer.Pause();
        }

        foreach (var trialAmbientPlayer in trialAmbientPlayers)
        {
            trialAmbientPlayer.UnPause();
        }
        
        oneShotPlayer.PlayOneShot(enterMind);
    }

    protected virtual void ThinkingOfProperty(bool encounterOver)
    {
        // if (!encounterOver) 
        //     oneShotPlayer.PlayOneShot(spotLight);
    }
    
    protected virtual void ShowingProperty(Action<InputHandler.InputState> callback)
    {
        // oneShotPlayer.PlayOneShot(enemyAppear);
    }

    protected virtual void EvaluatingEncounter()
    {
        foreach (var ambientPlayer in ambientPlayers)
        {
            ambientPlayer.UnPause();
        }
        
        foreach (var trialAmbientPlayer in trialAmbientPlayers)
        {
            trialAmbientPlayer.Pause();
        }
    }

    protected virtual void WonEncounter()
    {
        oneShotPlayer.PlayOneShot(enterMind);
    }

    protected virtual void LostEncounter()
    {
        oneShotPlayer.PlayOneShot(worldDeform);
    }
}
