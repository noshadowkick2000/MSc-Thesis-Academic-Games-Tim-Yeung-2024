using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelHandler : MonoBehaviour
{
    public static int CurrentLevel = 0;
    public static int LastLevel = 0;

    private void Awake()
    {
        CurrentLevel = 0;
    }
}
