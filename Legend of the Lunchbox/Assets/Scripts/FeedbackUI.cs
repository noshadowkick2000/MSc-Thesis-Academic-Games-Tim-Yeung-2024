using System;
using System.Collections;
using System.Collections.Generic;
using Assets;
using UnityEngine;
using UnityEngine.UI;

public class FeedbackUI : ObjectMover
{
    [SerializeField] private GameObject feedbackUI;
    [SerializeField] private Animation feedbackAnimation;
    [SerializeField] private RectTransform burstLines;

    private void Awake()
    {
        feedbackUI.SetActive(false);
        mainObject = feedbackUI.transform;
        
        if (PlayerPrefs.GetInt(MainMenuHandler.FeedbackKey) == 1)
            SubscribeToEvents();
    }

    private void OnDestroy()
    {
        UnSubscribeToEvents();
    }

    private void ShowPositiveFeedback()
    {
        feedbackUI.SetActive(true);
        feedbackAnimation.Play("feedbackCorrect");

        StartCoroutine(PlayPositive());
    }

    private IEnumerator PlayPositive()
    {
        float startTime = Time.realtimeSinceStartup;
        float x = 0;
        float y;

        while (x < 1)
        {
            y = UtilsT.EasedT(x) * 4f;
            burstLines.localScale = new Vector3(y, y, y);
            x = (Time.realtimeSinceStartup - startTime) / GameEngine.StaticTimeVariables.TrialFeedbackDuration;

            yield return null;
        }
    }

    private void ShowNegativeFeedback()
    {
        feedbackUI.SetActive(true);
        StartCoroutine(Nod(GameEngine.StaticTimeVariables.TrialFeedbackDuration/2, .1f, 20, false));
        
        feedbackAnimation.Play("feedbackIncorrect");
    }

    private void RemoveFeedback()
    {
        feedbackUI.SetActive(false);
    }
    
    //-----------------------------------------------------

    private void SubscribeToEvents()
    {
        GameEngine.AnswerCorrectStartedEvent += ShowPositiveFeedback;
        GameEngine.AnswerWrongStartedEvent += ShowNegativeFeedback;
        GameEngine.ObjectDelayStartedEvent += RemoveFeedback;
        GameEngine.EvaluatingEncounterStartedEvent += RemoveFeedback;
    }
    
    private void UnSubscribeToEvents()
    {
        GameEngine.AnswerCorrectStartedEvent -= ShowPositiveFeedback;
        GameEngine.AnswerWrongStartedEvent -= ShowNegativeFeedback;
        GameEngine.ObjectDelayStartedEvent -= RemoveFeedback;
        GameEngine.EvaluatingEncounterStartedEvent -= RemoveFeedback;
    }
}
