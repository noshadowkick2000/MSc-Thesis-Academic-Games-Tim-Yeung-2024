using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] protected Transform cameraTransform;
    
    protected Coroutine SmoothToObject(Transform goal, float duration)
    {
        Vector3 position = goal.position;
        Quaternion rotation = goal.rotation;
        return StartCoroutine(TransitionCamera(position, rotation, duration));
    }

    protected void ImmediateToObject(Transform goal)
    {
        Vector3 position = goal.position;
        Quaternion rotation = goal.rotation;
        cameraTransform.position = position;
        cameraTransform.rotation = rotation;
    }

    protected IEnumerator TransitionCamera(Vector3 position, Quaternion rotation, float duration)
    {
        Vector3 startingPos = cameraTransform.position;
        Quaternion startingRot = cameraTransform.rotation;

        float startTime = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup < startTime + duration)
        {
            float x = (Time.realtimeSinceStartup - startTime) / duration;
            cameraTransform.position = Vector3.Lerp(startingPos, position, x);
            cameraTransform.rotation = Quaternion.Lerp(startingRot, rotation, x);
            yield return null;
        }
        cameraTransform.position = position;
        cameraTransform.rotation = rotation;
    }
}
