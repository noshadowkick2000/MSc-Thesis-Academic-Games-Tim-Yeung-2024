using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImportTester : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    
    // Start is called before the first frame update
    void Start()
    {
        Invoke(nameof(Test), 2f);
    }

    private void Test()
    {
        var test = ExternalAssetLoader.GetAsset(UtilsT.GetId("hammer"));

        Instantiate(test, transform);
    }
}
