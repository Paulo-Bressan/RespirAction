using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NUnit.Framework;
using System.Collections.Generic;

public class ScreenManager : MonoBehaviour
{
    [Tooltip("Canvas / UIs principais")]
    [SerializeField] private Canvas[] canvas;

    [Tooltip("Toggle da tela cheia")]
    [SerializeField] private Toggle fullscreenToggle;

    [Tooltip("Menu em cascata da resolução")]
    [SerializeField] private TMP_Dropdown resDropDown;

    [Tooltip("Largura resolução")]
    [SerializeField] private int resWidth = 1920;
    [Tooltip("Altura resolução")]
    [SerializeField] private int resHeight = 1080;

    // Matriz de todas resoluções
    private Resolution[] resArray;
    // fullscreen ativado ou não
    private bool isFullScreen = false;
    // resolução selecionada
    private int dropdownIndex;
    

    void Start()
    {
        // inicializando player prefs

        if (!PlayerPrefs.HasKey("fullscreenOn"))
            PlayerPrefs.SetInt("fullscreenOn", (isFullScreen ? 1 : 0));
        else
            isFullScreen = (PlayerPrefs.GetInt("fullscreenOn") != 0);

        if (!PlayerPrefs.HasKey("resWidth"))
            PlayerPrefs.SetInt("resWidth", resWidth);
        else
            resWidth = PlayerPrefs.GetInt("resWidth");

        if (!PlayerPrefs.HasKey("resHeight"))
            PlayerPrefs.SetInt("resHeight", resHeight);
        else
            resHeight = PlayerPrefs.GetInt("resHeight");

        if (!PlayerPrefs.HasKey("screenDDIndex"))
            PlayerPrefs.SetInt("screenDDIndex", dropdownIndex);
        else
            dropdownIndex = PlayerPrefs.GetInt("screenDDIndex");

        // inicializando array de resolucoes

        resArray = Screen.resolutions;

        List<string> resStringList = new List<string>();
        foreach (Resolution res in resArray)
        {
            resStringList.Add(res.ToString());
        }

        resDropDown.AddOptions(resStringList);

        // alterando o menu de opcoes basedo nos player prefs

        fullscreenToggle.isOn = isFullScreen;
        resDropDown.value = dropdownIndex;

    }

    public void SetRes()
    {
        // pega a resolucao do array na posicao escolhida
        dropdownIndex = resDropDown.value;

        resWidth = resArray[dropdownIndex].width;
        resHeight = resArray[dropdownIndex].height;

        // configura resolucao da tela
        Screen.SetResolution(resWidth, resHeight, isFullScreen);

        // salva prefs
        PlayerPrefs.SetInt("resWidth", resWidth);
        PlayerPrefs.SetInt("resHeight", resHeight);
        PlayerPrefs.SetInt("screenDDIndex", dropdownIndex);

        Debug.Log("[TELA] Nova resolução: " + resWidth + " x " + resHeight);
        Debug.Log("[TELA] ScreenDDIndex guardado na pos " + dropdownIndex);
    }

    public void SetFullScreen()
    {
        // guarda valor do toggle
        isFullScreen = fullscreenToggle.isOn;

        // configura resolucao da tela
        Screen.SetResolution(resWidth, resHeight, isFullScreen);

        // salva prefs
        PlayerPrefs.SetInt("fullscreenOn", (isFullScreen ? 1 : 0));

        Debug.Log("[TELA] Fullscreen alterado para " + isFullScreen);
    }
}
