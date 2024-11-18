using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fibonacci : MonoBehaviour
{
    private static Dictionary<int, float> fibValues = new Dictionary<int, float>{{0, 0}, {1, 1}};

    public static float CalculateUsingDynamicProgramming(int n)
    {
        if (!fibValues.ContainsKey(n))
            fibValues[n] = CalculateUsingDynamicProgramming(n - 1) + CalculateUsingDynamicProgramming(n - 2);

        return fibValues[n];
    }
}
