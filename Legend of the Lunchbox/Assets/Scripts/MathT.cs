using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

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
    
    public static T[] Shuffle<T> (T[] array)
    {
        Random rng = new System.Random();
        
        int n = array.Length;
        while (n > 1) 
        {
            int k = rng.Next(n--);
            (array[n], array[k]) = (array[k], array[n]);
        }

        return array;
    }
}
