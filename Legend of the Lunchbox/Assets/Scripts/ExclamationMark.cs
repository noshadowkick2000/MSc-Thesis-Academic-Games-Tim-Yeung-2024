using System;
using System.Collections;
using System.Collections.Generic;
using Assets;
using UnityEngine;
using UnityEngine.UI;

public class ExclamationMark : MonoBehaviour
{
    [SerializeField] private RectTransform exclamationMark;
    [SerializeField] private Image exclamationMarkImage;
    private Vector2 ap;

    private void Awake()
    {
        GameEngine.StartingEncounterStartedEvent += StartingEncounter;
        GameEngine.SettingUpMindStartedEvent += SettingUpMind;
        
        ap = exclamationMark.anchoredPosition;
        exclamationMark.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        GameEngine.StartingEncounterStartedEvent -= StartingEncounter;
        GameEngine.SettingUpMindStartedEvent -= SettingUpMind;
    }

    protected virtual void StartingEncounter()
    {
        exclamationMark.gameObject.SetActive(true);
        StartCoroutine(AnimateExclamationMark());
    }

    private IEnumerator AnimateExclamationMark()
    {
        float startTime = Time.realtimeSinceStartup;
        float x = 0;
        float height = 7f;
        Vector2 p = ap;
        Color c = Color.white;
        c.a = 0f;
        
        exclamationMarkImage.color = c;
        exclamationMark.anchoredPosition = p;

        while (x < 1)
        {
            x = (Time.realtimeSinceStartup - startTime) / (GameEngine.StaticTimeVariables.EncounterStartDuration/2);
            c.a = x;
            p.y = x * height;

            exclamationMarkImage.color = c;
            exclamationMark.anchoredPosition = p;
            yield return null;
        }

        p.y = height;
        exclamationMarkImage.color = Color.white;
        exclamationMark.anchoredPosition = p;
    }

    protected virtual void SettingUpMind()
    {
        exclamationMark.gameObject.SetActive(false);
    }
}
