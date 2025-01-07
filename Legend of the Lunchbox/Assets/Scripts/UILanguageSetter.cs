using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Assets;

public class UILanguageSetter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI[] textMeshProUGUI;
    [SerializeField] private int[] localeEntryId;
    
    private void Start()
    {
        if (textMeshProUGUI.Length != localeEntryId.Length)
            throw new Exception();
        
        for (int i = 0; i < textMeshProUGUI.Length; i++)
        {
            textMeshProUGUI[i].text = TextHolder.GetLocaleEntry(localeEntryId[i]);
        }
    }
}
