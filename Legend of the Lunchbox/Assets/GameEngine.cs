using System.Collections;
using System.Collections.Generic;
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
      DAMAGINGPLAYER,
      DAMAGINGENEMY,
      ENDINGENCOUNTER
    }

    private GameState state = GameState.CUTSCENE;

    [Header("Experimental Variables")]
    [SerializeField] private float minimumWalkTime = 2.0f;
    [SerializeField] private float maximumWalkTime = 8.0f;
    [SerializeField] private float encounterStartTime = 2.0f;

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

    private void Init()
    {
      LogFolderInDocs = logFolderInDocs;
      PathToTrials= pathToTrials;
      PropertiesAndObjects = propertiesAndObjects;
      LevelId = levelId;

      console = FindObjectOfType<UIConsole>();
      cameraController = FindObjectOfType<CameraController>();
      trialHandler = GetComponent<TrialHandler>();
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
          break;
        case GameState.SHOWINGBAG:
          break;
        case GameState.PULLINGOOBJECT:
          break;
        case GameState.EVALUATINGINPUT:
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

    private void StartEncounter()
    {
      //Have the trial manager give an encounter which gives an object to transition to.
      Encounter curEncounter = trialHandler.GetEncounter();
      Transform obj = trialHandler.ActivateObject(curEncounter.GetEnemyId());
      cameraController.ShowObject(obj, encounterStartTime);
      
      StartCoroutine(Timer(encounterStartTime, GameState.SHOWINGENEMY));
      //cameraController.ShowObject()
    }
  }
}