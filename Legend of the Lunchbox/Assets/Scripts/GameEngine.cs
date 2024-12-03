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
    [field: Header("Experimental Variables")]
    public static float EncounterStartTime { get; } = 0.5f;

    public static float EnemyShowTime { get; } = 3f;

    public static float MindStartTime { get; } = .5f;

    public static float EnemyMindShowTime { get; } = 2f;

    public static float MindPropertyTransitionTime { get; } = 3f;

    public static float PullingTime { get; } = 2f;

    public static float EnemyTimeOut { get; } = 4.0f;

    public static float FeedbackTime { get; } = 2.0f;

    public static float EncounterStopTime { get; } = 5f;

    public static float PlayerReset { get; } = 2f;
    
    public static float BreakTimeOut { get; } = 8f;
    public static float WonBreakTime { get; } = 2f;

    public static float LevelOverTime { get; } = 3f;

    public static float RailDuration { get; private set; }


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
    public int TotalHealth => playerHealth + friendHealth;
    public int MaxHealth = 6;
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
    public delegate void StateChangeEventTyped(TrialHandler.PropertyType propertyType);
    public delegate void StateChangeEventInput(InputHandler.InputState input);
    public delegate void StateChangeEventCallback(Action<InputHandler.InputState> callback);

    public static event StateChangeEvent CutSceneStartedEvent;
    public static event StateChangeEvent OnRailStartedEvent;
    public static event StateChangeEvent StartingEncounterStartedEvent;
    public static event StateChangeEvent StartingBreakStartedEvent;
    public static event StateChangeEventCallback BreakingBadStartedEvent;
    public static event StateChangeEvent ShowingEnemyStartedEvent;
    public static event StateChangeEvent SettingUpMindStartedEvent;
    public static event StateChangeEvent ShowingEnemyInMindStartedEvent;
    public static event StateChangeEventTyped MovingToPropertyStartedEvent;
    public static event StateChangeEvent ThinkingOfPropertyStartedEvent;
    public static event StateChangeEventCallback ShowingPropertyStartedEvent;
    public static event StateChangeEventInput EvaluatingInputStartedEvent;
    public static event StateChangeEvent TimedOutStartedEvent;
    public static event StateChangeEvent AnswerWrongStartedEvent;
    public static event StateChangeEvent AnswerCorrectStartedEvent;
    public static event StateChangeEvent MovingToEnemyStartedEvent;
    public static event StateChangeEvent EvaluatingEncounterStartedEvent;
    public static event StateChangeEvent EvaluatingBreakStartedEvent;
    public static event StateChangeEvent WonBreakStartedEvent;
    public static event StateChangeEvent WonEncounterStartedEvent;
    public static event StateChangeEvent LostEncounterStartedEvent;
    public static event StateChangeEvent EndingEncounterStartedEvent;
    public static event StateChangeEvent EndingBreakStartedEvent;
    public static event StateChangeEvent LevelOverStartedEvent;

    private IEnumerator Timer(float duration, Action nextState)
    {
      yield return new WaitForSecondsRealtime(duration);

      nextState();
    }

    private void CutScene()
    {
      CutSceneStartedEvent?.Invoke();
      
      // TODO remove
    }

    private void OnRail()
    { 
      RailDuration = trialHandler.GetCurrentWaitTime();
      
      OnRailStartedEvent?.Invoke();
      
      if (trialHandler.EncounterIsObject())
        StartCoroutine(Timer(RailDuration, StartingEncounter));
      else
        StartCoroutine(Timer(RailDuration, StartingBreak));
    }

    private void StartingBreak()
    {
      StartingBreakStartedEvent?.Invoke();
      
      StartCoroutine(Timer(EncounterStartTime, BreakingBad));
    }

    private Coroutine breakRoutine; 
    private void BreakingBad()
    {
      BreakingBadStartedEvent?.Invoke(EvaluatingBreak);

      breakRoutine = StartCoroutine(Timer(BreakTimeOut, EndingBreak));
    }

    private void EvaluatingBreak(InputHandler.InputState input)
    {
      StopCoroutine(breakRoutine);
      
      EvaluatingBreakStartedEvent?.Invoke();
      
      if (input == InputHandler.InputState.USING)
        WonBreak();
      else
        EndingBreak();
    }

    private void WonBreak()
    {
      WonBreakStartedEvent?.Invoke();

      StartCoroutine(Timer(WonBreakTime, EndingBreak));
    }

    private void StartingEncounter()
    {
      StartingEncounterStartedEvent?.Invoke();

      StartCoroutine(Timer(EncounterStartTime, ShowingEnemy));
    }

    private void ShowingEnemy()
    {
      ShowingEnemyStartedEvent?.Invoke();
      
      StartCoroutine(Timer(EnemyShowTime, SettingUpMind));
    }

    private void SettingUpMind()
    {
      SettingUpMindStartedEvent?.Invoke();
      
      StartCoroutine(Timer(MindStartTime, ShowingEnemyInMind));
    }

    private void ShowingEnemyInMind()
    {
      if (trialHandler.EncounterOver)
        EvaluatingEncounter();
      else
      {
        ShowingEnemyInMindStartedEvent?.Invoke();
        StartCoroutine(Timer(EnemyMindShowTime, MovingToProperty));
      }
    }

    private void MovingToProperty()
    {
      MovingToPropertyStartedEvent?.Invoke(trialHandler.GetCurrentEncounterType());

      StartCoroutine(Timer(MindPropertyTransitionTime, ThinkingOfProperty));
    }
    
    private void ThinkingOfProperty()
    {
      ThinkingOfPropertyStartedEvent?.Invoke();
      
      StartCoroutine(Timer(PullingTime, ShowingProperty));
    }
    
    Coroutine timerRoutine;
    private void ShowingProperty()
    {
      ShowingPropertyStartedEvent?.Invoke(EvaluatingInput);
      
      timerRoutine = StartCoroutine(Timer(EnemyTimeOut, TimedOut));
    }
    
    // private void InputAvailable(InputHandler.InputState input)
    // {
    //   EvaluatingInput(input);
    // }

    private void EvaluatingInput(InputHandler.InputState input)
    {
      EvaluatingInputStartedEvent?.Invoke(input);
      
      StopCoroutine(timerRoutine);
      timerRoutine = null;

      MovingToEnemy(trialHandler.EvaluateProperty(input));
    }

    private void TimedOut()
    {
      TimedOutStartedEvent?.Invoke();

      MovingToEnemy(false);
    }
    
    private void MovingToEnemy(bool answerCorrect)
    {
      MovingToEnemyStartedEvent?.Invoke();
      
      if (answerCorrect)
        StartCoroutine(Timer(MindPropertyTransitionTime, AnswerCorrect));
      else
        StartCoroutine(Timer(MindPropertyTransitionTime, AnswerWrong));
    }
    
    private void AnswerWrong()
    {
      AnswerWrongStartedEvent?.Invoke();
      
      StartCoroutine(Timer(FeedbackTime, ShowingEnemyInMind));
    }

    private void AnswerCorrect()
    {
      AnswerCorrectStartedEvent?.Invoke();
      
      StartCoroutine(Timer(FeedbackTime, ShowingEnemyInMind));
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
      
      StartCoroutine(Timer(EncounterStopTime, EndingEncounter));
    }

    private void LostEncounter()
    {
      DamagePlayer();
      
      LostEncounterStartedEvent?.Invoke();
      
      StartCoroutine(Timer(EncounterStopTime, EndingEncounter));
    }

    private void EndingEncounter()
    {
      EndingEncounterStartedEvent?.Invoke();

      if (PlayerIsDead())
      { 
        Logger.Log("Played died");
        StartCoroutine(Timer(PlayerReset, CutScene)); // TODO animations etc
      }
      else if (trialHandler.LevelOver)
        StartCoroutine(Timer(PlayerReset, LevelOver));
      else
        StartCoroutine(Timer(PlayerReset, OnRail));
    }

    private void EndingBreak()
    {
      EndingBreakStartedEvent?.Invoke();
      
      StartCoroutine(Timer(PlayerReset, OnRail));
    }

    private void LevelOver()
    {
      LevelOverStartedEvent?.Invoke();
    }

    private void LoadNext()
    {
      SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
    }
  }
}