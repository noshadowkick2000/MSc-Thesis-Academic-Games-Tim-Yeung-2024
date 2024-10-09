using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class EncounterHandler : MonoBehaviour
{
  private Encounter[] encounters;
  private float timer;

  /// <summary>
  /// Generate new encounter and return time until next encounter
  /// </summary>
  /// <returns>(float) time until next encounter</returns>
  public float GenerateNewEncounter()
  {
    return 0f;
  }
}
