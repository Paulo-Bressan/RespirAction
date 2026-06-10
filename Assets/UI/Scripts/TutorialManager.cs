using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [Header("Ambas referencias opcionais, depende do objetivo do script na cena")]

    [Tooltip("(Se tiver o toggle) Funcionalidade do toggle em si")]
    [SerializeField] private Toggle myToggle;

    [Tooltip("(Se tiver os tutoriais) Objeto pai das informações de tutorial")]
    [SerializeField] private GameObject tutorialParentObj;

    // flag de tutoriais ativos
    private bool isTutorialOn = true;


    void Start()
    {
        // se nao existir a pref ainda, cria ela e salva no default (true)
        // se existir, carrega ela para isToturialOn
        if (!PlayerPrefs.HasKey("tutorialOn"))
            PlayerPrefs.SetInt("tutorialOn", (isTutorialOn ? 1 : 0));
        else
            isTutorialOn = (PlayerPrefs.GetInt("tutorialOn") != 0);

        // trocando o toggle baseado na pref salva, se o toggle existir na cena
        if (myToggle)
            myToggle.isOn = isTutorialOn;
        else
            Debug.Log("[MENU] Toggle de tutorial não encontrado, não modificando nada");

        // ativando tutoriais em tela dependendo do estado do toggle, se existir
        if (tutorialParentObj)
            tutorialParentObj.SetActive(isTutorialOn);
        else
            Debug.Log("[MENU] Objeto de tutorial não encontrado, não modificando nada");
    }

    public void toggleTutorial(Toggle toggle)
    {
        if (toggle.isOn)
            Debug.Log("[MENU] Ativando tutorial em tela");
        else
            Debug.Log("[MENU] Desativando tutorial em tela");

        isTutorialOn = toggle.isOn;

        PlayerPrefs.SetInt("tutorialOn", (isTutorialOn ? 1 : 0));
        //Debug.Log(PlayerPrefs.GetInt("tutorialOn"));
    }
}
