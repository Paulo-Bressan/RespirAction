using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager5 : MonoBehaviour
{
    [Header("Nome da cena carregada ao vencer")]
    [SerializeField] private string cenaVitoria;
    [Header("Nome da cena carregada ao perder")]
    [SerializeField] private string cenaDerrota;

    [Header("Checkmarks de acerto")]
    [SerializeField] private GameObject mark1;
    [SerializeField] private GameObject mark2;
    [SerializeField] private GameObject mark3;

    [Header("Checkmarks de falha")]
    [SerializeField] private GameObject mark4;
    [SerializeField] private GameObject mark5;
    [SerializeField] private GameObject mark6;
    [SerializeField] private GameObject mark7;
    [SerializeField] private GameObject mark8;

    // contador de acertos
    private int acertos = 0;

    // contador de falhas
    private int falhas = 0;

    void Start()
    {
        if (!TimeManager.instance)
            Debug.LogError("[LEVEL MANAGER] TimeManager faltando");
        
        if (!mark1 || !mark2 || !mark3 || !mark4 || !mark5 || !mark6)
            Debug.LogWarning("[LEVEL MANAGER] Checkmarks faltando");
    }

    public void updateAcertos()
    {
        acertos++;
        Debug.Log("[LEVEL MANAGER] Acertos: " + acertos);

        switch (acertos)
        {
            case 1:
                mark1.SetActive(true);
                break;

            case 2:
                mark2.SetActive(true);
                break;

            case 3:
                mark3.SetActive(true);
                // vence jogo
                Debug.Log("[LEVEL MANAGER] Condicao de vitoria alcançada");
                StartCoroutine(LoadSceneWithDelay(cenaVitoria));
                break;
        }
    }

    public void updateFalhas()
    {
        falhas++;
        Debug.Log("[LEVEL MANAGER] Falhas: " + falhas);

        switch (falhas)
        {
            case 1:
                mark4.SetActive(true);
                break;

            case 2:
                mark5.SetActive(true);
                break;

            case 3:
                mark6.SetActive(true);
                break;

            case 4:
                mark7.SetActive(true);
                break;

            case 5:
                mark8.SetActive(true);
                // falha jogo
                Debug.Log("[LEVEL MANAGER] Condicao de derrota alcançada");
                StartCoroutine(LoadSceneWithDelay(cenaDerrota));
                break;
        }
    }

    private IEnumerator LoadSceneWithDelay(string nomeCena)
    {
        Debug.Log("[LEVEL MANAGER] Carregando cena com atraso, aguarde");
        yield return new WaitForSeconds(2.0f);
        SceneManager.LoadScene(nomeCena);
    }
}
