using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
  public void SmoothToObject(Transform goal, float duration)
  {
    Vector3 position = goal.position;
    Quaternion rotation = goal.rotation;
    StartCoroutine(TransitionCamera(position, rotation, duration));
  }

  public void ImmediateToObject(Transform goal)
  {
    Vector3 position = goal.position;
    Quaternion rotation = goal.rotation;
    transform.position = position;
    transform.rotation = rotation;
  }

  private IEnumerator TransitionCamera(Vector3 position, Quaternion rotation, float duration)
  {
    Vector3 startingPos = transform.position;
    Quaternion startingRot = transform.rotation;

    float startTime = Time.realtimeSinceStartup;
    while (Time.realtimeSinceStartup < startTime + duration)
    {
      float x = (Time.realtimeSinceStartup - startTime) / duration;
      transform.position = Vector3.Lerp(startingPos, position, x);
      transform.rotation = Quaternion.Lerp(startingRot, rotation, x);
      yield return null;
    }
    transform.position = position;
    transform.rotation = rotation;
  }
}
