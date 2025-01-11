using System;
using System.Collections;
using System.Collections.Generic;
using Assets;
using UnityEngine;
using UnityEngine.UI;

public class FeedbackUI : MonoBehaviour
{
    [SerializeField] private GameObject feedbackUI;
    [SerializeField] private Animation feedbackAnimation;
    [SerializeField] private RectTransform burstLines;

    private void Awake()
    {
        feedbackUI.SetActive(false);
        
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
        GameEngine.ShowingEnemyInMindStartedEvent += RemoveFeedback;
        GameEngine.EvaluatingEncounterStartedEvent += RemoveFeedback;
    }
    
    private void UnSubscribeToEvents()
    {
        GameEngine.AnswerCorrectStartedEvent -= ShowPositiveFeedback;
        GameEngine.AnswerWrongStartedEvent -= ShowNegativeFeedback;
        GameEngine.ShowingEnemyInMindStartedEvent -= RemoveFeedback;
        GameEngine.EvaluatingEncounterStartedEvent -= RemoveFeedback;
    }
}
