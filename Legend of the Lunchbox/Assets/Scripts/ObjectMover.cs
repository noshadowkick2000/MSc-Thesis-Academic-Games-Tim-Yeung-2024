using System.Collections;
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
    
    protected IEnumerator Wiggle(float duration, float maxAngle, float repetitions)
    {
        Quaternion baseRot = mainObject.rotation;
      
        float startTime = Time.time;
        float x = 0;

        while (x < 1)
        {
            float y = Mathf.Sin((UtilsT.EasedT(x) * repetitions * Mathf.PI));
            mainObject.rotation = Quaternion.Euler(0, 0, maxAngle * y) * baseRot;
            x = (Time.time - startTime) / duration;
            yield return null;
        }
      
        mainObject.rotation = baseRot;
    }

    protected IEnumerator Nod(float duration, float maxOffset, float repetitions, bool vertical)
    {
        Vector3 start = mainObject.position;

        float startTime = Time.time;
        float x = 0;
        
        while (x < 1)
        {
            float y = Mathf.Sin((UtilsT.EasedT(x) * repetitions * Mathf.PI));
            mainObject.position = start + new Vector3(vertical ? 0 : maxOffset * y, vertical ? maxOffset * y : 0, 0);
            x = (Time.time - startTime) / duration;
            yield return null;
        }
    }
    
    protected IEnumerator GrowObject(float duration, float startingScaleMultiplier, float scaleMultiplier, bool destroy)
    {
        Vector3 endScale = scaleMultiplier * mainObject.localScale;
        Vector3 startScale = startingScaleMultiplier * mainObject.localScale;
        mainObject.localScale = startScale;
    
        float startTime = Time.time;
        while (Time.time < startTime + (duration))
        {
            float x = (Time.time - startTime) / (duration);
            float y = UtilsT.EasedT(x);
            mainObject.localScale = Vector3.Lerp(startScale, endScale, y);

            yield return null;
        }
    
        mainObject.localScale = endScale;

        if (!destroy) yield break;
        
        Destroy(mainObject.gameObject);
        mainObject = null;
    }

    private IEnumerator TransitionObject(Vector3 position, Quaternion rotation, float duration, bool ease)
    {
        Vector3 startingPos = mainObject.position;
        Quaternion startingRot = mainObject.rotation;

        float startTime = Time.time;
        while (Time.time < startTime + duration)
        {
            float x = (Time.time - startTime) / duration;
            if (ease)
                x = UtilsT.EasedT(x);
                // x = x < .5 ? 16 * x * x * x * x * x : 1 - Mathf.Pow(-2 * x + 2, 5) / 2;
            mainObject.position = Vector3.Lerp(startingPos, position, x);
            mainObject.rotation = Quaternion.Lerp(startingRot, rotation, x);
            yield return null;
        }
        mainObject.position = position;
        mainObject.rotation = rotation;
    }
}
