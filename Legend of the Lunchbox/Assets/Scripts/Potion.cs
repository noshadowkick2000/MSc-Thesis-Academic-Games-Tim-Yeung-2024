using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : ObjectMover
{
    [SerializeField] private Transform cap;
    [SerializeField] private float goal = .5f;

    private void Awake()
    {
        mainObject = cap;
    }

    public void RemoveCap()
    {
        SmoothToObject(cap.position + 2 * Vector3.up, Quaternion.identity, .2f, true);
    }
}
