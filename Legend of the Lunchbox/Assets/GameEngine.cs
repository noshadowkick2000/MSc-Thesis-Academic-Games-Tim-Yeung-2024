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
      STARTINGENCOUNTER,
      SHOWINGENEMY,
      SHOWINGBAG,
      PULLINGOOBJECT,
      EVALUATINGINPUT,
      TIMEDOUT,
      DAMAGINGPLAYER,
      DAMAGINGENEMY,
      ENDINGENCOUNTER
    }

    private GameState state = GameState.CUTSCENE;

    [Header("Experimental Variables")]
    [SerializeField] private float minimumWalkTime = 2.0f;
    [SerializeField] private float maximumWalkTime = 8.0f;
    [SerializeField] private float encounterStartTime = 1.0f;
    [SerializeField] private float enemyShowTime = 2.0f;
    [SerializeField] private float bagStartTime = 1.0f;
    [SerializeField] private float pullingTime = 0.5f;
    [SerializeField] private float enemyTimeOut = 4.0f;

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
        case GameState.SHOWINGBAG:
          ShowingBag();
          break;
        case GameState.PULLINGOOBJECT:
          PullObject();
          break;
        case GameState.EVALUATINGINPUT:
          EvaluatingInput();
          break;
        case GameState.TIMEDOUT:
          TimedOut();
          break;
        case GameState.DAMAGINGPLAYER:
          break;
        case GameState.DAMAGINGENEMY:
          break;
        case GameState.ENDINGENCOUNTER:
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

    private Encounter curEncounter;
    private Transform obj;

    private void StartEncounter()
    {
      //Have the trial manager give an encounter which gives an object to transition to.
      curEncounter = trialHandler.GetEncounter();
      obj = trialHandler.ActivateObject(curEncounter.GetEnemyId());
      cameraController.ShowObject(obj, encounterStartTime);
      
      StartCoroutine(Timer(encounterStartTime, GameState.SHOWINGENEMY));
      //cameraController.ShowObject()
    }

    private void ShowingEnemy()
    {
      StartCoroutine(Timer(enemyShowTime, GameState.SHOWINGBAG));
    }

    private void ShowingBag()
    {
      cameraController.ShowObject(playerController.GetBag(), bagStartTime);
      StartCoroutine(Timer(bagStartTime, GameState.PULLINGOOBJECT));
    }

    bool acceptingInput = false;
    Coroutine timerRoutine;
    private void PullObject()
    {
      acceptingInput = true;

      timerRoutine = StartCoroutine(Timer(enemyTimeOut, GameState.DAMAGINGPLAYER));
    }

    private void EvaluatingInput()
    {
      acceptingInput = false;

      bool correct = curEncounter.EvaluateInput(input == InputState.Using);
      if (correct) { StateChange(GameState.DAMAGINGENEMY); }
      else { StateChange(GameState.DAMAGINGPLAYER); }

      StopCoroutine(timerRoutine);
      timerRoutine = null;
    }

    private void TimedOut()
    {
      acceptingInput = false;
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
      if (Input.GetButtonDown("use")) { input = InputState.Using; }
      if (Input.GetButtonDown("discard")) { input = InputState.Discarding; }
      Logger.Log($"Input: {input.ToString()}");

      if (acceptingInput && input != InputState.None)
      {
        StateChange(GameState.EVALUATINGINPUT);
      }
    }
  }
}