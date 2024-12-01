using System;
using System.Collections;
using System.Collections.Generic;
using Assets;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private GameObject mindUI;
    [SerializeField] private Image mindPanel;
    [SerializeField] private Color spotColor;
    [SerializeField] private GameObject spotLight;
    [SerializeField] private Material spotLightMaterial;
    [SerializeField] private GameObject thoughtUI;
    [SerializeField] private TextMeshProUGUI thoughtWords;
    [SerializeField] private Sprite[] thoughtSprites;
    [SerializeField] private GameObject controlIndicatorUI;
    [SerializeField] private GameObject distractionUI;
    [SerializeField] private GameObject endScreen;
    [SerializeField] private Transform stencil;
    [SerializeField] private GameObject flashBang;
    [SerializeField] private Image timerUI;
    
    void Awake()
    {
        thoughtUI.SetActive(false);
        mindUI.SetActive(false);
        controlIndicatorUI.SetActive(false);
        flashBang.SetActive(false);

        StartPinhole(true, GameEngine.LevelOverTime);
        
        SubscribeToEvents();
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    private void Idle()
    {
        controlIndicatorUI.SetActive(false);
        mindUI.SetActive(false);
        thoughtUI.SetActive(false);
    }

    private void StartMind(float duration)
    {
        distractionUI.SetActive(false);
    }

    private Coroutine thoughtRoutine;

    private void StartThought(TrialHandler.PropertyType propertyType) 
    {
        thoughtUI.SetActive(true);
        controlIndicatorUI.SetActive(false);
        thoughtWords.text = "Hmm.. ";

        switch (propertyType)
        {
            case TrialHandler.PropertyType.ACTION:
                thoughtWords.text += "how would this be used?";
                break;
            case TrialHandler.PropertyType.SOUND:
                thoughtWords.text += "what would this sound like?";
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(propertyType), propertyType, null);
        }

        thoughtRoutine = StartCoroutine(AnimateThought());
    }
    
    private void StartPinhole(bool opening, float duration)
    {
        StartCoroutine(AnimatePinhole(opening, duration));
        if (!opening)
            endScreen.SetActive(true);
    }

    private IEnumerator AnimateThought()
    {
        // Image img = thoughtUI.GetComponent<Image>();
        // int counter = 0;
        //
        // while (true)
        // {
        //     img.sprite = thoughtSprites[counter];
        //     counter++;
        //     if (counter == thoughtSprites.Length)
        //         counter = 0;
        //     
        //     yield return new WaitForSecondsRealtime(.2f);
        // }

        float startTime = Time.realtimeSinceStartup;
        float x = 0;
        float y;
        float power = 16f;
        Color a = Color.white; 

        while (x < 1)
        {
            y = x < .5f ? 1 - Mathf.Pow(x-1, power) : 1 - Mathf.Pow(x, power);
            a.a = y;
            thoughtWords.color = a;
            x = (Time.realtimeSinceStartup - startTime) / GameEngine.MindPropertyTransitionTime;

            yield return null;
        }

        startTime = Time.realtimeSinceStartup;
        x = 0;
        thoughtWords.text = ".";

        while (x < 1)
        {
            y = x < .5f ? 1 - Mathf.Pow(x-1, power) : 1 - Mathf.Pow(x, power);
            a.a = y;
            thoughtWords.color = a;
            x = (Time.realtimeSinceStartup - startTime) / GameEngine.PullingTime;

            if (x > .66f)
                thoughtWords.text = "...";
            else if (x > .33f)
                thoughtWords.text = "..";

            yield return null;
        }
    }

    Coroutine timerRoutine;

    private void EndThought(float timeOut) 
    { 
        thoughtUI.SetActive(false);
        controlIndicatorUI.SetActive(true);

        StopCoroutine(thoughtRoutine);
        timerRoutine = StartCoroutine(AnimateTimer(timeOut));
    }

    private void ExitMind()
    {
        distractionUI.SetActive(true);
    }

    private void CancelTimer()
    {
        StopCoroutine(timerRoutine);
    }

    private IEnumerator AnimateTimer(float timeOut)
    {
        float startTime = Time.realtimeSinceStartup;
        timerUI.fillAmount = 0;
        while (Time.realtimeSinceStartup < startTime + timeOut)
        {
            timerUI.fillAmount = (Time.realtimeSinceStartup - startTime) / timeOut;
            yield return null;
        }

        timerUI.fillAmount = 1;
    }

    private void SubscribeToEvents()
    {
        GameEngine.ShowingEnemyStartedEvent += ShowingEnemy;
        GameEngine.SettingUpMindStartedEvent += SettingUpMind;
        GameEngine.ShowingEnemyInMindStartedEvent += ShowingEnemyInMind;
        GameEngine.MovingToPropertyStartedEvent += MovingToProperty;
        GameEngine.ShowingPropertyStartedEvent += ShowingProperty;
        GameEngine.EvaluatingInputStartedEvent += EvaluatingInput;
        GameEngine.TimedOutStartedEvent += TimedOut;
        GameEngine.MovingToEnemyStartedEvent += MovingToEnemy;
        GameEngine.EvaluatingEncounterStartedEvent += EvaluatingEncounter;
        GameEngine.EndingEncounterStartedEvent += EndingEncounter;
        GameEngine.LevelOverStartedEvent += LevelOver;
    }

    private void UnsubscribeFromEvents()
    {
        GameEngine.ShowingEnemyStartedEvent -= ShowingEnemy;
        GameEngine.SettingUpMindStartedEvent -= SettingUpMind;
        GameEngine.ShowingEnemyInMindStartedEvent -= ShowingEnemyInMind;
        GameEngine.MovingToPropertyStartedEvent -= MovingToProperty;
        GameEngine.ShowingPropertyStartedEvent -= ShowingProperty;
        GameEngine.EvaluatingInputStartedEvent -= EvaluatingInput;
        GameEngine.TimedOutStartedEvent -= TimedOut;
        GameEngine.MovingToEnemyStartedEvent -= MovingToEnemy;
        GameEngine.EvaluatingEncounterStartedEvent -= EvaluatingEncounter;
        GameEngine.EndingEncounterStartedEvent -= EndingEncounter;
        GameEngine.LevelOverStartedEvent -= LevelOver;
    }

    protected virtual void ShowingEnemy()
    {
        flashBang.SetActive(true);
        StartCoroutine(FlashOff());
    }

    private IEnumerator FlashOff()
    {
        yield return new WaitForSecondsRealtime(.1f);
        flashBang.SetActive(false);
    }
    
    protected virtual void SettingUpMind()
    {
        StartMind(GameEngine.MindStartTime);
        
        StartPinhole(false, GameEngine.MindStartTime);
    }
    
    protected virtual void MovingToProperty(TrialHandler.PropertyType propertyType)
    { 
        StartThought(propertyType);
    }

    protected virtual void ShowingProperty(Action<InputHandler.InputState> callback)
    {
        EndThought(GameEngine.EnemyTimeOut);
    }

    protected virtual void EvaluatingInput(InputHandler.InputState input)
    {
        CancelTimer();
    }

    protected virtual void TimedOut()
    {
    }

    protected virtual void MovingToEnemy()
    {
        controlIndicatorUI.SetActive(false);
    }

    protected virtual void ShowingEnemyInMind()
    {
        mindUI.SetActive(true);
    }

    protected virtual void EvaluatingEncounter()
    {
        Idle();
        StartPinhole(true, GameEngine.playerReset);
    }

    protected virtual void EndingEncounter()
    {
        ExitMind();
    }

    protected virtual void LevelOver()
    {
        StartPinhole(false, GameEngine.LevelOverTime);
    }

    private IEnumerator AnimatePinhole(bool opening, float duration)
    {
        stencil.localScale = opening ? Vector3.zero : Vector3.one;
        
        float startTime = Time.realtimeSinceStartup;

        while (Time.realtimeSinceStartup < startTime + duration)
        {
            float x = ((Time.realtimeSinceStartup - startTime) / duration);
            if (!opening)
                x = 1 - x;
            stencil.localScale = new Vector3(x, x, 0);
            yield return null;
        }
        
        stencil.localScale = opening ? Vector3.one : Vector3.zero;
        if (opening)
            endScreen.SetActive(false);
    }
}
