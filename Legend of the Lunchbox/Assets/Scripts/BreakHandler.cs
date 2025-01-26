using Assets;
using UnityEngine;

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
    
    private void StartingBreak()
    {
        mainObject.gameObject.SetActive(true);
        mainObject.position = LocationHolder.PropertyLocation.position;
        mainObject.rotation = Quaternion.identity;

        StartCoroutine(GrowObject(GameEngine.StaticTimeVariables.EncounterDiscoverableDuration/4f, 0, 1, false));
    }

    bool breaking = false;
    private void BreakingBad()
    {
        StartingBreak();
        breaking = true;
    }


    private void WonBreak()
    {
        breaking = false;
        print("WON");
        p.RemoveCap();
        SmoothToObject(LocationHolder.PropertyLocation.position, Quaternion.Euler(60, 0, 0), GameEngine.StaticTimeVariables.BreakFeedbackDuration/2, true);
    }

    private void EndingBreak()
    {
        SmoothToObject(mainObject.position + Vector3.down, mainObject.rotation, GameEngine.StaticTimeVariables.EncounterEndDuration, true);
    }

    private void OnRail()
    {
        mainObject.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!breaking) return;

        float rotation = (Mathf.PingPong(Time.time * 10f, 2) - 1f) * (60f * InputHandler.InputAverage);
        
        mainObject.position = Vector3.Lerp(LocationHolder.PropertyLocation.position + new Vector3(rotation * .002f, 0, 0), LocationHolder.MindCameraLocation.position, InputHandler.InputAverage / 2);
        mainObject.rotation = Quaternion.Euler(0, 0, rotation);
    }
}
