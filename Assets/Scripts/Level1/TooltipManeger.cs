using UnityEngine;
using TMPro; 

public class TooltipManager : MonoBehaviour
{
    // Refer�ncia para os elementos de UI que criamos no Editor
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
        // Cria um raio a partir da posi��o do mouse na tela
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

                // Atualiza a posi��o do painel para seguir o mouse
                // Adicionamos um pequeno deslocamento (offset) para que o cursor n�o fique sobre o tooltip
                tooltipPanel.transform.position = Input.mousePosition + new Vector3(-200, -75, 0);

                // Define o texto do tooltip com base no nome do objeto atingido
                SetTooltipText(hit.collider.gameObject.name);
            }
            else
            {
                // Se o raio atingiu algo que n�o � uma pe�a, esconde o tooltip
                tooltipPanel.SetActive(false);
            }
        }
        else
        {
            // Se o raio n�o atingiu nada, esconde o tooltip
            tooltipPanel.SetActive(false);
        }
    }

    // Define o texto da descri��o baseado no nome da pe�a
    private void SetTooltipText(string pieceName)
    {
        string description = "";

        // O switch-case que voc� sugeriu para mapear nomes para descri��es
        switch (pieceName)
        {
            case "Tendao":
                description = "Tendão Central";
                break;
            case "Trapezio":
                description = "Trapézio";
                break;
            case "CostalDir":
                description = "Costal Esquerdo";
                break;
            case "CostalEsq":
                description = "Costal Direito";
                break;
            case "Esternal":
                description = "Esternal";
                break;
            case "EsternoCleidoM1":
                description = "Esternocleidomastóideo";
                break;
            case "EsternoCleidoM2":
                description = "Esternocleidomastóideo";
                break;
            case "Intercostais":
                description = "Intercostais Externos";
                break;
            case "Lombar":
                description = "Lombar";
                break;
            case "OblExterno":
                description = "Oblíquo Externo";
                break;
            case "PsoasMaior":
                description = "Psoas Maior";
                break;
            case "QuadLombar":
                description = "Quadrado Lombar";
                break;
            default:
                // Um texto padr�o caso a pe�a n�o seja encontrada no switch
                description = "";
                break;
        }

        tooltipText.text = description;
    }
}