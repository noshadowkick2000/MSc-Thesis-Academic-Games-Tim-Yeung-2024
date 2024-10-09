using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

namespace Assets
{
  public class GameEngine : MonoBehaviour
  {
    public enum GameState
    {
      CUTSCENE,
      ONRAIL,
      STARTINGENCOUNTER,
      SHOWINGENEMY,
      SHOWINGBAG,
      PULLINGOOBJECT,
      EVALUATINGINPUT,
      DAMAGINGPLAYER,
      DAMAGINGENEMY,
      ENDINGENCOUNTER
    }

    private GameState state = GameState.CUTSCENE;
    private Logger logger;

    private void Awake()
    {
      logger = GetComponent<Logger>();
      StateChange(GameState.CUTSCENE);
    }


    public void StateChange(GameState newState)
    {
      state = newState;
      logger.Log($"State changed to {nameof(state)}");

      switch (state)
      {
        case GameState.CUTSCENE:
          break;
        case GameState.ONRAIL:
          StartOnRail();
          break;
        case GameState.STARTINGENCOUNTER:
          StartEncounter();
          break;
        case GameState.SHOWINGENEMY:
          break;
        case GameState.SHOWINGBAG:
          break;
        case GameState.PULLINGOOBJECT:
          break;
        case GameState.EVALUATINGINPUT:
          break;
        case GameState.DAMAGINGPLAYER:
          break;
        case GameState.DAMAGINGENEMY:
          break;
        case GameState.ENDINGENCOUNTER:
          break;
      }
    }

    private int playerHealth;
    private int friendHealth;
    public void DamagePlayer()
    {
      if (friendHealth > 0)
        friendHealth--;
      else
        playerHealth--;

      //TODO EVAL PLAYER DIE ETC
    }

    private IEnumerator Timer(float duration, GameState nextState)
    {
      yield return new WaitForSecondsRealtime(duration);

      StateChange(nextState);
    }

    private void StartOnRail()
    {
      float duration = Random.Range(2.0f, 8.0f);

      StartCoroutine(Timer(duration, GameState.STARTINGENCOUNTER));
    }

    private void StartEncounter()
    {
      
    }
  }
}