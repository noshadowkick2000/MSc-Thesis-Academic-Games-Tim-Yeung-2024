using System;
using System.Collections;
using System.Collections.Generic;
using Assets;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuHandler : MonoBehaviour
{
    [SerializeField] private RectTransform MainMenu;
    private float xStart;
    [SerializeField] private RectTransform SettingsMenu;
    private float xEnd;
    [SerializeField] private RectTransform CreditsMenu;
    [SerializeField] private Toggle soundToggle;
    [SerializeField] private Toggle feedbackToggle;
    [SerializeField] private Toggle promptToggle;
    [SerializeField] private TextMeshProUGUI language;
    
    public void StartGame()
    {
        LeanTween.moveX(MainMenu, xEnd, .5f).setEaseInBack();
        
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
        LeanTween.moveX(MainMenu, xEnd, .5f).setEaseInBack();
        LeanTween.moveX(SettingsMenu, xStart, .5f).setEaseOutBack();
    }

    public void CloseSettings()
    {
        LeanTween.moveX(MainMenu, xStart, .5f).setEaseOutBack();
        LeanTween.moveX(SettingsMenu, xEnd, .5f).setEaseInBack();
        
        PlayerPrefs.Save();
    }

    public void ShowCredits()
    {
        LeanTween.moveX(MainMenu, xEnd, .5f).setEaseInBack();
        LeanTween.moveX(CreditsMenu, xStart, .5f).setEaseOutBack();
    }

    public void CloseCredits()
    {
        LeanTween.moveX(MainMenu, xStart, .5f).setEaseOutBack();
        LeanTween.moveX(CreditsMenu, xEnd, .5f).setEaseInBack();
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
        if (!PlayerPrefs.HasKey(SoundKey) || !PlayerPrefs.HasKey(FeedbackKey) || !PlayerPrefs.HasKey(PromptKey))
        {
            PlayerPrefs.SetInt(SoundKey, 1);
            PlayerPrefs.SetInt(FeedbackKey, 1);
            PlayerPrefs.SetInt(PromptKey, 1);
        }
        
        soundToggle.isOn = PlayerPrefs.GetInt(SoundKey, 1) == 1;
        feedbackToggle.isOn = PlayerPrefs.GetInt(FeedbackKey, 1) == 1;
        promptToggle.isOn = PlayerPrefs.GetInt(PromptKey, 1) == 1;

        xStart = MainMenu.anchoredPosition.x;
        xEnd = SettingsMenu.anchoredPosition.x;

        language.text = LocalizationTextLoader.GetCurrentLocale();
    }
    
    public static readonly string SoundKey = "SoundEnabled";
    public static readonly string FeedbackKey = "FeedbackEnabled";
    public static readonly string PromptKey = "PromptEnabled";
    public static readonly string LanguageKey = "Language";
}
