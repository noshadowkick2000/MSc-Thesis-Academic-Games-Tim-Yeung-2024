using System;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEngine;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using Unity.VisualScripting;

namespace Assets
{
    public class LocalizationTextLoader : MonoBehaviour
    {
        [SerializeField] private string localizationFileName;
        private static LocaleEntry[] localeEntries;
        private static string[] localeNames;

        private void Awake()
        {
            using (StreamReader reader = new StreamReader(Application.streamingAssetsPath+"/"+localizationFileName))
            using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Context.RegisterClassMap<LocaleEntryMap>();
                csv.Read(); // Reads the header row
                csv.ReadHeader(); // Sets the HeaderRecord property
                localeNames = csv.Context.Reader.HeaderRecord;
                
                localeEntries = csv.GetRecords<LocaleEntry>().ToArray();
            }

            localeNames = localeNames.Skip(1).ToArray();
            var test = GetLocales();

            foreach (var VARIABLE in test)
            {
                print(VARIABLE);
            }
            
            if (PlayerPrefs.HasKey(MainMenuHandler.LanguageKey))
                currentLocale = PlayerPrefs.GetInt(MainMenuHandler.LanguageKey);
            else
                currentLocale = 0;
        }

        private static int currentLocale = 0;

        public static void SwitchLocale(bool forward)
        {
            currentLocale = Mathf.Clamp(currentLocale + (forward ? 1 : -1), 0, localeEntries.Length-1);
            PlayerPrefs.SetInt(MainMenuHandler.LanguageKey, currentLocale);
            PlayerPrefs.Save();
        }

        public static string[] GetLocales()
        {
            return localeNames;
        }

        public static string GetCurrentLocale()
        {
            return localeNames[currentLocale];
        }
            
        public static string GetLocaleEntry(int index)
        {
            return localeEntries[index].text[currentLocale];
        }
    }

    public class LocaleEntry
    {
        public int id { get; set; }
        public string[] text { get; set; } 
    }

    public class LocaleEntryMap : ClassMap<LocaleEntry>
    {
        public LocaleEntryMap()
        {
            Map(m => m.id).Index(0);
            Map(m => m.text).Convert(row =>
            {
                var fields = row.Row.Parser.Record;
                return fields.Skip(1).ToArray();
            });
        }
    }
}