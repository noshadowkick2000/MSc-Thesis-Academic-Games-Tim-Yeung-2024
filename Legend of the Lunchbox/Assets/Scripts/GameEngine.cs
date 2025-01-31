using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Assets
{
  [RequireComponent(typeof(TrialHandler))]
  public class GameEngine : MonoBehaviour
  {
    public class TimingData
    {
      public float EncounterStartDuration { get; set; }
      public float EncounterDiscoverableDuration { get; set; }
      public float EncounterTrialStartDuration { get; set; }

      public float ExplanationPromptDuration { get; set; }
      public float FixationDuration { get; set; }
      public float TrialDuration { get; set; }
      public float TrialEndDuration { get; set; }
      public float TrialFeedbackDuration { get; set; }
      public float EncounterEvaluationDuration { get; set; }
      public float EncounterEndDuration { get; set; }

      public float BreakDuration { get; set; }
      public float BreakFeedbackDuration { get; set; }

      public float LevelTransitionDuration { get; set; }
      
      public float EnemyMindShowTime { get; set; }
    }
    
    public static TimingData StaticTimeVariables;

    public static float CurrentRailDuration { get; private set; }


    // [Header("Assets")]
    // [SerializeField] private GameObject[] propertiesAndObjects;
    // public static GameObject[] PropertiesAndObjects;

    [FormerlySerializedAs("TutorialPrefab")] [Header("Paths")] [SerializeField] private GameObject tutorialPrefab;
    // [SerializeField] private string logFolderInDocs = "LotL";
    // public static string LogFolderInDocs;

    // Other
    // [SerializeField] private int levelId = 0;
    // public static int LevelId;

    // Connected components
    private TrialHandler trialHandler = null;
    
    private int playerHealth = 15;
    public int TotalHealth => playerHealth;
    [FormerlySerializedAs("MaxHealth")] public int maxHealth = 20;

    private void DamagePlayer()
    {
      playerHealth--;
      if (playerHealth == 0)
        playerHealth++;
    }

    private void HealPlayer()
    {
      if (playerHealth < maxHealth)
        playerHealth++;
    }

    private bool PlayerIsDead()
    {
      return playerHealth <= 0;
    }

    private void Awake()
    {
      Time.timeScale = 1f;
      
      // LogFolderInDocs = logFolderInDocs;

      feedbackEnabled = PlayerPrefs.GetInt(MainMenuHandler.FeedbackKey) == 1;
      promptEnabled = PlayerPrefs.GetInt(MainMenuHandler.PromptKey) == 1;
      
      Random.InitState(12345);
      
      trialHandler = GetComponent<TrialHandler>();

      if (LevelHandler.CurrentLevel == 0)
        Instantiate(tutorialPrefab);
    }

    private void Start()
    {
      Invoke(nameof(OnRail), StaticTimeVariables.LevelTransitionDuration);
    }

    public delegate void StateChangeEvent();
    public delegate void StateChangeEventTyped(EncounterData.PropertyType propertyType);
    public delegate void StateChangeEventInput(InputHandler.InputState input);
    public delegate void StateChangeEventCallback(Action<InputHandler.InputState> callback);

    public static event StateChangeEvent RespawnEvent;
    public static event StateChangeEvent OnRailStartedEvent;
    public static event StateChangeEvent StartingEncounterStartedEvent;
    public static event StateChangeEvent StartingBreakStartedEvent;
    public static event StateChangeEvent BreakingBadStartedEvent;
    public static event StateChangeEvent ShowingDiscoverableStartedEvent;
    public static event StateChangeEvent SettingUpMindStartedEvent;

    public static event StateChangeEvent ObjectDelayStartedEvent;
    public static event StateChangeEvent ShowingObjectInMindStartedEvent;
    public static event StateChangeEventTyped MovingToPropertyStartedEvent;
    public static event StateChangeEvent ThinkingOfPropertyStartedEvent;
    public static event StateChangeEventCallback ShowingPropertyStartedEvent;
    public static event StateChangeEventInput TrialInputRegisteredStartedEvent;
    public static event StateChangeEventInput EvaluatingInputStartedEvent;
    public static event StateChangeEventInput TimedOutStartedEvent;
    public static event StateChangeEvent AnswerWrongStartedEvent;
    public static event StateChangeEvent AnswerCorrectStartedEvent;
    public static event StateChangeEvent MovingToObjectStartedEvent;
    public static event StateChangeEvent EvaluatingEncounterStartedEvent;
    public static event StateChangeEvent WonBreakStartedEvent;
    public static event StateChangeEvent WonEncounterStartedEvent;
    public static event StateChangeEvent LostEncounterStartedEvent;
    public static event StateChangeEvent EndingEncounterStartedEvent;
    public static event StateChangeEvent EndingBreakStartedEvent;
    public static event StateChangeEvent LevelOverStartedEvent;

    private IEnumerator Timer(float duration, Action nextState)
    {
      yield return new WaitForSeconds(duration);

      nextState();
    }

    private void CutScene()
    {
      RespawnEvent?.Invoke();
      
      // TODO remove
    }

    private void OnRail()
    { 
      CurrentRailDuration = trialHandler.GetCurrentBlockDelay();
      
      OnRailStartedEvent?.Invoke();
      
      if (trialHandler.EncounterIsObject())
        StartCoroutine(Timer(CurrentRailDuration, StartingEncounter));
      else
        StartCoroutine(Timer(CurrentRailDuration, StartingBreak));
    }

    private void StartingBreak()
    {
      StartingBreakStartedEvent?.Invoke();
      
      StartCoroutine(Timer(StaticTimeVariables.EncounterStartDuration, BreakingBad));
    }

    private void BreakingBad()
    {
      BreakingBadStartedEvent?.Invoke();

      StartCoroutine(Timer(StaticTimeVariables.BreakDuration, WonBreak));
    }

    private void WonBreak()
    {
      HealPlayer();
      
      WonBreakStartedEvent?.Invoke();

      StartCoroutine(Timer(StaticTimeVariables.BreakFeedbackDuration, EndingBreak));
    }

    private void StartingEncounter()
    {
      StartingEncounterStartedEvent?.Invoke();

      StartCoroutine(Timer(StaticTimeVariables.EncounterStartDuration, ShowingDiscoverable));
    }

    private void ShowingDiscoverable()
    {
      ShowingDiscoverableStartedEvent?.Invoke();
      
      StartCoroutine(Timer(StaticTimeVariables.EncounterDiscoverableDuration, SettingUpMind));
    }

    private void SettingUpMind()
    {
      SettingUpMindStartedEvent?.Invoke();
      
      StartCoroutine(Timer(StaticTimeVariables.EncounterTrialStartDuration, ObjectDelay));
    }

    private void ObjectDelay()
    {
      if (trialHandler.EncounterOver)
        EvaluatingEncounter();
      else
      {
        ObjectDelayStartedEvent?.Invoke();
        StartCoroutine(Timer(trialHandler.GetCurrentTrialDelay(), ShowingObjectInMind));
      }
    }

    private void ShowingObjectInMind()
    {
      ShowingObjectInMindStartedEvent?.Invoke();
      StartCoroutine(Timer(StaticTimeVariables.EnemyMindShowTime, MovingToProperty));
    }

    private void MovingToProperty()
    {
      MovingToPropertyStartedEvent?.Invoke(trialHandler.GetCurrentEncounterPropertyType());

      if (promptEnabled)
        StartCoroutine(Timer(StaticTimeVariables.ExplanationPromptDuration, ThinkingOfProperty));
      else
        ThinkingOfProperty();
    }
    
    private void ThinkingOfProperty()
    {
      ThinkingOfPropertyStartedEvent?.Invoke();
      
      StartCoroutine(Timer(StaticTimeVariables.FixationDuration, ShowingProperty));
    }
    
    private void ShowingProperty()
    {
      ShowingPropertyStartedEvent?.Invoke(EvaluatingInput);
      
      StartCoroutine(Timer(StaticTimeVariables.TrialDuration, TimedOut));
    }
    
    // private void InputAvailable(InputHandler.InputState input)
    // {
    //   EvaluatingInput(input);
    // }

    private InputHandler.InputState currentInput;
    private void EvaluatingInput(InputHandler.InputState input)
    {
      currentInput = input;
      
      TrialInputRegisteredStartedEvent?.Invoke(input);
    }

    private void TimedOut()
    {
      bool correct = false;
      if (currentInput == InputHandler.InputState.NONE)
        TimedOutStartedEvent?.Invoke(currentInput);
      else
      {
        EvaluatingInputStartedEvent?.Invoke(currentInput);
        correct = trialHandler.EvaluateProperty(currentInput);
      }

      MovingToObject(correct);
      currentInput = InputHandler.InputState.NONE;
    }

    private bool feedbackEnabled;
    private bool promptEnabled;
    private void MovingToObject(bool answerCorrect)
    {
      MovingToObjectStartedEvent?.Invoke();
      
      StartCoroutine(Timer(StaticTimeVariables.TrialEndDuration, answerCorrect ? AnswerCorrect : AnswerWrong));
    }
    
    private void AnswerWrong()
    {
      AnswerWrongStartedEvent?.Invoke();
      
      trialHandler.IncreaseCounter();
     
      if (feedbackEnabled)
        StartCoroutine(Timer(StaticTimeVariables.TrialFeedbackDuration, ObjectDelay));
      else
        ObjectDelay();
    }

    private void AnswerCorrect()
    {
      AnswerCorrectStartedEvent?.Invoke();
      
      trialHandler.IncreaseCounter();
      
      if (feedbackEnabled)
        StartCoroutine(Timer(StaticTimeVariables.TrialFeedbackDuration, ObjectDelay));
      else
        ObjectDelay();
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
      
      StartCoroutine(Timer(StaticTimeVariables.EncounterEvaluationDuration, EndingEncounter));
    }

    private void LostEncounter()
    {
      DamagePlayer();
      
      LostEncounterStartedEvent?.Invoke();
      
      StartCoroutine(Timer(StaticTimeVariables.EncounterEvaluationDuration, EndingEncounter));
    }

    private void EndingEncounter()
    {
      EndingEncounterStartedEvent?.Invoke();

      if (PlayerIsDead())
      { 
        // Logger.Log("Played died");
        StartCoroutine(Timer(StaticTimeVariables.EncounterEndDuration, CutScene)); // TODO animations etc
      }
      else if (trialHandler.LevelOver)
        StartCoroutine(Timer(StaticTimeVariables.EncounterEndDuration, LevelOver));
      else
        StartCoroutine(Timer(StaticTimeVariables.EncounterEndDuration, OnRail));
    }

    private void EndingBreak()
    {
      EndingBreakStartedEvent?.Invoke();
      
      if (trialHandler.LevelOver)
        StartCoroutine(Timer(StaticTimeVariables.EncounterEndDuration, LevelOver));
      else
        StartCoroutine(Timer(StaticTimeVariables.EncounterEndDuration, OnRail));
    }

    private void LevelOver()
    {
      print("LevelOver");
      LevelOverStartedEvent?.Invoke();

      StartCoroutine(Timer(StaticTimeVariables.LevelTransitionDuration, LoadNext));
    }

    private void LoadNext()
    {
      print("Loading next level");
      StartCoroutine(LoadScene());
    }

    private IEnumerator LoadScene()
    {
      AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(3);
      
      while (!asyncLoad.isDone)
      {
        yield return null;
      }
    }
    
    private void Update() 
    {
      if (Input.GetKeyDown(KeyCode.Backspace))
        LoadNext();
    }
  }
}