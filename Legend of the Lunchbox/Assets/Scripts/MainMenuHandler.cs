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
    [SerializeField] private RectTransform creditsText;
    [SerializeField] private Toggle soundToggle;
    [SerializeField] private Toggle feedbackToggle;
    [SerializeField] private Toggle promptToggle;
    [SerializeField] private Toggle fmriToggle;
    [SerializeField] private TextMeshProUGUI language;
    [FormerlySerializedAs("inputMapScreen")] [SerializeField] private RectTransform inputMapMenu;
    [SerializeField] private GameObject inputMapButtonEntryPrefab;
    
    private ButtonMapper[] inputMapButtonEntries;

    private float xStart;
    private float yStart;
    
    public void StartGame()
    {
        LeanTween.moveX(mainMenu, -1.5f * mainMenu.rect.width, .5f).setEaseInBack();
        
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
        LeanTween.moveX(mainMenu, -1.5f * mainMenu.rect.width, .5f).setEaseInBack();
        LeanTween.moveY(settingsMenu, 0, .5f).setEaseOutBack();
    }

    public void CloseSettings()
    {
        LeanTween.moveX(mainMenu, xStart, .5f).setEaseOutBack();
        LeanTween.moveY(settingsMenu, -1.5f * settingsMenu.rect.height, .5f).setEaseInBack();
        
        PlayerPrefs.Save();
    }

    public void ShowCredits()
    {
        LeanTween.moveX(mainMenu, -1.5f * mainMenu.rect.width, .5f).setEaseInBack();
        LeanTween.moveX(creditsMenu, xStart, .5f).setEaseOutBack();

        creditsScrollRoutine = StartCoroutine(ScrollCredits());
    }

    private Coroutine creditsScrollRoutine;

    private IEnumerator ScrollCredits()
    {
        float height = creditsText.rect.height * 2;
        Vector2 pos = Vector2.zero;
        float move = .1f;
        
        creditsText.anchoredPosition = Vector2.zero;
        
        while (true)
        {
            if (creditsText.anchoredPosition.y > height)
                creditsText.anchoredPosition = Vector2.zero; 
            creditsText.anchoredPosition += new Vector2(0, move);

            yield return null;
        }
    }

    public void CloseCredits()
    {
        LeanTween.moveX(mainMenu, xStart, .5f).setEaseOutBack();
        LeanTween.moveX(creditsMenu, -1.5f * creditsMenu.rect.width, .5f).setEaseInBack();
        
        StopCoroutine(creditsScrollRoutine);
        creditsScrollRoutine = null;
    }

    public void OpenCalibrationMenu()
    {
        objectPrefab.SetActive(true);
        
        LeanTween.moveY(settingsMenu, -1.5f * settingsMenu.rect.height, .5f).setEaseInBack();
        LeanTween.moveY(calibrationMenu, yStart, .5f).setEaseOutBack();
    }
    
    public void CloseCalibrationMenu()
    {
        objectPrefab.SetActive(false);
        
        LeanTween.moveY(settingsMenu, 0, .5f).setEaseOutBack();
        LeanTween.moveY(calibrationMenu, -1.5f * calibrationMenu.rect.height, .5f).setEaseInBack();
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

    public void OpenMapMenu()
    {
        LeanTween.moveY(settingsMenu, -1.5f * settingsMenu.rect.height, .5f).setEaseInBack();
        LeanTween.moveY(inputMapMenu, 0, .5f).setEaseOutBack();
    }

    public void CloseMapMenu()
    {
        LeanTween.moveY(settingsMenu, 0, .5f).setEaseOutBack();
        LeanTween.moveY(inputMapMenu, -1.5f * inputMapMenu.rect.height, .5f).setEaseInBack();
    }

    public void StartMap(ButtonMapper buttonMapper)
    {
        FindObjectOfType<TInput>().StartInputMap((TInput.ButtonNames) buttonMapper.id, EndMap);
        buttonMapper.buttonKeyText.text = LocalizationTextLoader.GetLocaleEntry(56);
    }

    private void EndMap(TInput.ButtonNames bName, KeyCode newCode)
    {
        inputMapButtonEntries[(int)bName].buttonKeyText.text = newCode.ToString();
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

    public void SetFMRISettings()
    {
        bool fmriEnabled = fmriToggle.isOn;
        PlayerPrefs.SetInt(FMRIKey, fmriEnabled ? 1 : 0);
        print(fmriEnabled);
    }

    public void AdvanceLanguage(bool forward)
    {
        LocalizationTextLoader.SwitchLocale(forward);
        language.text = LocalizationTextLoader.GetCurrentLocale();
        FindObjectOfType<UILanguageSetter>().SetAll();
    }

    public void CloseGame()
    {
        Application.Quit();
    }

    private void Start()
    {
        if (!PlayerPrefs.HasKey(SoundKey) 
            || !PlayerPrefs.HasKey(FeedbackKey) 
            || !PlayerPrefs.HasKey(PromptKey)
            || !PlayerPrefs.HasKey(SpriteSizeKey)
            || !PlayerPrefs.HasKey(FMRIKey))
        {
            PlayerPrefs.SetInt(SoundKey, 1);
            PlayerPrefs.SetInt(FeedbackKey, 1);
            PlayerPrefs.SetInt(PromptKey, 1);
            PlayerPrefs.SetFloat(SpriteSizeKey, .5f);
            PlayerPrefs.SetInt(FMRIKey, 1);
        }
        
        inputMapButtonEntries = new ButtonMapper[TInput.Buttons.Length];

        for (int i = 0; i < TInput.Buttons.Length; i++)
        {
            inputMapButtonEntries[i] = Instantiate(inputMapButtonEntryPrefab, inputMapMenu).GetComponent<ButtonMapper>();
            inputMapButtonEntries[i].buttonText.text = TInput.Buttons[i].buttonName.ToString();
            inputMapButtonEntries[i].buttonKeyText.text = TInput.Buttons[i].assignedKeyCode.ToString();
            inputMapButtonEntries[i].id = i;
            ((RectTransform)inputMapButtonEntries[i].transform).anchoredPosition = new Vector2(0, -20 - i * 40);
        }
        
        soundToggle.isOn = PlayerPrefs.GetInt(SoundKey) == 1;
        feedbackToggle.isOn = PlayerPrefs.GetInt(FeedbackKey) == 1;
        promptToggle.isOn = PlayerPrefs.GetInt(PromptKey) == 1;
        fmriToggle.isOn = PlayerPrefs.GetInt(FMRIKey) == 1;
        calibrationSlider.value = PlayerPrefs.GetFloat(SpriteSizeKey);
        calibrationInputField.text = PlayerPrefs.GetFloat(SpriteSizeKey).ToString(CultureInfo.InvariantCulture);
        
        objectPrefab.SetActive(false);

        xStart = 50;
        yStart = 25f;

        objectPrefab.GetComponentInChildren<SpriteRenderer>().sprite = ExternalAssetLoader.GetFirstSprite();

        language.text = LocalizationTextLoader.GetCurrentLocale();

        LevelHandler.CurrentLevel = 0;
    }
    
    public static readonly string SoundKey = "SoundEnabled";
    public static readonly string FeedbackKey = "FeedbackEnabled";
    public static readonly string PromptKey = "PromptEnabled";
    public static readonly string LanguageKey = "Language";
    public static readonly string SpriteSizeKey = "SpriteSize";
    public static readonly string FMRIKey = "FMRIEnabled";
}
