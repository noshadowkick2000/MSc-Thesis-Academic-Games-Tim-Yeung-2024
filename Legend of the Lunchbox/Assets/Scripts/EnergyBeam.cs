using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class EnergyBeam : MonoBehaviour
{
    [SerializeField] private Material beamMaterial;

    private void Update()
    {
        beamMaterial.mainTextureOffset = new Vector2(Time.time, 5 * Time.time);
    }
}
