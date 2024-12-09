using System;
using System.Collections;
using System.Collections.Generic;
using Assets;
using UnityEngine;

public class Potion : ObjectMover
{
    [SerializeField] private Transform cap;
    [SerializeField] private Liquid potionLiquid;
    [SerializeField] private float drainGoal = .65f;
    // [SerializeField] private float goal = .5f;

    private void Awake()
    {
        mainObject = cap;
    }

    public void RemoveCap()
    {
        StartCoroutine(DrainLiquid());
        SmoothToObject(cap.position + cap.up, Quaternion.identity, GameEngine.WonBreakTime / 2f, true);
    }

    private IEnumerator DrainLiquid()
    {
        float startTime = Time.realtimeSinceStartup;
        float x = 0;
        float startAmount = potionLiquid.fillAmount;

        while (x < 1)
        {
            potionLiquid.fillAmount = Mathf.Lerp(startAmount, drainGoal, x);
            
            x = (Time.realtimeSinceStartup - startTime) / GameEngine.WonBreakTime;
            yield return null;
        }
    }
}
