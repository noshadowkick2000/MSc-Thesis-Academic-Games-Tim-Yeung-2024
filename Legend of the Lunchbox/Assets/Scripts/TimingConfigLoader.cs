using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using Assets;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using UnityEngine;

public class TimingConfigLoader : MonoBehaviour
{
    [SerializeField] private string timingFileName;
    
    private void Awake()
    {
        using (StreamReader reader = new StreamReader(Application.streamingAssetsPath+"/"+timingFileName))
        using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            csv.Context.TypeConverterCache.AddConverter<float>(new UtilsT.MillisToSeconds());

            csv.Read();
            csv.ReadHeader();
            csv.Read();
            
            GameEngine.StaticTimeVariables = csv.GetRecord<GameEngine.TimingData>();
        }
        
        print(GameEngine.StaticTimeVariables.EncounterStartDuration);
    }
}
