using System;
using Assets;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InputHandler : MonoBehaviour
{
    public enum InputState
    {
        NONE,
        USING,
        DISCARDING
    }

    private enum InputType
    {
        NONE,
        SINGLE,
        RAPID        
    }

    private InputType acceptingInput = InputType.NONE;
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

    private float inputThreshold = 1;
    public static float InputAverage;
    private float inputAdd = .1f;
    private float inputDecay = 50f;
    private void Update()
    {
        _input = InputState.NONE;
        if (TInput.GetButtonDown(TInput.ButtonNames.RIGHT)) { _input = InputState.USING; }
        if (TInput.GetButtonDown(TInput.ButtonNames.LEFT)) { _input = InputState.DISCARDING; }
        if (_input != InputState.NONE) { Logger.Log(Logger.Event.INPUT, _input == InputState.USING ? Logger.CodeTypes.INPUT_POSITIVE : Logger.CodeTypes.INPUT_NEGATIVE); }
        
        if (acceptingInput == InputType.SINGLE && _input != InputState.NONE)
        {
            inputCallback?.Invoke(_input);
            acceptingInput = InputType.NONE;
        }
        else if (acceptingInput == InputType.RAPID && _input !=InputState.NONE)
        {
            InputAverage += inputAdd;
        }
        
        InputAverage -= inputAdd / inputDecay;
        InputAverage = Mathf.Clamp(InputAverage, 0, inputThreshold);
        
        // TODO CHECK WITH PLAYERPREFS HERE FOR CHEATS

        if (TInput.GetButtonDown(TInput.ButtonNames.UP))
            Time.timeScale = 4f;
        else if (TInput.GetButtonUp(TInput.ButtonNames.UP))
            Time.timeScale = 1f;
        
        if (TInput.GetButtonDown(TInput.ButtonNames.DOWN))
            FindObjectOfType<GameEngine>().LoadNext();

        if (TInput.GetButtonDown(TInput.ButtonNames.CANCEL))
        {
            Logger.CloseLog();
            SceneManager.LoadScene(0);
        }
    }
    
    //-----------------------------------------------------

    private void SubscribeToEvents()
    {
        GameEngine.ShowingPropertyStartedEvent += ShowingProperty;
        GameEngine.TimedOutStartedEvent += TimedOut;
        GameEngine.BreakingBadStartedEvent += BreakingBad;
        GameEngine.EndingBreakStartedEvent += EndingBreak;
    }
    
    private void UnSubscribeToEvents()
    {
        GameEngine.ShowingPropertyStartedEvent -= ShowingProperty;
        GameEngine.TimedOutStartedEvent -= TimedOut;
        GameEngine.BreakingBadStartedEvent -= BreakingBad;
        GameEngine.EndingBreakStartedEvent -= EndingBreak;
    }

    private void BreakingBad()
    {
        acceptingInput = InputType.RAPID;
    }

    private void EndingBreak()
    {
        acceptingInput = InputType.NONE;
    }

    private void ShowingProperty(Action<InputState> callback)
    {
        acceptingInput = InputType.SINGLE;
        inputCallback = callback;
    }

    private void TimedOut(InputState input)
    {
        acceptingInput = InputType.NONE;
    }
}
