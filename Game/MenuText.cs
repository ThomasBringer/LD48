using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MenuText : MonoBehaviour
{
    [SerializeField] GameObject smPrefab;

    TextMeshProUGUI uiText;

    void OnEnable()
    {
        uiText = GetComponent<TextMeshProUGUI>();

        string text = Save.menuText;

        if (text == null) Instantiate(smPrefab);
        else uiText.text = text;

        // string text = PlayerPrefs.GetString("MenuText", "");

        // if (text != "") uiText.text = text;
    }
}