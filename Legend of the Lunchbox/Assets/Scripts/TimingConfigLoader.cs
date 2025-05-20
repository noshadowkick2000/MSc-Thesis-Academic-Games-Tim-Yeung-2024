using System.Globalization;
using System.IO;
using Assets;
using CsvHelper;
using UnityEngine;

public class TimingConfigLoader : MonoBehaviour
{
    public static readonly string TimingFileName = "LOTL_timing.csv";
    
    private void Awake()
    {
        using (StreamReader reader = new StreamReader(Application.streamingAssetsPath+"/"+TimingFileName))
        using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            csv.Context.TypeConverterCache.AddConverter<float>(new UtilsT.MillisToSeconds());

            csv.Read();
            csv.ReadHeader();
            csv.Read();
            
            GameEngine.StaticTimeVariables = csv.GetRecord<GameEngine.TimingData>();
        }
        
        // print(GameEngine.StaticTimeVariables.EncounterStartDuration);
    }
}
