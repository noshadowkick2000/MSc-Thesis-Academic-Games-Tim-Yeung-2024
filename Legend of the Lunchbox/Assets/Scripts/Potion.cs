using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : ObjectMover
{
    [SerializeField] private Transform cap;
    [SerializeField] private float goal = .5f;

    
    private float start;
    private float t;

    private void Awake()
    {
        start = cap.localPosition.y;
    }

    public void SetCap(float t)
    {
        this.t = t;
    }

    private void Update()
    {
        cap.localPosition = new Vector3(cap.localPosition.x, Mathf.Lerp(start, goal, t), cap.localPosition.z);
    }
}
