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

            var test = GetLocales();

            foreach (var VARIABLE in test)
            {
                print(VARIABLE);
            }
            
            SetLocale(1);
        }


        public static int currentLocale = 0;
        public static void SetLocale(int locale)
        {
            currentLocale = locale;
        }

        public static string[] GetLocales()
        {
            return localeNames;
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