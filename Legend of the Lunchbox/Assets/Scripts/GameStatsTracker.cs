using System;
using System.Collections;
using System.Collections.Generic;
using Assets;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStatsTracker : MonoBehaviour
{
    private float startTime;
    private int currentStreak;
    private void Awake()
    {
        EncountersWon = 0;
        ComboCounter = 0;
        LastLevel = (SceneManager.GetActiveScene().buildIndex)-3;

        startTime = Time.realtimeSinceStartup;

        GameEngine.WonEncounterStartedEvent += WonEncounter;
        GameEngine.LostEncounterStartedEvent += LostEncounter;
        GameEngine.LevelOverStartedEvent += LevelOver;
    }

    private void OnDestroy()
    {
        GameEngine.WonEncounterStartedEvent -= WonEncounter;
        GameEngine.LostEncounterStartedEvent -= LostEncounter;
        GameEngine.LevelOverStartedEvent -= LevelOver;
    }

    private void LevelOver()
    {
        if (currentStreak > ComboCounter)
            ComboCounter = currentStreak;
        CompletionTime = Time.realtimeSinceStartup - startTime;
    }

    private void WonEncounter()
    {
        currentStreak++;
        EncountersWon++;
    }

    private void LostEncounter()
    {
        if (currentStreak > ComboCounter)
            ComboCounter = currentStreak;
        currentStreak = 0;
    }

    public static int EncountersWon;
    public static int ComboCounter;
    public static float CompletionTime;
    public static int LastLevel;
}
