using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickerLight : MonoBehaviour
{
    [SerializeField] private Light light;
    private float start;
    
    // Start is called before the first frame update
    void Start()
    {
        start = Random.Range(0f, 10f);
    }

    // Update is called once per frame
    void Update()
    {
        start += Time.deltaTime;
        light.intensity = Mathf.PerlinNoise1D(start);
    }
}
