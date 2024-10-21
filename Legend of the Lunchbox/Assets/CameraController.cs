using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
  [SerializeField] private Vector3 objectOffset;

  public void SmoothToObject(Transform showedObject, float duration)
  {
    StartCoroutine(TransitionCamera(showedObject, duration));
  }

  public void ImmediateToObject(Transform showedObject)
  {
    transform.position = showedObject.position + objectOffset;
  }

  private IEnumerator TransitionCamera(Transform showedObject, float duration)
  {
    Vector3 startingPos = transform.position;
    Vector3 goalPos = showedObject.position + objectOffset;
    float startTime = Time.realtimeSinceStartup;
    while (Time.realtimeSinceStartup < startTime + duration)
    {
      float x = (Time.realtimeSinceStartup - startTime) / duration;
      transform.position = Vector3.Lerp(startingPos, goalPos, x);
      yield return null;
    }
    transform.position = goalPos;
    //transform.LookAt(showedObject.position, Vector3.up);
  }
}
