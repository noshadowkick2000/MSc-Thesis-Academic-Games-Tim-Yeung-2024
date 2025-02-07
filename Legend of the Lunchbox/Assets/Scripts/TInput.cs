using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class TInput : MonoBehaviour
{
    public enum ButtonNames
    {
        LEFT,
        RIGHT,
        UP,
        DOWN,
        SUBMIT,
        CANCEL,
        FMRI
    }
    
    [Serializable]
    public class InputButton
    {
        [FormerlySerializedAs("ButtonName")] public ButtonNames buttonName;
        [FormerlySerializedAs("AssignedKeyCode")] public KeyCode assignedKeyCode = KeyCode.None;
        // public bool IsUp = false;
    }

    [FormerlySerializedAs("buttons")] [SerializeField] private InputButton[] baseButtons;
    public static InputButton[] Buttons; 

    private void Awake()
    {
        Buttons = baseButtons;
        
        foreach (var button in Buttons)
        {
            if (PlayerPrefs.HasKey(button.buttonName.ToString()))
                button.assignedKeyCode = (KeyCode)PlayerPrefs.GetInt(button.buttonName.ToString());
            else 
                PlayerPrefs.SetInt(button.buttonName.ToString(), (int) button.assignedKeyCode);
        }
    }

    // private void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.Space))
    //         StartInputMap(ButtonNames.CANCEL, null);
    // }

    public static bool GetButtonDown(ButtonNames buttonName)
    {
        return Input.GetKeyDown(GetButton(buttonName).assignedKeyCode);
    }    
    
    public static bool GetButtonUp(ButtonNames buttonName)
    {
        return Input.GetKeyUp(GetButton(buttonName).assignedKeyCode);
    }

    public void StartInputMap(ButtonNames buttonName, Action<ButtonNames, KeyCode> inputDetectedCallback)
    {
        print("awaiting input");
        StartCoroutine(WaitForInput(buttonName, 5, inputDetectedCallback));
    }

    private IEnumerator WaitForInput(ButtonNames buttonName, float timeOut, Action<ButtonNames, KeyCode> inputDetectedCallback)
    {
        yield return null;
        
        KeyCode keyCode = KeyCode.None;
        float endTime = Time.time + timeOut;
        
        while (keyCode == KeyCode.None && Time.time < endTime)
        {
            keyCode = DetectKey();
            yield return null;
        }

        if (keyCode == KeyCode.None) yield break;

        SetAssignedKey(buttonName, keyCode);
        
        inputDetectedCallback?.Invoke(buttonName, keyCode);
    }

    private void SetAssignedKey(ButtonNames buttonName, KeyCode assignedKeyCode)
    {
        print($"setting {buttonName.ToString()} to {assignedKeyCode.ToString()}");
        GetButton(buttonName).assignedKeyCode = assignedKeyCode;
        PlayerPrefs.SetInt(buttonName.ToString(), (int)assignedKeyCode);
    }

    private static InputButton GetButton(ButtonNames buttonName)
    {
        foreach (var button in Buttons)
        {
            if (button.buttonName == buttonName)
            {
                return button;
            }
        }
        
        throw new Exception("Button not found");
    }

    private static KeyCode DetectKey()
    {
        for (int i = 0; i < Enum.GetNames(typeof(KeyCode)).Length; i++)
        {
            if (Input.GetKeyDown((KeyCode)i))
                return (KeyCode) i;
        }

        return KeyCode.None;
    }
}
