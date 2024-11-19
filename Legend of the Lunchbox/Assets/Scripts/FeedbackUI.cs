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
    }

    private void ShowNegativeFeedback()
    {
        feedbackUI.SetActive(true);
        feedbackAnimation.Play("feedbackIncorrect");
    }

    private void RemoveFeedback(bool boolean)
    {
        feedbackUI.SetActive(false);
    }
    
    //-----------------------------------------------------

    private void SubscribeToEvents()
    {
        GameEngine.AnswerCorrectStartedEvent += ShowPositiveFeedback;
        GameEngine.AnswerWrongStartedEvent += ShowNegativeFeedback;
        GameEngine.ThinkingOfPropertyStartedEvent += RemoveFeedback;
    }
    
    private void UnSubscribeToEvents()
    {
        GameEngine.AnswerCorrectStartedEvent -= ShowPositiveFeedback;
        GameEngine.AnswerWrongStartedEvent -= ShowNegativeFeedback;
        GameEngine.ThinkingOfPropertyStartedEvent -= RemoveFeedback;
    }
}
