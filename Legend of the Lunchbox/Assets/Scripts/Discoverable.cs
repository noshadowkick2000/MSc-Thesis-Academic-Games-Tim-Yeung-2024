using System;
using System.Collections;
using System.Collections.Generic;
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
        transform.LookAt(Camera.main.transform);
        center.localScale = Vector3.Lerp(startScale, Vector3.zero, Mathf.Pow(Mathf.PingPong(Time.realtimeSinceStartup, duration)/2, 2));
        // outer.LookAt(Camera.main.transform);
        outer.localRotation = Quaternion.Euler(eulerAngles + speed * Time.realtimeSinceStartup * Vector3.forward);
    }
}
