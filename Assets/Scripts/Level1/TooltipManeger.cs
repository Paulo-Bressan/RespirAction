using UnityEngine;
using TMPro; 

public class TooltipManager : MonoBehaviour
{
    // Referência para os elementos de UI que criamos no Editor
    public GameObject tooltipPanel;
    public TextMeshProUGUI tooltipText;

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        // Garante que o painel comece desativado
        if (tooltipPanel != null)
        {
            tooltipPanel.SetActive(false);
        }
    }

    void Update()
    {
        // Cria um raio a partir da posição do mouse na tela
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Executa o Raycast. Verifica se o raio colidiu com algum objeto
        if (Physics.Raycast(ray, out hit))
        {
            // Verifica se o objeto com que colidiu tem a tag "Peca"
            if (hit.collider.CompareTag("Peca"))
            {
                // Ativa o painel do tooltip
                tooltipPanel.SetActive(true);

                // Atualiza a posição do painel para seguir o mouse
                // Adicionamos um pequeno deslocamento (offset) para que o cursor não fique sobre o tooltip
                tooltipPanel.transform.position = Input.mousePosition + new Vector3(200, -75, 0);

                // Define o texto do tooltip com base no nome do objeto atingido
                SetTooltipText(hit.collider.gameObject.name);
            }
            else
            {
                // Se o raio atingiu algo que não é uma peça, esconde o tooltip
                tooltipPanel.SetActive(false);
            }
        }
        else
        {
            // Se o raio não atingiu nada, esconde o tooltip
            tooltipPanel.SetActive(false);
        }
    }

    // Define o texto da descrição baseado no nome da peça
    private void SetTooltipText(string pieceName)
    {
        string description = "";

        // O switch-case que você sugeriu para mapear nomes para descrições
        switch (pieceName)
        {
            case "Esternal":
                description = "Parte esternal";
                break;
            case "CostalEsq":
                description = "Parte costal esquerda";
                break;
            case "CostalDir":
                description = "Parte costal direita";
                break;
            case "Tendao":
                description = "Centro tendíneo";
                break;
            case "Lombar":
                description = "Parte lombar";
                break;
            default:
                // Um texto padrão caso a peça não seja encontrada no switch
                description = "";
                break;
        }

        tooltipText.text = description;
    }
}