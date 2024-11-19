using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace Assets
{
  [RequireComponent(typeof(TrialHandler))]
  public class GameEngine : MonoBehaviour
  {
    [Header("Experimental Variables")]
    // [SerializeField] private float minimumWalkTime = 2.0f;
    // [SerializeField] private float maximumWalkTime = 8.0f;
    // [SerializeField] private float encounterStartTime = 0.5f;
    // [SerializeField] private float enemyShowTime = 3.0f;
    // [SerializeField] private float mindStartTime = 1.0f;
    // [SerializeField] private float pullingTime = 0.5f;
    // [SerializeField] private float enemyTimeOut = 4.0f;
    // [SerializeField] private float feedbackTime = 2.0f;
    // [SerializeField] private float encounterStopTime = 2f;
    // [SerializeField] private float playerResetTime = 2f;
    
    // TODO load these in in a separate config loader from xml or sth
    private static float encounterStartTime = 0.5f;
    public static float EncounterStartTime => encounterStartTime;
    private static float enemyShowTime = 2f;
    public static float EnemyShowTime => enemyShowTime;
    private static float mindStartTime = .5f;
    public static float MindStartTime => mindStartTime;
    private static float pullingTime = 2f;
    public static float PullingTime => pullingTime;
    private static float enemyTimeOut = 4.0f;
    public static float EnemyTimeOut => enemyTimeOut;
    private static float feedbackTime = 2.0f;
    public static float FeedbackTime => feedbackTime;
    private static float encounterStopTime = 5f;
    public static float EncounterStopTime => encounterStopTime;
    private static float playerResetTime = 2f;
    public static float playerReset => playerResetTime;
    private static float railDuration;
    public static float RailDuration => railDuration;
    

    [Header("Assets")]
    [SerializeField] private GameObject[] propertiesAndObjects;
    public static GameObject[] PropertiesAndObjects;

    [Header("Paths")]
    [SerializeField] private string logFolderInDocs = "LotL";
    public static string LogFolderInDocs;
    [SerializeField] private string pathToTrials;
    public static string PathToTrials;

    // Other
    private int levelId = 0;
    public static int LevelId;

    // Connected components
    private TrialHandler trialHandler = null;
    
    private int playerHealth = 4;
    private int friendHealth = 0;

    private void DamagePlayer()
    {
      if (friendHealth > 0)
      {
        friendHealth--;
      }
      else
      {
        playerHealth--;
      }
    }

    private bool PlayerIsDead()
    {
      return playerHealth + friendHealth <= 0;
    }

    private void Awake()
    {
      LogFolderInDocs = logFolderInDocs;
      PathToTrials= pathToTrials;
      PropertiesAndObjects = propertiesAndObjects;
      LevelId = levelId;
      
      trialHandler = GetComponent<TrialHandler>();
    }

    private void Start()
    {
      Invoke(nameof(OnRail), 2f);
    }

    private void OnDestroy()
    {
      Logger.OnDestroy();
    }

    public delegate void StateChangeEvent();
    // public delegate void StateChangeEventTimed(float duration);
    public delegate void StateChangeEventBooled(bool boolean);
    // public delegate void StateChangeEventTimedBooled(float duration, bool boolean);
    // public delegate void StateChangeEventTimedCallback(float duration, Action<InputHandler.InputState> callback);
    public delegate void StateChangeEventCallback(Action<InputHandler.InputState> callback);

    public static event StateChangeEvent CutSceneStartedEvent;
    public static event StateChangeEvent OnRailStartedEvent;
    public static event StateChangeEvent StartingEncounterStartedEvent;
    public static event StateChangeEvent ShowingEnemyStartedEvent;
    public static event StateChangeEvent SettingUpMindStartedEvent;
    public static event StateChangeEventBooled ThinkingOfPropertyStartedEvent;
    public static event StateChangeEventCallback ShowingPropertyStartedEvent;
    public static event StateChangeEvent EvaluatingInputStartedEvent;
    public static event StateChangeEvent TimedOutStartedEvent;
    public static event StateChangeEvent AnswerWrongStartedEvent;
    public static event StateChangeEvent AnswerCorrectStartedEvent;
    public static event StateChangeEvent EvaluatingEncounterStartedEvent;
    public static event StateChangeEvent WonEncounterStartedEvent;
    public static event StateChangeEvent LostEncounterStartedEvent;
    public static event StateChangeEvent EndingEncounterStartedEvent;

    private IEnumerator Timer(float duration, Action nextState)
    {
      yield return new WaitForSecondsRealtime(duration);

      nextState();
    }

    private void CutScene()
    {
      CutSceneStartedEvent?.Invoke();
      
      // TODO remove
      SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
    }

    private void OnRail()
    { 
      railDuration = trialHandler.GetCurrentWaitTime();
      
      OnRailStartedEvent?.Invoke();
      
      StartCoroutine(Timer(railDuration, StartingEncounter));
    }

    private void StartingEncounter()
    {
      StartingEncounterStartedEvent?.Invoke();

      StartCoroutine(Timer(encounterStartTime, ShowingEnemy));
    }

    private void ShowingEnemy()
    {
      ShowingEnemyStartedEvent?.Invoke();
      
      StartCoroutine(Timer(enemyShowTime, SettingUpMind));
    }

    private void SettingUpMind()
    {
      SettingUpMindStartedEvent?.Invoke();
      
      StartCoroutine(Timer(mindStartTime, ThinkingOfProperty));
    }
    
    private void ThinkingOfProperty()
    {
      ThinkingOfPropertyStartedEvent?.Invoke(trialHandler.EncounterOver);
      
      if (trialHandler.EncounterOver)
        EvaluatingEncounter();
      else 
        StartCoroutine(Timer(pullingTime, ShowingProperty));
    }
    
    Coroutine timerRoutine;
    private void ShowingProperty()
    {
      ShowingPropertyStartedEvent?.Invoke(InputAvailable);
      
      timerRoutine = StartCoroutine(Timer(enemyTimeOut, TimedOut));
    }
    
    private void InputAvailable(InputHandler.InputState input)
    {
      EvaluatingInput(input);
    }

    private void EvaluatingInput(InputHandler.InputState input)
    {
      EvaluatingInputStartedEvent?.Invoke();
      
      StopCoroutine(timerRoutine);
      timerRoutine = null;

      if (trialHandler.EvaluateProperty(input))
        AnswerCorrect();
      else
        AnswerWrong();
    }

    private void TimedOut()
    {
      TimedOutStartedEvent?.Invoke();

      AnswerWrong();
    }

    private void AnswerWrong()
    {
      AnswerWrongStartedEvent?.Invoke();
      
      StartCoroutine(Timer(feedbackTime, ThinkingOfProperty));
    }

    private void AnswerCorrect()
    {
      AnswerCorrectStartedEvent?.Invoke();
      
      StartCoroutine(Timer(feedbackTime, ThinkingOfProperty));
    }

    private void EvaluatingEncounter()
    {
      EvaluatingEncounterStartedEvent?.Invoke();
      
      bool won = trialHandler.WonEncounter();

      if (won)
        WonEncounter();
      else 
        LostEncounter();
    }

    private void WonEncounter()
    {
      WonEncounterStartedEvent?.Invoke();
      
      StartCoroutine(Timer(encounterStopTime, EndingEncounter));
    }

    private void LostEncounter()
    {
      LostEncounterStartedEvent?.Invoke();

      DamagePlayer();
      
      StartCoroutine(Timer(encounterStopTime, EndingEncounter));
    }

    private void EndingEncounter()
    {
      EndingEncounterStartedEvent?.Invoke();

      if (PlayerIsDead())
      { 
        Logger.Log("Played died");
        StartCoroutine(Timer(playerResetTime, CutScene)); // TODO animations etc
      }
      else if (trialHandler.LevelOver)
        StartCoroutine(Timer(playerResetTime, CutScene));
      else
        StartCoroutine(Timer(playerResetTime, OnRail));
    }
  }
}