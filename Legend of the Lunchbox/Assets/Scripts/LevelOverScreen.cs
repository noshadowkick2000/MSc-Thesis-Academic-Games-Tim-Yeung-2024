using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class LevelOverScreen : MonoBehaviour
{
    [FormerlySerializedAs("ProgressBar")] [SerializeField] private GameObject progressBar;
    [FormerlySerializedAs("ProgressYGoal")] [SerializeField] private float progressYGoal = 165f;
    [FormerlySerializedAs("StatsPanel")] [SerializeField] private GameObject statsPanel;
    [FormerlySerializedAs("StatsPanelYGoal")] [SerializeField] private float statsPanelYGoal = -25f;
    [FormerlySerializedAs("StatTexts")] [SerializeField] private TextMeshProUGUI[] statTexts;
    [SerializeField] private Image faderPanel;
    
    
    private void Start()
    {
        SetValues();
        
        StartCoroutine(Fader(.5f, true));
        StartCoroutine(AnimatePin(2f));
        
        LeanTween.moveLocalY(progressBar, progressYGoal, .5f).setDelay(2f).setEaseOutBack();
        LeanTween.moveLocalY(statsPanel, statsPanelYGoal, .5f).setDelay(2.5f).setEaseOutBack();

        int counter = 0;
        foreach (var text in statTexts)
        {
            LeanTween.scale(text.gameObject, Vector3.one,.2f).setDelay(2.5f + (counter * .2f)).setEaseOutBack();
            counter++;
        }
    }

    private void SetValues()
    {
        statTexts[1].text = GameStatsTracker.EncountersWon.ToString();
        statTexts[3].text = GameStatsTracker.ComboCounter.ToString();
        statTexts[5].text = TimeSpan.FromSeconds(GameStatsTracker.CompletionTime).ToString(@"mm\:ss");

        string grade = "A";
        if (GameStatsTracker.ComboCounter == GameStatsTracker.EncountersWon)
            grade = "S";
        else if (GameStatsTracker.ComboCounter > 4)
            grade = "A++";
        else if (GameStatsTracker.ComboCounter > 2)
            grade = "A+";

        statTexts[6].text = grade;
    }

    private IEnumerator Fader(float duration, bool fadeIn)
    {
        Color tempColor = Color.black;
        tempColor.a = fadeIn ? 1f : 0f;
        
        float x = 0;
        float startTime = Time.time;
        
        while (x < 1)
        {
            x = (Time.time - startTime) / duration;

            tempColor.a = fadeIn ? 1 - x : x;
            faderPanel.color = tempColor;
            
            yield return null;
        }

        faderPanel.color = fadeIn ? Color.clear : Color.black;
    }

    private IEnumerator AnimatePin(float duration)
    {
        Slider progress = progressBar.GetComponentInChildren<Slider>();

        float start = 0f; //TEMP load in actual level Progress
        float end = 1f;
        float x = 0;
        float startTime = Time.time;
        
        while (x < 1)
        {
            x = (Time.time - startTime) / duration;

            progress.value = Mathf.Lerp(start, end, UtilsT.EasedT(x));
            
            yield return null;
        }

        progress.value = end;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Use") || Input.GetButton("Discard"))
        {
            StartCoroutine(Fader(0.5f, false));
            StartCoroutine(LoadNextLevel());
        }
    }

    private IEnumerator LoadNextLevel()
    {
        yield return new WaitForSeconds(.5f);

        int nextLevel = 4 + GameStatsTracker.LastLevel;
        if (nextLevel == SceneManager.GetActiveScene().buildIndex)
            nextLevel = 0;
        
        AsyncOperation async = SceneManager.LoadSceneAsync(0);

        while (!async.isDone)
        {
            yield return null;
        }
    }
}
