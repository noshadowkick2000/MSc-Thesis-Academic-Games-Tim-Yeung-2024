using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

namespace Assets
{
  [RequireComponent(typeof(TrialHandler))]
  public class GameEngine : MonoBehaviour
  {
    public enum GameState
    {
      CUTSCENE,
      ONRAIL,
      STARTINGENCOUNTER, //Camera starts panning to enemy
      SHOWINGENEMY, //Camera shows enemy
      SETTINGUPMIND, //Camera transitions to player and clouds pop up to frame the thinking
      THINKINGOFPROPERTY, //Thinking animation is shown
      SHOWINGPROPERTY, //Property is shown and player needs to confirm or reject
      EVALUATINGINPUT, 
      TIMEDOUT, 
      ANSWERWRONG,
      ANSWERCORRECT,
      ENDINGENCOUNTER
    }

    private GameState state = GameState.CUTSCENE;

    [Header("Experimental Variables")]
    [SerializeField] private float minimumWalkTime = 2.0f;
    [SerializeField] private float maximumWalkTime = 8.0f;
    [SerializeField] private float encounterStartTime = 0.5f;
    [SerializeField] private float enemyShowTime = 3.0f;
    [SerializeField] private float mindStartTime = 1.0f;
    [SerializeField] private float pullingTime = 0.5f;
    [SerializeField] private float enemyTimeOut = 4.0f;
    [SerializeField] private float feedbackTime = 2.0f;
    [SerializeField] private float encounterStopTime = 2f;

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
    private UIConsole console = null; 
    private CameraController cameraController = null;
    private TrialHandler trialHandler = null;
    PlayerController playerController = null;

    private void Init()
    {
      LogFolderInDocs = logFolderInDocs;
      PathToTrials= pathToTrials;
      PropertiesAndObjects = propertiesAndObjects;
      LevelId = levelId;

      console = FindObjectOfType<UIConsole>();
      cameraController = FindObjectOfType<CameraController>();
      trialHandler = GetComponent<TrialHandler>();
      playerController = FindObjectOfType<PlayerController>();
    }

    private void Awake()
    {
      Init();
      Logger.Awake(logFolderInDocs, console);
      trialHandler.Init();
      
      StateChange(GameState.ONRAIL);
    }

    private void OnDestroy()
    {
      Logger.OnDestroy();
    }


    public void StateChange(GameState newState)
    {
      state = newState;
      Logger.Log($"State changed to {state.ToString()}");

      switch (state)
      {
        case GameState.CUTSCENE:
          break;
        case GameState.ONRAIL:
          StartOnRail();
          break;
        case GameState.STARTINGENCOUNTER:
          StartEncounter();
          break;
        case GameState.SHOWINGENEMY:
          ShowingEnemy();
          break;
        case GameState.SETTINGUPMIND:
          SetUpMindFrame();
          break;
        case GameState.THINKINGOFPROPERTY:
          ThinkingOfProperty();
          break;
        case GameState.SHOWINGPROPERTY:
          ShowingProperty();
          break;
        case GameState.EVALUATINGINPUT:
          EvaluatingInput();
          break;
        case GameState.TIMEDOUT:
          TimedOut();
          break;
        case GameState.ANSWERWRONG:
          AnswerWrong();
          break;
        case GameState.ANSWERCORRECT:
          AnswerCorrect();
          break;
        case GameState.ENDINGENCOUNTER:
          EndingEncounter();
          break;
      }
    }

    private int playerHealth;
    private int friendHealth;
    public void DamagePlayer()
    {
      if (friendHealth > 0)
        friendHealth--;
      else
        playerHealth--;

      //TODO EVAL PLAYER DIE ETC
    }

    private IEnumerator Timer(float duration, GameState nextState)
    {
      yield return new WaitForSecondsRealtime(duration);

      StateChange(nextState);
    }

    private void StartOnRail()
    {     
      float duration = Random.Range(minimumWalkTime, maximumWalkTime);

      StartCoroutine(Timer(duration, GameState.STARTINGENCOUNTER));
    }

    private void StartEncounter()
    {
      trialHandler.StartEncounter();

      cameraController.SmoothToObject(LocationHolder.EnemyCameraLocation, encounterStartTime);

      StartCoroutine(Timer(encounterStartTime, GameState.SHOWINGENEMY));
    }

    private void ShowingEnemy()
    {
      Logger.Log($"Enemy: {trialHandler.GetCurrentEncounterId()}");

      playerController.Idle();
      
      StartCoroutine(Timer(enemyShowTime, GameState.SETTINGUPMIND));
    }

    private void SetUpMindFrame()
    {
      Logger.Log("Setting up mind frame");

      playerController.StartMind();

      cameraController.ImmediateToObject(LocationHolder.MindCameraLocation);
      
      StartCoroutine(Timer(mindStartTime, GameState.THINKINGOFPROPERTY));
    }

    bool acceptingInput = false;
    Coroutine timerRoutine;
    private void ThinkingOfProperty()
    {
      if (trialHandler.EncounterOver)
      {
        Logger.Log("Encounter over");

        playerController.Idle();

        StateChange(GameState.ENDINGENCOUNTER);
      }
      else
      {
        Logger.Log(trialHandler.GetCurrentPropertyInfo());

        playerController.StartThought();

        StartCoroutine(Timer(pullingTime, GameState.SHOWINGPROPERTY));
      }
    }

    private void ShowingProperty()
    {
      acceptingInput = true;

      playerController.EndThought();

      trialHandler.SpawnProperty().position = LocationHolder.PropertyLocation.position;

      timerRoutine = StartCoroutine(Timer(enemyTimeOut, GameState.TIMEDOUT));
    }

    private void EvaluatingInput()
    {
      acceptingInput = false;
      timerRoutine = null;
      StopCoroutine(timerRoutine);

      bool correct = trialHandler.EvaluateProperty(input == InputState.Using);

      Logger.Log($"Input correct: {correct}");

      if (correct) { StateChange(GameState.ANSWERCORRECT); }
      else { StateChange(GameState.ANSWERWRONG); }
    }

    private void TimedOut()
    {
      Logger.Log("No player input");

      acceptingInput = false;

      trialHandler.SkipProperty();

      StateChange(GameState.ANSWERWRONG);
    }

    private void AnswerWrong()
    {
      Logger.Log("Player gave wrong response"); 
      
      StartCoroutine(Timer(feedbackTime, GameState.THINKINGOFPROPERTY));
    }

    private void AnswerCorrect()
    {
      trialHandler.DamageEncounter();
      StartCoroutine(Timer(feedbackTime, GameState.THINKINGOFPROPERTY));
    }

    private void EndingEncounter()
    {
      bool won = trialHandler.KillEncounter();
      if (won) 
      {
        Logger.Log("Player defeated enemy");
        if (trialHandler.LevelOver)
          StartCoroutine(Timer(encounterStopTime, GameState.CUTSCENE));
        else
          StartCoroutine(Timer(encounterStopTime, GameState.ONRAIL));
      }
      else
      {
        Logger.Log("Player lost enemy");
        playerHealth--;
        if (playerHealth == 0) { Logger.Log("Played died"); }//TODO DIE
      }
    }

    private enum InputState
    {
      None,
      Using,
      Discarding
    }

    InputState input = InputState.None; 
    private void Update()
    {
      input = InputState.None;
      if (Input.GetButtonDown("Use")) { input = InputState.Using; }
      if (Input.GetButtonDown("Discard")) { input = InputState.Discarding; }
      if (input != InputState.None) { Logger.Log($"Input: {input.ToString()}"); }

      if (acceptingInput && input != InputState.None)
      {
        StateChange(GameState.EVALUATINGINPUT);
      }
    }
  }
}