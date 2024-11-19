using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathT : MonoBehaviour
{
    private static Dictionary<int, float> fibValues = new Dictionary<int, float>{{0, 0}, {1, 1}};

    public static float Fibonacci(int n)
    {
        if (!fibValues.ContainsKey(n))
            fibValues[n] = Fibonacci(n - 1) + Fibonacci(n - 2);

        return fibValues[n];
    }
    
    public static float EasedT(float t)
    {
        return t < .5 ? 4 * t * t * t : 1 - Mathf.Pow(-2 * t + 2, 3) / 2;;
    }
}
