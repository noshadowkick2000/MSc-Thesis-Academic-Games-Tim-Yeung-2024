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
    [FormerlySerializedAs("mindUI")] [SerializeField] private Image mindBG;
    // [SerializeField] private Image mindPanel;
    [SerializeField] private Color spotColor;
    [SerializeField] private GameObject spotLight;
    [SerializeField] private Material spotLightMaterial;
    [SerializeField] private GameObject thoughtUI;
    [SerializeField] private TextMeshProUGUI thoughtWords;
    [SerializeField] private GameObject scramble;
    [SerializeField] private Image scrambleImage;
    [FormerlySerializedAs("thoughtSprites")] [SerializeField] private Sprite[] scrambleSprites;
    [SerializeField] private GameObject controlIndicatorUI;
    [SerializeField] private GameObject[] controlButtonUI;
    [SerializeField] private RectTransform progressBarUI;
    [SerializeField] private RectTransform imaginationBarUI;
    [SerializeField] private GameObject endScreen;
    [SerializeField] private Transform stencil;
    [SerializeField] private GameObject flashBang;
    [SerializeField] private Image timerUI;
    [SerializeField] private GameObject breakInstructions;

    private Color maxColorBg;
    
    void Start()
    {
        thoughtUI.SetActive(false);
        maxColorBg = mindBG.color;
        mindBG.color = Color.clear;
        controlIndicatorUI.SetActive(false);
        flashBang.SetActive(false);
        scramble.SetActive(false);
        breakInstructions.SetActive(false);

        StartPinhole(true, GameEngine.StaticTimeVariables.LevelTransitionDuration);
    }

    private void Awake()
    {
        SubscribeToEvents();
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    private void Idle()
    {
        controlIndicatorUI.SetActive(false);
        // mindBG.SetActive(false);
        StartCoroutine(FadeScreen(false, .1f));
        thoughtUI.SetActive(false);
    }

    private Coroutine thoughtRoutine;

    private void StartThought(EncounterData.PropertyType propertyType) 
    {
        thoughtUI.SetActive(true);
        controlIndicatorUI.SetActive(false);
        thoughtWords.text = LocalizationTextLoader.GetLocaleEntry(0);

        switch (propertyType)
        {
            case EncounterData.PropertyType.ACTION:
                thoughtWords.text += LocalizationTextLoader.GetLocaleEntry(1);
                break;
            case EncounterData.PropertyType.SOUND:
                thoughtWords.text += LocalizationTextLoader.GetLocaleEntry(2);
                break;
            case EncounterData.PropertyType.WORD:
                thoughtWords.text += LocalizationTextLoader.GetLocaleEntry(3);
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
        float startTime = Time.time;
        float x = 0;
        float y;
        float power = 10f;
        Color a = Color.white; 

        while (x < 1)
        {
            y = x < .5f ? 1 - Mathf.Pow(x-1, power) : 1 - Mathf.Pow(x, power);
            a.a = y;
            thoughtWords.color = a;
            x = (Time.time - startTime) / GameEngine.StaticTimeVariables.ExplanationPromptDuration;

            yield return null;
        }
    }

    private IEnumerator AnimateFixation()
    {
        float startTime = Time.time;
        float x = 0;
        float y;
        float power = 10f;
        Color a = Color.white; 

        thoughtWords.text = ".";
        
        while (x < 1)
        {
            y = x < .5f ? 1 - Mathf.Pow(x-1, power) : 1 - Mathf.Pow(x, power);
            a.a = y;
            thoughtWords.color = a;
            x = (Time.time - startTime) / GameEngine.StaticTimeVariables.FixationDuration;

            if (x > .66f)
                thoughtWords.text = "...";
            else if (x > .33f)
                thoughtWords.text = "..";

            yield return null;
        }
    }

    private IEnumerator AnimateShakePrompt()
    {
        thoughtWords.text = LocalizationTextLoader.GetLocaleEntry(4);
        
        float startTime = Time.time;
        float x = 0;
        float y;
        float power = 10f;
        Color a = Color.white; 

        while (x < 1)
        {
            y = x < .5f ? 1 - Mathf.Pow(x-1, power) : 1 - Mathf.Pow(x, power);
            a.a = y;
            thoughtWords.color = a;
            x = (Time.time - startTime) / (GameEngine.StaticTimeVariables.BreakDuration / 2f);

            yield return null;
        }
        
        thoughtWords.color = Color.clear;
    }
    
    private IEnumerator FadeScreen(bool fadingIn, float duration)
    {
        Color startColor = fadingIn ? Color.clear : maxColorBg;
        Color endColor = fadingIn ? maxColorBg : Color.clear;
        
        mindBG.color = startColor;

        float startTime = Time.time;
        float x = 0;
        Color tempColor = maxColorBg;

        while (x < 1)
        {
            tempColor.a = Mathf.Lerp(startColor.a, endColor.a, UtilsT.EasedT(x));
         
            mindBG.color = tempColor;
         
            x = (Time.time - startTime) / duration;
            yield return null;
        }
      
        mindBG.color = endColor;
    }

    Coroutine timerRoutine;

    private void EndThought(float timeOut) 
    { 
        thoughtUI.SetActive(false);
        controlIndicatorUI.SetActive(true);

        StopCoroutine(thoughtRoutine);
        timerRoutine = StartCoroutine(AnimateTimer(timeOut));
    }

    private void CancelTimer()
    {
        StopCoroutine(timerRoutine);
    }

    private IEnumerator AnimateTimer(float timeOut)
    {
        float startTime = Time.time;
        timerUI.fillAmount = 0;
        while (Time.time < startTime + timeOut)
        {
            timerUI.fillAmount = (Time.time - startTime) / timeOut;
            yield return null;
        }

        timerUI.fillAmount = 1;
    }

    private void SubscribeToEvents()
    {
        GameEngine.StartingBreakStartedEvent += StartingBreak;
        GameEngine.BreakingBadStartedEvent += BreakingBad;
        GameEngine.SettingUpMindStartedEvent += SettingUpMind;
        GameEngine.ObjectDelayStartedEvent += ObjectDelay;
        GameEngine.ShowingObjectInMindStartedEvent += ShowingObjectInMind;
        GameEngine.MovingToPropertyStartedEvent += MovingToProperty;
        GameEngine.ThinkingOfPropertyStartedEvent += ThinkingOfProperty;
        GameEngine.ShowingPropertyStartedEvent += ShowingProperty;
        GameEngine.EvaluatingInputStartedEvent += EvaluatingInput;
        GameEngine.AnswerWrongStartedEvent += AnswerWrong;
        GameEngine.TrialInputRegisteredStartedEvent += TrialInputRegistered;
        GameEngine.MovingToObjectStartedEvent += MovingToObject;
        GameEngine.EvaluatingEncounterStartedEvent += EvaluatingEncounter;
        GameEngine.EndingEncounterStartedEvent += EndingEncounter;
        GameEngine.LostEncounterStartedEvent += LostEncounter;
        GameEngine.WonBreakStartedEvent += WonBreak;
        GameEngine.EndingBreakStartedEvent += EndingBreak;
        GameEngine.LevelOverStartedEvent += LevelOver;
    }

    private void UnsubscribeFromEvents()
    {
        GameEngine.StartingBreakStartedEvent -= StartingBreak;
        GameEngine.BreakingBadStartedEvent -= BreakingBad;
        GameEngine.SettingUpMindStartedEvent -= SettingUpMind;
        GameEngine.ObjectDelayStartedEvent -= ObjectDelay;
        GameEngine.ShowingObjectInMindStartedEvent -= ShowingObjectInMind;
        GameEngine.MovingToPropertyStartedEvent -= MovingToProperty;
        GameEngine.ThinkingOfPropertyStartedEvent -= ThinkingOfProperty;
        GameEngine.ShowingPropertyStartedEvent -= ShowingProperty;
        GameEngine.EvaluatingInputStartedEvent -= EvaluatingInput;
        GameEngine.AnswerWrongStartedEvent -= AnswerWrong;
        GameEngine.TrialInputRegisteredStartedEvent -= TrialInputRegistered;
        GameEngine.MovingToObjectStartedEvent -= MovingToObject;
        GameEngine.EvaluatingEncounterStartedEvent -= EvaluatingEncounter;
        GameEngine.EndingEncounterStartedEvent -= EndingEncounter;
        GameEngine.LostEncounterStartedEvent -= LostEncounter;
        GameEngine.WonBreakStartedEvent -= WonBreak;
        GameEngine.EndingBreakStartedEvent -= EndingBreak;
        GameEngine.LevelOverStartedEvent -= LevelOver;
    }

    private void StartingBreak()
    {
        SettingUpMind();
    }

    private void BreakingBad()
    {
        Flash();
        
        breakInstructions.SetActive(true);
        // mindBG.SetActive(true);
        StartCoroutine(FadeScreen(true, .1f));
        thoughtUI.SetActive(true);
        StartCoroutine(AnimateShakePrompt());
    }

    private void WonBreak()
    {
        breakInstructions.SetActive(false);
        // mindBG.SetActive(false);
        StartCoroutine(FadeScreen(false, .1f));

        thoughtUI.SetActive(false);
        
        LeanTween.moveY(imaginationBarUI, 20f, GameEngine.StaticTimeVariables.EncounterTrialStartDuration/4).setEaseInBack();
        lostAnimation = true;
    }

    private void EndingBreak()
    {
        EndingEncounter();
    }

    private void Flash()
    {
        flashBang.SetActive(true);
        StartCoroutine(FlashOff());
    }

    private IEnumerator FlashOff()
    {
        yield return new WaitForSeconds(.1f);
        flashBang.SetActive(false);
    }
    
    private void SettingUpMind()
    {
        LeanTween.moveY(progressBarUI,  + 50f, GameEngine.StaticTimeVariables.EncounterTrialStartDuration/4).setEaseInBack();
        LeanTween.moveY(imaginationBarUI, -100f, GameEngine.StaticTimeVariables.EncounterTrialStartDuration/4).setEaseInBack();
    }
    
    private void MovingToProperty(EncounterData.PropertyType propertyType)
    { 
        StartThought(propertyType);
    }

    private void ThinkingOfProperty()
    {
        StartCoroutine(AnimateFixation());
    }

    private void ShowingProperty(Action<InputHandler.InputState> callback)
    {
        EndThought(GameEngine.StaticTimeVariables.TrialDuration);
    }

    private void EvaluatingInput(InputHandler.InputState input)
    {
        CancelTimer();
    }

    private void AnswerWrong()
    {
        // scramble.SetActive(true);
        // StartCoroutine(AnimateScramble());
    }

    private void MovingToObject()
    {
        controlIndicatorUI.SetActive(false);

        foreach (var button in controlButtonUI)
        {
            button.transform.localScale = Vector3.one;
        }
    }
    
    private void TrialInputRegistered(InputHandler.InputState input)
    {
        int ind = (int)input - 1;
        LeanTween.scale(controlButtonUI[ind], 1.1f * Vector3.one, .1f);
    }

    private void ObjectDelay()
    {
        // mindBG.SetActive(true);
        if (mindBG.color.a != 0) return;
        
        StartCoroutine(FadeScreen(true, .1f));

    }

    private void ShowingObjectInMind()
    {
        Flash();
    }

    private void EvaluatingEncounter()
    {
        Idle();
        // StartPinhole(true, GameEngine.StaticTimeVariables.EncounterEndDuration);
    }

    private bool lostAnimation = false;
    private void LostEncounter()
    {
        LeanTween.moveY(imaginationBarUI, 20f, GameEngine.StaticTimeVariables.EncounterTrialStartDuration/4).setEaseInBack();
        lostAnimation = true;

        StartCoroutine(LostAnimation());
    }
    
    private IEnumerator LostAnimation()
    {
        // Vector3 randomVector = new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
      
        yield return new WaitForSeconds(GameEngine.StaticTimeVariables.EncounterEvaluationDuration / 4);

        Flash();
    }

    private void EndingEncounter()
    {
        LeanTween.moveY(progressBarUI,  -15f, GameEngine.StaticTimeVariables.EncounterTrialStartDuration/4).setEaseInBack();

        if (lostAnimation)
            lostAnimation = false;
        else
            LeanTween.moveY(imaginationBarUI, 20f, GameEngine.StaticTimeVariables.EncounterTrialStartDuration/4).setEaseInBack();
    }

    private void LevelOver()
    {
        StartPinhole(false, GameEngine.StaticTimeVariables.LevelTransitionDuration);
        print(GameEngine.StaticTimeVariables.LevelTransitionDuration);
        
    }

    private IEnumerator AnimatePinhole(bool opening, float duration)
    {
        stencil.localScale = opening ? Vector3.zero : Vector3.one;
        
        float startTime = Time.time;
        float x = 0;
        
        while (x < 1)
        {
            x = ((Time.time - startTime) / duration);
            float y = opening ? x : 1 - x;
            stencil.localScale = new Vector3(y, y, 0);
            yield return null;
        }
        
        stencil.localScale = opening ? Vector3.one : Vector3.zero;
        if (opening)
            endScreen.SetActive(false);
    }
}
