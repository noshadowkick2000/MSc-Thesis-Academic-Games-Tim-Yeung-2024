using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ObjectMover : MonoBehaviour
{
    [FormerlySerializedAs("cameraTransform")] [SerializeField] protected Transform mainObject;
    
    protected Coroutine SmoothToObject(Transform goal, float duration, bool ease)
    {
        Vector3 position = goal.position;
        Quaternion rotation = goal.rotation;
        return StartCoroutine(TransitionObject(position, rotation, duration, ease));
    }
    
    protected Coroutine SmoothToObject(Vector3 goalPosition, Quaternion goalRotation, float duration, bool ease)
    {
        Vector3 position = goalPosition;
        Quaternion rotation = goalRotation;
        return StartCoroutine(TransitionObject(position, rotation, duration, ease));
    }

    protected void ImmediateToObject(Transform goal)
    {
        Vector3 position = goal.position;
        Quaternion rotation = goal.rotation;
        mainObject.position = position;
        mainObject.rotation = rotation;
    }
    
    protected void ImmediateToObject(Vector3 goalPos, Quaternion goalRot)
    {
        mainObject.position = goalPos;
        mainObject.rotation = goalRot;
    }

    protected IEnumerator TransitionObject(Vector3 position, Quaternion rotation, float duration, bool ease)
    {
        Vector3 startingPos = mainObject.position;
        Quaternion startingRot = mainObject.rotation;

        float startTime = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup < startTime + duration)
        {
            float x = (Time.realtimeSinceStartup - startTime) / duration;
            if (ease)
                x = MathT.EasedT(x);
                // x = x < .5 ? 16 * x * x * x * x * x : 1 - Mathf.Pow(-2 * x + 2, 5) / 2;
            mainObject.position = Vector3.Lerp(startingPos, position, x);
            mainObject.rotation = Quaternion.Lerp(startingRot, rotation, x);
            yield return null;
        }
        mainObject.position = position;
        mainObject.rotation = rotation;
    }
}
