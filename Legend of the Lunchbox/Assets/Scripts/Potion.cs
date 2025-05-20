using System.Collections;
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
        SmoothToObject(cap.position + cap.up, Quaternion.identity, GameEngine.StaticTimeVariables.BreakFeedbackDuration / 2f, true);
    }

    private IEnumerator DrainLiquid()
    {
        float startTime = Time.time;
        float x = 0;
        float startAmount = potionLiquid.fillAmount;

        while (x < 1)
        {
            potionLiquid.fillAmount = Mathf.Lerp(startAmount, drainGoal, x);
            
            x = (Time.time - startTime) / GameEngine.StaticTimeVariables.BreakFeedbackDuration;
            yield return null;
        }
    }
}
