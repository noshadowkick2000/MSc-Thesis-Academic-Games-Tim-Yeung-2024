using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bob : MonoBehaviour
{
    private Coroutine bobRoutine;
    private void Awake()
    {
        bobRoutine = StartCoroutine(Bobbing());
    }

    private void OnDestroy()
    {
        StopCoroutine(bobRoutine);
    }

    private readonly float duration = .5f;
    private IEnumerator Bobbing()
    {
        RectTransform rt = transform as RectTransform;
        while (true)
        {
            float startTime = Time.realtimeSinceStartup;
            float x = 0;
            
            while (x < 1)
            {
                x = (Time.realtimeSinceStartup - startTime) / duration;
                float y = Mathf.Sin(x * 2 * MathF.PI);
                rt.anchoredPosition = new Vector2(0, y);
                yield return null;
            }
        }
    }
}
