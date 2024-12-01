using System;
using System.Collections;
using System.Collections.Generic;
using Assets;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public enum InputState
    {
        NONE,
        USING,
        DISCARDING
    }
    
    bool acceptingInput = false;
    private static InputState _input = InputState.NONE;
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
        _input = InputState.NONE;
        if (Input.GetButtonDown("Use")) { _input = InputState.USING; }
        if (Input.GetButtonDown("Discard")) { _input = InputState.DISCARDING; }
        if (_input != InputState.NONE) { Logger.Log($"Input: {_input.ToString()}"); }

        if (acceptingInput && _input != InputState.NONE)
        {
            inputCallback?.Invoke(_input);
            acceptingInput = false;
        }
    }
    
    //-----------------------------------------------------

    private void SubscribeToEvents()
    {
        GameEngine.ShowingPropertyStartedEvent += ShowingProperty;
        GameEngine.TimedOutStartedEvent += TimedOut;
    }
    
    private void UnSubscribeToEvents()
    {
        GameEngine.ShowingPropertyStartedEvent -= ShowingProperty;
        GameEngine.TimedOutStartedEvent -= TimedOut;
    }

    protected virtual void ShowingProperty(Action<InputState> callback)
    {
        acceptingInput = true;
        inputCallback = callback;
    }

    protected virtual void TimedOut()
    {
        acceptingInput = false;
    }
}
