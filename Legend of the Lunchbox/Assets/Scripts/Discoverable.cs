using UnityEngine;

public class Discoverable : MonoBehaviour
{
    [SerializeField] private Transform center;
    [SerializeField] private Transform outer;
    [SerializeField] private float speed;
    [SerializeField] private float duration;

    private Vector3 startScale;
    private Vector3 eulerAngles;
    
    private void Awake()
    {
        startScale = center.localScale;
        eulerAngles = outer.localEulerAngles;
    }

    void Update()
    {
        center.localScale = Vector3.Lerp(startScale, Vector3.zero, Mathf.Pow(Mathf.PingPong(Time.time, duration)/2, 2));
        outer.localRotation = Quaternion.Euler(eulerAngles + speed * Time.time * Vector3.forward);
        center.LookAt(Camera.main.transform);
    }
}
