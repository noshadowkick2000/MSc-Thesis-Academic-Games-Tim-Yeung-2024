using System.Collections;
using Assets;
using UnityEngine;
using UnityEngine.UI;

public class Progressbar : MonoBehaviour
{
    [SerializeField] private Slider progressSlider;
    [SerializeField] private Transform bGoal;

    [SerializeField] private Vector3 baseSize = .5f * Vector3.one;
    [SerializeField] private Vector3 goalSize = Vector3.one;

    private float total;
    private float progress;
    
    private void Start()
    {
        total = FindObjectOfType<TrialHandler>().GetTotalBlockDelay();
        SubscribeToEvents();
        
        bGoal.localScale = baseSize;
    }

    private void OnDestroy()
    {
        UnsubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
        GameEngine.OnRailStartedEvent += OnRail;
    }

    private void UnsubscribeToEvents()
    {
        GameEngine.OnRailStartedEvent -= OnRail;
    }

    private void OnRail()
    {
        StartCoroutine(MoveSlider());
    }

    private IEnumerator MoveSlider()
    {
        float startTime = Time.time;
        float x = 0;
        float startProgress = progress;
        float addedProgress = GameEngine.CurrentRailDuration / total;
            
        while (x < 1)
        {
            x = (Time.time - startTime) / GameEngine.CurrentRailDuration;
            float y = x * addedProgress;

            progress = startProgress + y;
            progressSlider.value = progress;
            bGoal.localScale = Vector3.Lerp(baseSize, goalSize, progress);
            
            yield return null;
        }

        progress = startProgress + addedProgress;
    }
}
