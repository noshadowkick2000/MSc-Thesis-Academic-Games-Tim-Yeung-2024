using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets;
using UnityEngine;

public class SelectUI : MonoBehaviour
{
    [SerializeField] private SpriteRenderer circleRenderer;
    [SerializeField] private Sprite[] circleSprites;
    [SerializeField] private Sprite[] crossSprites;
    
    private void Awake()
    {
        circleRenderer.gameObject.SetActive(false);
        
        SubscribeToEvents();
    }

    private void Update()
    {
        print(circleRenderer == null);
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    private void SubscribeToEvents()
    {
        GameEngine.EvaluatingInputStartedEvent += EvaluatingInput;
        // GameEngine.ShowingEnemyStartedEvent += ShowingEnemy;
    }

    private void UnsubscribeFromEvents()
    {
        GameEngine.EvaluatingInputStartedEvent -= EvaluatingInput;
        // GameEngine.MovingToEnemyStartedEvent += ShowingEnemy;
    }

    private void EvaluatingInput(InputHandler.InputState input)
    {
        print(circleRenderer == null);
        circleRenderer.color = Color.white;
        circleRenderer.transform.position = PropertyCameraController.PropertyCamTransform.position + (LocationHolder.PropertyLocation.position - LocationHolder.MindCameraLocation.position);
        circleRenderer.gameObject.SetActive(true);
        if (input == InputHandler.InputState.USING) 
            StartCoroutine(AnimateCircle());
        else if (input == InputHandler.InputState.DISCARDING)
            StartCoroutine(AnimateCross());
    }
    
    private IEnumerator AnimateCross()
    {
        circleRenderer.sprite = crossSprites[0];
        float startTime = Time.time;
        float x = 0;
        float y;
        Color circleColor = circleRenderer.color;

        while (x < 1)
        {
            
            int frame = Mathf.FloorToInt(x * crossSprites.Length);
            circleRenderer.sprite = crossSprites[frame];
            y = 1 - MathF.Pow(x, 16);
            circleColor.a = y;
            circleRenderer.color = circleColor;
            x = (Time.time - startTime) / (GameEngine.StaticTimeVariables.TrialEndDuration);
            
            yield return null;
        }

        circleRenderer.sprite = crossSprites.Last();
        
        circleRenderer.gameObject.SetActive(false);
    }

    private IEnumerator AnimateCircle()
    {
        circleRenderer.sprite = circleSprites[0];
        float startTime = Time.time;
        float x = 0;
        float y;
        Color circleColor = circleRenderer.color;

        while (x < 1)
        {
            
            int frame = Mathf.FloorToInt(x * circleSprites.Length);
            circleRenderer.sprite = circleSprites[frame];
            y = 1 - MathF.Pow(x, 16);
            circleColor.a = y;
            circleRenderer.color = circleColor;
            x = (Time.time - startTime) / (GameEngine.StaticTimeVariables.TrialEndDuration);
            
            yield return null;
        }

        circleRenderer.sprite = circleSprites.Last();
        
        circleRenderer.gameObject.SetActive(false);
    }
}
