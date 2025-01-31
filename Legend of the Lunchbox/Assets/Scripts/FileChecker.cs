using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Assets;
using CsvHelper;
using TMPro;
using UnityEngine;

public class FileChecker : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMesh;
    
    // check timing file for missing columns
    // check that all values are larger than 0 and numerical
    
    // check that there are at least 2 columns in locale
    // check that there are at least 54 rows (including header)
    
    // check for the trial that all columns are present and for every entry that the values are valid
    // check that there is at least 4 interactions and a break in level 0
    // check for each non-word asset that there is a corresponding asset

    public void CheckFiles()
    {
        try
        {
            CheckTimingFile();
            CheckLocalizationFile();
            CheckTrialFile();

            textMesh.text = "File check completed successfully, no errors found";
        }
        catch (Exception e)
        {
            // Console.WriteLine(e);
            textMesh.text = e.Message;
            throw;
        }
    }

    private void CheckTimingFile()
    {
        if (!File.Exists(Application.streamingAssetsPath + "/" + TimingConfigLoader.TimingFileName))
            throw new Exception("Timing file not found");
        
        using (StreamReader reader = new StreamReader(Application.streamingAssetsPath+"/"+TimingConfigLoader.TimingFileName))
        using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            csv.Context.TypeConverterCache.AddConverter<float>(new UtilsT.MillisToSeconds());

            csv.Read();
            csv.ReadHeader();
            csv.Read();

            if (csv.Context.Reader?.HeaderRecord?.Length != 14)
                throw new Exception("Not all timing columns present in timing file");

            GameEngine.TimingData temp;
            try
            {
                temp = csv.GetRecord<GameEngine.TimingData>();
            }
            catch (Exception e)
            {
                throw new Exception($"Could not read row of timing file, is your data missing a row or are there any variables that have incorrect values?");
            }
            
            Exception negativeValException = new Exception("One or more values in timing file is negative");
            if (temp.EncounterStartDuration < 0
                || temp.EncounterDiscoverableDuration < 0
                || temp.EncounterTrialStartDuration < 0
                || temp.ExplanationPromptDuration < 0
                || temp.FixationDuration < 0
                || temp.TrialDuration < 0
                || temp.TrialEndDuration < 0
                || temp.TrialFeedbackDuration < 0
                || temp.EncounterEvaluationDuration < 0
                || temp.EncounterEndDuration < 0
                || temp.BreakDuration < 0
                || temp.BreakFeedbackDuration < 0
                || temp.LevelTransitionDuration < 0
                || temp.EnemyMindShowTime < 0)
                throw negativeValException;
        }
    }

    private const int LocalizationFileIdCount = 53;
    private void CheckLocalizationFile()
    {
        using (StreamReader reader = new StreamReader(Application.streamingAssetsPath+"/"+LocalizationTextLoader.LocalizationFileName))
        using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            csv.Context.RegisterClassMap<LocaleEntryMap>();
            csv.Read(); // Reads the header row
            csv.ReadHeader(); // Sets the HeaderRecord property

            if (csv.Context.Reader?.HeaderRecord?.Length < 2)
                throw new Exception("Missing id column and or at least on text column in localization file");

            bool[] present = new bool[LocalizationFileIdCount];

            int counter = 1;
            for (int i = 0; i < LocalizationFileIdCount; i++)
            {
                if (!csv.Read())
                    throw new Exception("Missing one or more entries in localization file");
                try 
                {
                    counter++;
                    present[csv.GetRecord<LocaleEntry>().ID] = true;
                }
                catch (Exception e)
                {
                    throw new Exception($"Could not read row {counter} of localization file, is your data missing a row, are there any variables that have incorrect values or is the ID of the entry incorrect?");
                }
            }

            bool throwIt = false;
            string missing = "Missing ids: ";
            for (int i = 0; i < LocalizationFileIdCount; i++)
            {
                if (!present[i])
                {
                    throwIt = true;
                    missing += $" ({i})";
                }
            }

            if (throwIt)
                throw new Exception(missing);
        }
    }

    private string[] minimumHeaders = new[]
    {
        "Level", "Environment", "BlockDelay", "ObjectType", "StimulusObject", "PropertyType", "StimulusProperty",
        "ValidProperty", "ITI"
    };

    private void CheckTrialFile()
    {
        using (StreamReader reader = new StreamReader(Application.streamingAssetsPath + "/" + TrialHandler.TrialsFileName))
        using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            csv.Context.TypeConverterCache.AddConverter<float>(new UtilsT.MillisToSeconds());
            csv.Read(); // Reads the header row
            csv.ReadHeader(); // Sets the HeaderRecord property

            List<string> missing = minimumHeaders.ToList();
            foreach (var t in csv.Context.Reader?.HeaderRecord)
            {
                missing.Remove(t);
            }

            string missingString = "Missing following columns: ";
            foreach (var header in missing)
            {
                missingString += $"({header}) ";
            }
            
            if (missing.Count > 0)
                throw new Exception(missingString);

            string missingObjectsString = "Missing following objects: ";
            bool missingObjects = false;
            string missingPropertiesString = "Missing following properties: ";
            bool missingProperties = false;
            int tutorialMinimumTracker = 5;
            int counter = 1;
            while (csv.Read())
            {
                TrialHandler.EncounterEntry encounterEntry;
                try
                {
                    counter++;
                    encounterEntry = csv.GetRecord<TrialHandler.EncounterEntry>();
                }
                catch (Exception e)
                {
                    throw new Exception($"Could not read row {counter} of trial file, is your data missing a row or are there any variables that have incorrect values?");
                }
                if (encounterEntry.Level == 0)
                {
                    tutorialMinimumTracker--;
                }
                
                TrialHandler.ConvertStringEnvironmentType(encounterEntry.Environment);
                EncounterData.ObjectType ot = TrialHandler.ConvertStringObjectType(encounterEntry.ObjectType);
                
                if (ot == EncounterData.ObjectType.BREAK) continue;
                
                EncounterData.PropertyType pt = TrialHandler.ConvertStringPropertyType(encounterEntry.PropertyType);
                
                if (ot != EncounterData.ObjectType.WORD)
                {
                    if (!ExternalAssetLoader.AssetExists(UtilsT.GetId(encounterEntry.StimulusObject)))
                    {
                        missingObjects = true;
                        missingObjectsString += $" ({encounterEntry.StimulusObject})";
                    }
                }

                if (pt != EncounterData.PropertyType.WORD)
                {
                    if (!ExternalAssetLoader.AssetExists(UtilsT.GetId(encounterEntry.StimulusProperty)))
                    {
                        missingProperties = true;
                        missingPropertiesString += $" ({encounterEntry.StimulusProperty})";
                    }
                }
            }
            
            if (missingObjects)
                throw new Exception(missingObjectsString);
            if (missingProperties)
                throw new Exception(missingPropertiesString);

            if (tutorialMinimumTracker > 0)
                throw new Exception("Missing trials for the tutorial (see manual)");
        }
    }
}
