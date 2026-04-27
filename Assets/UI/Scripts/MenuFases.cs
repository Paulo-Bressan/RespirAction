using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuFases : MonoBehaviour
{
    [Header("Botões das Fases")]
    public Button fase1Button;
    public Button fase2Button;
    public Button fase3Button;

    [Header("Botão Créditos")]
    public Button creditosButton;

    void Start()
    {
        // Verifica qual fase está desbloqueada
        int faseDesbloqueada = PlayerPrefs.GetInt("faseDesbloqueada", 1);

        // Fase 1 sempre desbloqueada
        fase1Button.interactable = true;

        // Fase 2 e 3 dependem do progresso
        fase2Button.interactable = (faseDesbloqueada >= 2);
        fase3Button.interactable = (faseDesbloqueada >= 3);

        // Adiciona eventos aos botões
        // ALTERAÇÃO AQUI: Fase 1 agora carrega a cena do vídeo, que por sua vez carregará a Fase 1.
        fase1Button.onClick.AddListener(() => CarregarFase("Cutscene1")); 
        
        fase2Button.onClick.AddListener(() => CarregarFase("Fase2"));
        fase3Button.onClick.AddListener(() => CarregarFase("Fase3"));
        creditosButton.onClick.AddListener(() => CarregarFase("Creditos"));
    }

    // Função para carregar fases
    void CarregarFase(string nomeCena)
    {
        SceneManager.LoadScene(nomeCena);
    }

    // Chame esta função quando o jogador terminar uma fase
    public static void DesbloquearProximaFase(int faseAtual)
    {
        int faseDesbloqueada = PlayerPrefs.GetInt("faseDesbloqueada", 1);

        if (faseAtual >= faseDesbloqueada)
        {
            PlayerPrefs.SetInt("faseDesbloqueada", faseAtual + 1);
            PlayerPrefs.Save();
        }
    }
}
