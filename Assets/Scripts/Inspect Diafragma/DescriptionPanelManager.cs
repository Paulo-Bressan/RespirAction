using UnityEngine;
using TMPro; // Para TextMeshPro
using UnityEngine.UI; // Para o Botão

public class DescriptionPanelManager : MonoBehaviour
{
    // Singleton: Uma forma de acessar este script de qualquer lugar
    public static DescriptionPanelManager Instance { get; private set; }

    [Header("UI do Painel")]
    public GameObject descriptionPanelObject;
    public TextMeshProUGUI descriptionText;
    public Button closeButton;

    void Awake()
    {
        // Configuração do Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        // Adiciona a função 'HidePanel' ao clique do botão
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(HidePanel);
        }
        // Garante que o painel começa desligado
        descriptionPanelObject.SetActive(false); 
    }

    // Função pública para mostrar o painel com um novo texto
    public void ShowPanel(string newDescription)
    {
        descriptionText.text = newDescription;
        descriptionPanelObject.SetActive(true);
    }

    // Função para esconder o painel (usada pelo botão)
    public void HidePanel()
    {
        descriptionPanelObject.SetActive(false);
    }

    // Função para verificar se o painel está aberto
    public bool IsPanelOpen()
    {
        return descriptionPanelObject.activeSelf;
    }
}