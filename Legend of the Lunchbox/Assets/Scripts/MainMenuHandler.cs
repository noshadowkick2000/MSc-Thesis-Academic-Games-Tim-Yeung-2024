using System.Collections;
using System.Globalization;
using Assets;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MainMenuHandler : MonoBehaviour
{
    [FormerlySerializedAs("MainMenu")] [SerializeField] private RectTransform mainMenu;
    [FormerlySerializedAs("SettingsMenu")] [SerializeField] private RectTransform settingsMenu;
    [SerializeField] private RectTransform calibrationMenu;
    [SerializeField] private TMP_InputField calibrationInputField;
    [SerializeField] private Slider calibrationSlider;
    [SerializeField] private GameObject objectPrefab;
    [FormerlySerializedAs("CreditsMenu")] [SerializeField] private RectTransform creditsMenu;
    [SerializeField] private Toggle soundToggle;
    [SerializeField] private Toggle feedbackToggle;
    [SerializeField] private Toggle promptToggle;
    [SerializeField] private TextMeshProUGUI language;

    private float xStart;
    private float xEnd;
    private float yStart;
    private float yEnd;
    
    public void StartGame()
    {
        LeanTween.moveX(mainMenu, xEnd, .5f).setEaseInBack();
        
        Logger.StartLogger();
        
        Invoke(nameof(Load), .5f);
    }

    private void Load()
    {
        StartCoroutine(LoadingScene());
    }

    private IEnumerator LoadingScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(1);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
    
    public void OpenSettings()
    {
        LeanTween.moveX(mainMenu, xEnd, .5f).setEaseInBack();
        LeanTween.moveX(settingsMenu, xStart, .5f).setEaseOutBack();
    }

    public void CloseSettings()
    {
        LeanTween.moveX(mainMenu, xStart, .5f).setEaseOutBack();
        LeanTween.moveX(settingsMenu, xEnd, .5f).setEaseInBack();
        
        PlayerPrefs.Save();
    }

    public void ShowCredits()
    {
        LeanTween.moveX(mainMenu, xEnd, .5f).setEaseInBack();
        LeanTween.moveX(creditsMenu, xStart, .5f).setEaseOutBack();
    }

    public void CloseCredits()
    {
        LeanTween.moveX(mainMenu, xStart, .5f).setEaseOutBack();
        LeanTween.moveX(creditsMenu, xEnd, .5f).setEaseInBack();
    }

    public void OpenCalibrationMenu()
    {
        objectPrefab.SetActive(true);
        
        LeanTween.moveX(settingsMenu, xEnd, .5f).setEaseInBack();
        LeanTween.moveY(calibrationMenu, yStart, .5f).setEaseOutBack();
    }
    
    public void CloseCalibrationMenu()
    {
        objectPrefab.SetActive(false);
        
        LeanTween.moveX(settingsMenu, xStart, .5f).setEaseOutBack();
        LeanTween.moveY(calibrationMenu, yEnd, .5f).setEaseInBack();
    }

    public void SetCalibratedSizeFromText()
    {
        string size = calibrationInputField.text;
        if (!float.TryParse(size, out float sizeConverted))
            calibrationInputField.text = PlayerPrefs.GetFloat(SpriteSizeKey).ToString(CultureInfo.InvariantCulture);
        else
            SetCalibratedSize(sizeConverted);
    }

    public void SetCalibratedSizeFromSlider()
    {
        SetCalibratedSize(calibrationSlider.value);
    }

    private void SetCalibratedSize(float size)
    {
        size = Mathf.Clamp(size, 0f, 1f);
        size = Mathf.Round(size * 100) / 100f;
        
        calibrationInputField.text = size.ToString();
        calibrationSlider.value = size;

        objectPrefab.transform.localScale = (.5f + size) * Vector3.one;
        
        PlayerPrefs.SetFloat(SpriteSizeKey, size);
    }

    public void OpenManualWebsite()
    {
        Application.OpenURL("https://dandymaro.notion.site/MPI-Project-d09ee1e6c19e48bc8cfdc33c7b90cbf0?pvs=4");
    }

    public void SetSoundSettings()
    {
        bool soundEnabled = soundToggle.isOn;
        PlayerPrefs.SetInt(SoundKey, soundEnabled ? 1 : 0);
        print(soundEnabled);
    }

    public void SetFeedbackSettings()
    {
        bool feedbackEnabled = feedbackToggle.isOn;
        PlayerPrefs.SetInt(FeedbackKey, feedbackEnabled ? 1 : 0);
        print(feedbackEnabled);
    }

    public void SetPromptSettings()
    {
        bool promptEnabled = promptToggle.isOn;
        PlayerPrefs.SetInt(PromptKey, promptEnabled ? 1 : 0);
        print(promptEnabled);
    }

    public void AdvanceLanguage(bool forward)
    {
        LocalizationTextLoader.SwitchLocale(forward);
        language.text = LocalizationTextLoader.GetCurrentLocale();
        FindObjectOfType<UILanguageSetter>().SetAll();
    }

    private void Start()
    {
        if (!PlayerPrefs.HasKey(SoundKey) 
            || !PlayerPrefs.HasKey(FeedbackKey) 
            || !PlayerPrefs.HasKey(PromptKey)
            || !PlayerPrefs.HasKey(SpriteSizeKey))
        {
            PlayerPrefs.SetInt(SoundKey, 1);
            PlayerPrefs.SetInt(FeedbackKey, 1);
            PlayerPrefs.SetInt(PromptKey, 1);
            PlayerPrefs.SetFloat(SpriteSizeKey, .5f);
        }
        
        soundToggle.isOn = PlayerPrefs.GetInt(SoundKey, 1) == 1;
        feedbackToggle.isOn = PlayerPrefs.GetInt(FeedbackKey, 1) == 1;
        promptToggle.isOn = PlayerPrefs.GetInt(PromptKey, 1) == 1;
        calibrationSlider.value = PlayerPrefs.GetFloat(SpriteSizeKey);
        calibrationInputField.text = PlayerPrefs.GetFloat(SpriteSizeKey).ToString(CultureInfo.InvariantCulture);
        
        objectPrefab.SetActive(false);

        xStart = mainMenu.anchoredPosition.x;
        xEnd = settingsMenu.anchoredPosition.x;

        yStart = 25f;
        yEnd = calibrationMenu.anchoredPosition.y;

        objectPrefab.GetComponentInChildren<SpriteRenderer>().sprite = ExternalAssetLoader.GetFirstSprite();

        language.text = LocalizationTextLoader.GetCurrentLocale();

        LevelHandler.CurrentLevel = 0;
    }
    
    public static readonly string SoundKey = "SoundEnabled";
    public static readonly string FeedbackKey = "FeedbackEnabled";
    public static readonly string PromptKey = "PromptEnabled";
    public static readonly string LanguageKey = "Language";
    public static readonly string SpriteSizeKey = "SpriteSize";
}
