using System;
using System.Collections;
using System.Collections.Generic;
using Assets;
using UnityEngine;

public class Runner : MonoBehaviour
{
    [SerializeField] private Material runnerMaterial;

    private void Awake()
    {
        GameEngine.OnRailStartedEvent += OnRail;
    }

    private void OnDestroy()
    {
        GameEngine.OnRailStartedEvent -= OnRail;
    }

    private void OnRail()
    {
        StartCoroutine(MoveLine());
    }
    
    private IEnumerator MoveLine()
    {
        float startTime = Time.time;
        float x = 0;
        float startProgress = runnerMaterial.GetTextureOffset("_MainTex").x;
            
        while (x < 1)
        {
            x = (Time.time - startTime) / GameEngine.CurrentRailDuration;

            startProgress += Time.deltaTime;
            
            runnerMaterial.SetTextureOffset("_MainTex", new Vector2(startProgress, 0));
            
            yield return null;
        }
    }
}
