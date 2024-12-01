using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets;
using UnityEngine;

public class CircleUI : MonoBehaviour
{
    [SerializeField] private SpriteRenderer circleRenderer;
    [SerializeField] private Sprite[] circleSprites;
    
    private void Awake()
    {
        circleRenderer.gameObject.SetActive(false);
        
        SubscribeToEvents();
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
        GameEngine.EvaluatingInputStartedEvent += EvaluatingInput;
        // GameEngine.MovingToEnemyStartedEvent += ShowingEnemy;
    }

    protected virtual void EvaluatingInput(InputHandler.InputState input)
    {
        if (input != InputHandler.InputState.USING) return;
        circleRenderer.gameObject.SetActive(true);
        StartCoroutine(AnimateCircle());
    }

    private IEnumerator AnimateCircle()
    {
        circleRenderer.sprite = circleSprites[0];
        float startTime = Time.realtimeSinceStartup;
        float x = 0;

        while (x < 1)
        {
            int frame = Mathf.FloorToInt(x * circleSprites.Length);
            circleRenderer.sprite = circleSprites[frame];
            
            x = (Time.realtimeSinceStartup - startTime) * GameEngine.MindPropertyTransitionTime;
            yield return null;
        }

        circleRenderer.sprite = circleSprites.Last();
        
        circleRenderer.gameObject.SetActive(false);
    }
}
