using System;
using System.Collections;
using System.Collections.Generic;
using Assets;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public enum InputState
    {
        None,
        Using,
        Discarding
    }
    
    bool acceptingInput = false;
    private static InputState input = InputState.None;
    private Action<InputState> inputCallback;
        
    private void Awake()
    {
        SubscribeToEvents();
    }

    private void OnDestroy()
    {
        UnSubscribeToEvents();
    }
    
    private void Update()
    {
        input = InputState.None;
        if (Input.GetButtonDown("Use")) { input = InputState.Using; }
        if (Input.GetButtonDown("Discard")) { input = InputState.Discarding; }
        if (input != InputState.None) { Logger.Log($"Input: {input.ToString()}"); }

        if (acceptingInput && input != InputState.None)
        {
            inputCallback?.Invoke(input);
        }
    }
    
    //-----------------------------------------------------

    private void SubscribeToEvents()
    {
        GameEngine.ShowingPropertyStartedEvent += ShowingProperty;
        GameEngine.EvaluatingInputStartedEvent += EvaluatingInput;
        GameEngine.TimedOutStartedEvent += TimedOut;
    }
    
    private void UnSubscribeToEvents()
    {
        GameEngine.ShowingPropertyStartedEvent -= ShowingProperty;
        GameEngine.EvaluatingInputStartedEvent -= EvaluatingInput;
        GameEngine.TimedOutStartedEvent -= TimedOut;
    }

    protected virtual void ShowingProperty(Action<InputState> callback)
    {
        acceptingInput = true;
        inputCallback = callback;
    }

    protected virtual void EvaluatingInput()
    {
        acceptingInput = false;
    }

    protected virtual void TimedOut()
    {
        acceptingInput = false;
    }
}
