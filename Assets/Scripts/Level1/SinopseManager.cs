using UnityEngine;

public class SinopseManager : MonoBehaviour
{
    [SerializeField] private GameObject painelSinopse; // Arraste o painel aqui no Inspector
    private string chaveSinopse = "JaViuSinopse";

    void Start()
    {
        // Verifica se a chave "JaViuSinopse" NÃO existe (valor 0)
        if (PlayerPrefs.GetInt(chaveSinopse, 0) == 0)
        {
            ExibirSinopse();
        }
    }

    void ExibirSinopse()
    {
        painelSinopse.SetActive(true);
        // Salva que o jogador já viu, mudando o valor para 1
        PlayerPrefs.SetInt(chaveSinopse, 1);
        PlayerPrefs.Save();
    }

    // Função para ligar ao botão "Fechar" da sinopse
    public void FecharSinopse()
    {
        painelSinopse.SetActive(false);
    }
}