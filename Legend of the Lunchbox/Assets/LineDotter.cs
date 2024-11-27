using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineDotter : MonoBehaviour
{
    private Material mat;
    
    // Start is called before the first frame update
    void Start()
    {
        mat = GetComponent<LineRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        mat.mainTextureOffset = new Vector2(Time.time, 0);
    }
}
