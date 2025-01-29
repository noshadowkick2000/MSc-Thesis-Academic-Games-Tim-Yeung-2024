using System;
using System.Collections;
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
    [SerializeField] private AudioClip goodFeedback;
    [SerializeField] private AudioClip badFeedback;
    [SerializeField] private AudioClip foundDiscoverable;
    [SerializeField] private AudioClip cork;
    [SerializeField] private AudioClip corkPop;
    [SerializeField] private AudioClip drinking;
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
        if (PlayerPrefs.GetInt(MainMenuHandler.SoundKey) == 1)
        {
            Init();
            SubscribeToEvents();
        }
        else
            Destroy(this);
    }

    private void OnDestroy()
    {
        UnSubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
        GameEngine.OnRailStartedEvent += OnRail;
        GameEngine.StartingEncounterStartedEvent += StartingEncounter;
        GameEngine.StartingBreakStartedEvent += StartingBreak;
        GameEngine.BreakingBadStartedEvent += BreakingBad;
        GameEngine.WonBreakStartedEvent += WonBreak;
        GameEngine.SettingUpMindStartedEvent += SettingUpMind;
        GameEngine.ShowingObjectInMindStartedEvent += ShowingObjectInMind;
        GameEngine.ShowingPropertyStartedEvent += ShowingProperty;
        GameEngine.AnswerCorrectStartedEvent += AnswerCorrect;
        GameEngine.AnswerWrongStartedEvent += AnswerWrong;
        GameEngine.WonEncounterStartedEvent += WonEncounter;
        GameEngine.LostEncounterStartedEvent += LostEncounter;
        GameEngine.EvaluatingEncounterStartedEvent += EvaluatingEncounter;
    }
  
    private void UnSubscribeToEvents()
    {
        GameEngine.OnRailStartedEvent -= OnRail;
        GameEngine.StartingEncounterStartedEvent -= StartingEncounter;
        GameEngine.StartingBreakStartedEvent -= StartingBreak;
        GameEngine.BreakingBadStartedEvent -= BreakingBad;
        GameEngine.WonBreakStartedEvent -= WonBreak;
        GameEngine.SettingUpMindStartedEvent -= SettingUpMind;
        GameEngine.ShowingObjectInMindStartedEvent -= ShowingObjectInMind;
        GameEngine.ShowingPropertyStartedEvent -= ShowingProperty;
        GameEngine.AnswerCorrectStartedEvent -= AnswerCorrect;
        GameEngine.AnswerWrongStartedEvent -= AnswerWrong;
        GameEngine.WonEncounterStartedEvent -= WonEncounter;
        GameEngine.LostEncounterStartedEvent -= LostEncounter;
        GameEngine.EvaluatingEncounterStartedEvent -= EvaluatingEncounter;
    }

    private void Update()
    {
        if (!breaking || oneShotPlayer.isPlaying ||
            ((!Input.GetKeyDown(KeyCode.LeftArrow) && !Input.GetKeyDown(KeyCode.RightArrow)))) return;
        oneShotPlayer.pitch = InputHandler.InputAverage + .5f;
        oneShotPlayer.PlayOneShot(cork);
        // oneShotPlayer.pitch = 1;
    }

    private void OnRail()
    {
        foreach (var runPlayer in runPlayers)
        {
            runPlayer.UnPause();
        }
    }

    private void StartingEncounter()
    {
        foreach (var runPlayer in runPlayers)
        {
            runPlayer.Pause();
        }
        
        oneShotPlayer.PlayOneShot(foundDiscoverable);
    }

    private void StartingBreak()
    {
        StartingEncounter();
    }

    private bool breaking = false;
    private void BreakingBad()
    {
        breaking = true;
    }

    private void WonBreak()
    {
        breaking = false;
        oneShotPlayer.pitch = 1f;
        oneShotPlayer.PlayOneShot(corkPop, .3f);
        
        oneShotPlayer.PlayOneShot(drinking, 2f);
    }

    private void SettingUpMind()
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

    private void ShowingObjectInMind()
    {
        oneShotPlayer.PlayOneShot(enemyAppear, .6f);
    }

    private void ThinkingOfProperty(bool encounterOver)
    {
        // if (!encounterOver) 
        //     oneShotPlayer.PlayOneShot(spotLight);
    }
    
    private void ShowingProperty(Action<InputHandler.InputState> callback)
    {
        // oneShotPlayer.PlayOneShot(enemyAppear);
    }

    private void AnswerCorrect()
    {
        oneShotPlayer.PlayOneShot(goodFeedback);
    }

    private void AnswerWrong()
    {
        oneShotPlayer.PlayOneShot(badFeedback);
    }

    private void EvaluatingEncounter()
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

    private void WonEncounter()
    {
        oneShotPlayer.PlayOneShot(enterMind);
    }

    private void LostEncounter()
    {
        oneShotPlayer.PlayOneShot(worldDeform);

        StartCoroutine(FlashSoundDelayed());
    }

    private IEnumerator FlashSoundDelayed()
    {
        yield return new WaitForSeconds(GameEngine.StaticTimeVariables.EncounterEvaluationDuration / 4);
        
        oneShotPlayer.PlayOneShot(enemyAppear);
    }
}
