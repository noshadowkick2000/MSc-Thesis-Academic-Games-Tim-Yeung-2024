using System;
using System.Collections;
using System.Collections.Generic;
using Assets;
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

    private void StartThought(float duration) 
    {
        thoughtUI.SetActive(true);
        controlIndicatorUI.SetActive(false);

        thoughtRoutine = StartCoroutine(AnimateThought());
    }
    
    private void StartPinhole(bool opening, float duration)
    {
        StartCoroutine(AnimatePinhole(opening, duration));
        if (!opening)
            endScreen.SetActive(true);
    }

    private IEnumerator DelayedSpotlight(float duration)
    {
        yield return new WaitForSecondsRealtime(duration / 4f);
        spotLight.SetActive(true);
    }

    private IEnumerator AnimateThought()
    {
        Image img = thoughtUI.GetComponent<Image>();
        int counter = 0;

        while (true)
        {
            img.sprite = thoughtSprites[counter];
            counter++;
            if (counter == thoughtSprites.Length)
                counter = 0;
            
            yield return new WaitForSecondsRealtime(.2f);
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
        GameEngine.ThinkingOfPropertyStartedEvent += ThinkingOfProperty;
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
        GameEngine.ThinkingOfPropertyStartedEvent -= ThinkingOfProperty;
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
    
    protected virtual void ThinkingOfProperty()
    { 
        StartThought(GameEngine.PullingTime);
    }

    protected virtual void ShowingProperty(Action<InputHandler.InputState> callback)
    {
        EndThought(GameEngine.EnemyTimeOut);
    }

    protected virtual void EvaluatingInput()
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
