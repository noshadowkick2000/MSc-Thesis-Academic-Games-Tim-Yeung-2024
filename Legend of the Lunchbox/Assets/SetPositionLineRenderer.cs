using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetPositionLineRenderer : MonoBehaviour
{
    LineRenderer lineRenderer;

    private void Awake()
    {
        lineRenderer = GetComponentInChildren<LineRenderer>();
    }

    private float x;
    private float y;
    private float z;
    public void SetX(float x)
    {
        this.x = x;
    }

    public void SetY(float y)
    {
        this.y = y;
    }

    public void SetZ(float z)
    {
        this.z = z;
    }

    public void SetPosition()
    {
        lineRenderer.SetPosition(1, new Vector3(x, y, z));
    }
}
