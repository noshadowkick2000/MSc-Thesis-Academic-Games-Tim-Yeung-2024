using System.Globalization;
using System.IO;
using System.Linq;
using UnityEngine;
using CsvHelper;
using CsvHelper.Configuration;

namespace Assets
{
    public class LocalizationTextLoader : MonoBehaviour
    {
        public static readonly string LocalizationFileName = "LOTL_locale.csv";
        private static LocaleEntry[] _localeEntries;
        private static string[] _localeNames;

        private void Awake()
        {
            using (StreamReader reader = new StreamReader(Application.streamingAssetsPath+"/"+LocalizationFileName))
            using (CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Context.RegisterClassMap<LocaleEntryMap>();
                csv.Read(); // Reads the header row
                csv.ReadHeader(); // Sets the HeaderRecord property
                _localeNames = csv.Context.Reader.HeaderRecord;
                
                _localeEntries = csv.GetRecords<LocaleEntry>().ToArray();
            }

            _localeNames = _localeNames.Skip(1).ToArray();
            var test = GetLocales();

            foreach (var variable in test)
            {
                print(variable);
            }
            
            if (PlayerPrefs.HasKey(MainMenuHandler.LanguageKey))
                _currentLocale = PlayerPrefs.GetInt(MainMenuHandler.LanguageKey);
            else
                _currentLocale = 0;
        }

        private static int _currentLocale = 0;

        public static void SwitchLocale(bool forward)
        {
            _currentLocale = Mathf.Clamp(_currentLocale + (forward ? 1 : -1), 0, _localeNames.Length-1);
            PlayerPrefs.SetInt(MainMenuHandler.LanguageKey, _currentLocale);
            PlayerPrefs.Save();
        }

        public static string[] GetLocales()
        {
            return _localeNames;
        }

        public static string GetCurrentLocale()
        {
            return _localeNames[_currentLocale];
        }
            
        public static string GetLocaleEntry(int index)
        {
            return _localeEntries[index].Text[_currentLocale];
        }
    }

    public class LocaleEntry
    {
        public int ID { get; set; }
        public string[] Text { get; set; } 
    }

    public class LocaleEntryMap : ClassMap<LocaleEntry>
    {
        public LocaleEntryMap()
        {
            Map(m => m.ID).Index(0);
            Map(m => m.Text).Convert(row =>
            {
                var fields = row.Row.Parser.Record;
                return fields.Skip(1).ToArray();
            });
        }
    }
}