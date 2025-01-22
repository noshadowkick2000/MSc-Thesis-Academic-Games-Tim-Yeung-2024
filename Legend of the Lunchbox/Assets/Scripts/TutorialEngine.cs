using System;
using System.Collections;
using System.Collections.Generic;
using Assets;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialEngine : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private GameObject textPanel;
    [SerializeField] private TextMeshProUGUI name;
    [SerializeField] private TextMeshProUGUI instruction;

    private readonly float bottomY = 10f;
    private readonly float middleY = 165f;
    public void SetText(int id, bool bottom)
    {
        text.SetText(LocalizationTextLoader.GetLocaleEntry(id));
        textPanel.SetActive(true);
        ((RectTransform)textPanel.transform).anchoredPosition = new Vector3(0, bottom ? bottomY : middleY);
        
        blocked = true;
        Time.timeScale = 0f;
    }

    public void ClearText()
    {
        text.SetText("");
        textPanel.SetActive(false);
    }
    private void Awake()
    {
        SubscribeToEvents();
    }

    private void Start()
    {
        name.text = LocalizationTextLoader.GetLocaleEntry(44);
        instruction.text = LocalizationTextLoader.GetLocaleEntry(45);
        textPanel.SetActive(false);
    }

    private void OnDestroy()
    {
        UnSubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
        GameEngine.OnRailStartedEvent += OnRail;
        GameEngine.ShowingDiscoverableStartedEvent += ShowingDiscoverable;
        GameEngine.ShowingObjectInMindStartedEvent += ShowingObject;
        GameEngine.ThinkingOfPropertyStartedEvent += ShowingProperty;
        GameEngine.AnswerCorrectStartedEvent += AnswerCorrect;
        GameEngine.AnswerWrongStartedEvent += AnswerWrong;
        GameEngine.EvaluatingEncounterStartedEvent += EvaluatingEncounter;
        GameEngine.WonEncounterStartedEvent += WonEncounter;
        GameEngine.LostEncounterStartedEvent += LostEncounter;
        GameEngine.LevelOverStartedEvent += LevelOver;
        GameEngine.StartingBreakStartedEvent += StartingBreak;
        GameEngine.WonBreakStartedEvent += WonBreak;
    }

    private void UnSubscribeToEvents()
    {
        GameEngine.OnRailStartedEvent -= OnRail;
        GameEngine.ShowingDiscoverableStartedEvent -= ShowingDiscoverable;
        GameEngine.ShowingObjectInMindStartedEvent -= ShowingObject;
        GameEngine.ThinkingOfPropertyStartedEvent -= ShowingProperty;
        GameEngine.AnswerCorrectStartedEvent -= AnswerCorrect;
        GameEngine.AnswerWrongStartedEvent -= AnswerWrong;
        GameEngine.EvaluatingEncounterStartedEvent -= EvaluatingEncounter;
        GameEngine.WonEncounterStartedEvent -= WonEncounter;
        GameEngine.LostEncounterStartedEvent -= LostEncounter;
        GameEngine.LevelOverStartedEvent -= LevelOver;
        GameEngine.StartingBreakStartedEvent -= StartingBreak;
        GameEngine.WonBreakStartedEvent -= WonBreak;
    }

    private int railCounter = 0;
    private void OnRail()
    {
        railCounter++;
        
        if (railCounter == 1)
            SetText(31, true);
        if (railCounter == 2)
            SetText(41, false);
    }

    private bool firstDiscoverable = true;
    private void ShowingDiscoverable()
    {
        if (!firstDiscoverable) return;
        
        SetText(32, true);
        firstDiscoverable = false;
    }

    private int objectCounter = 0;
    private void ShowingObject()
    {
        objectCounter++;

        StartCoroutine(Delay());
    }

    private IEnumerator Delay()
    {
        yield return new WaitForSeconds(GameEngine.StaticTimeVariables.EnemyMindShowTime / 2);
        
        if (objectCounter == 1)
        {
            SetText(33, true);
        }
        else if (objectCounter == 3)
        {
            SetText(42, true);
        }
    }

    private void ShowingProperty()
    {
        if (objectCounter == 1)
        { 
            SetText(34, true);   
        }
        else if (objectCounter == 2)
        {
            SetText(37, true);
        }
    }

    private void AnswerCorrect()
    {
        SetText(35, true);
    }

    private void AnswerWrong()
    {
        SetText(36, true);
    }
    
    private void EvaluatingEncounter()
    {
        if (objectCounter == 2)
            SetText(38, true);
    }

    private void WonEncounter()
    {
        SetText(39, true);
    }

    private void LostEncounter()
    {
        SetText(40, true);
    }

    private void LevelOver()
    {
        SetText(43, true);
    }

    private void StartingBreak()
    {
        SetText(46, true);
    }

    private void WonBreak()
    {
        SetText(47, false);
    }

    private bool blocked;
    private void Update()
    {
        if ((Input.GetButtonDown("Use") || Input.GetButtonDown("Discard")) && blocked)
        {
            blocked = false;
            Time.timeScale = 1f;
            ClearText();
        }

        if (Input.GetKeyDown(KeyCode.Backspace))
            StartCoroutine(LoadNextScene());
    }

    private IEnumerator LoadNextScene()
    { 
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(3, LoadSceneMode.Single);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
