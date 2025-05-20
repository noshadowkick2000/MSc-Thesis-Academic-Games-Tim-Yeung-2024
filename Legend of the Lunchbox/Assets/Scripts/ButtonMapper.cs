using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonMapper : MonoBehaviour
{
    public TextMeshProUGUI buttonText;
    public TextMeshProUGUI buttonKeyText;
    public int id = 0;

    public void MapButton()
    {
        FindObjectOfType<MainMenuHandler>().StartMap(this);
    }
}
