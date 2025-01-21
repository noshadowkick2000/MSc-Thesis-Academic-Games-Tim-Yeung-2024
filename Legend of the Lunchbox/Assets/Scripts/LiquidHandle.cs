using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LiquidHandle : MonoBehaviour
{
    [SerializeField] private RawImage[] waves;
    [SerializeField] private float margin = .05f;
    [SerializeField] private float waveHeight = .8f;
    [SerializeField] private float offset = 1f;
    
    private void Update()
    {
        float counter = 0;
        foreach (var wave in waves)
        {
            float timeVal = 0.2f * (Time.time - counter);
            wave.uvRect = new Rect(timeVal, 0, wave.uvRect.width, Mathf.PingPong(timeVal, 2 * margin) + (waveHeight - margin));
            counter += offset;
        }
    }
}
