using System;
using System.Collections;
using System.Collections.Generic;
using Assets;
using UnityEngine;
using UnityEngine.UI;

public class BreakHandler : ObjectMover
{
    [SerializeField] private GameObject potion;
    
    private void Awake()
    {
        mainObject = Instantiate(potion, LocationHolder.PropertyLocation.position, Quaternion.identity).transform;
        p = mainObject.GetComponent<Potion>();
        mainObject.gameObject.SetActive(false);
        
        SubscribeToEvents();
    }

    private void OnDestroy()
    {
        UnsubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
        // GameEngine.StartingBreakStartedEvent += StartingBreak;
        GameEngine.BreakingBadStartedEvent += BreakingBad;
        GameEngine.WonBreakStartedEvent += WonBreak;
        GameEngine.EndingBreakStartedEvent += EndingBreak;
        GameEngine.OnRailStartedEvent += OnRail;
    }

    private void UnsubscribeToEvents()
    {
        // GameEngine.StartingBreakStartedEvent -= StartingBreak;
        GameEngine.BreakingBadStartedEvent -= BreakingBad;
        GameEngine.WonBreakStartedEvent -= WonBreak;
        GameEngine.EndingBreakStartedEvent -= EndingBreak;
        GameEngine.OnRailStartedEvent -= OnRail;
    }

    private Potion p;
    
    protected virtual void StartingBreak()
    {
        mainObject.gameObject.SetActive(true);
        mainObject.position = LocationHolder.PropertyLocation.position;
        mainObject.rotation = Quaternion.identity;

        StartCoroutine(GrowObject(GameEngine.EnemyShowTime/4f, 0, 1, false));
    }

    bool breaking = false;
    protected virtual void BreakingBad()
    {
        StartingBreak();
        breaking = true;
    }


    protected virtual void WonBreak()
    {
        breaking = false;
        print("WON");
        p.RemoveCap();
        SmoothToObject(LocationHolder.PropertyLocation.position, Quaternion.Euler(60, 0, 0), GameEngine.WonBreakTime/2, true);
    }

    protected virtual void EndingBreak()
    {
        SmoothToObject(mainObject.position + Vector3.down, mainObject.rotation, GameEngine.PlayerReset, true);
    }

    protected virtual void OnRail()
    {
        mainObject.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!breaking) return;
        
        mainObject.position = Vector3.Lerp(LocationHolder.PropertyLocation.position, LocationHolder.MindCameraLocation.position, InputHandler.InputAverage / 2);
        mainObject.rotation = Quaternion.Euler(0, 0, (Mathf.PingPong(Time.realtimeSinceStartup * 10f, 2) - 1f) * (60f * InputHandler.InputAverage));
    }
}
