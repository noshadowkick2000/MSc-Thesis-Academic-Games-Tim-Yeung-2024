using System.Collections.Generic;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using UnityEngine;
using Random = System.Random;

public class UtilsT : MonoBehaviour
{
    private static Dictionary<int, float> _fibValues = new Dictionary<int, float>{{0, 0}, {1, 1}};

    public static float Fibonacci(int n)
    {
        if (!_fibValues.ContainsKey(n))
            _fibValues[n] = Fibonacci(n - 1) + Fibonacci(n - 2);

        return _fibValues[n];
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
    
    public static int GetId(string objectName)
    {
        return (objectName.Aggregate(0, (current, c) => (current * 31) + c));
    }
    
    public class MillisToSeconds : DefaultTypeConverter
    {
        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            if (int.TryParse(text, out int intValue))
            {
                return intValue / 1000f;
            }
            throw new CsvHelperException(row.Context, $"Cannot convert '{text}' to float.");
        }
    }
}
