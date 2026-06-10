using UnityEngine;
using TMPro; // Necessário para TextMeshPro
using UnityEngine.EventSystems; // Necessário para checar se o mouse está sobre a UI

public class TooltipManagerF2 : MonoBehaviour
{
    [Tooltip("A Câmera principal da cena")]
    public Camera mainCamera;
    
    [Header("Tooltip UI")]
    [Tooltip("O objeto 'pai' do tooltip (que você desativou)")]
    public GameObject tooltipObject; 
    [Tooltip("O componente de texto do tooltip")]
    public TextMeshProUGUI tooltipText;

    private Vector3 mousePosition;
    private RaycastHit2D raycastHit;



    void Start()
    {
        // Se a câmera não foi definida, tenta pegar a câmera principal
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        
        // Garante que o tooltip começa desligado
        tooltipObject.SetActive(false);
    }

    void Update()
    {
        // 1. Cria o raio a partir da câmera na posição do mouse
        mousePosition = Input.mousePosition;
        Ray mouseRay = Camera.main.ScreenPointToRay(mousePosition);

        raycastHit = Physics2D.Raycast(mouseRay.origin, mouseRay.direction);

        // 2. Dispara o raio
        if (raycastHit)
        {
            //Debug.Log(raycastHit.collider.name);

            // 3. Verifica se o objeto atingido tem o script 'InteractablePart'
            InteractablePart part = raycastHit.collider.GetComponent<InteractablePart>();

            if (part != null)
            {
                // ACERTAMOS UMA PARTE INTERATIVA!
                
                // Ativa o objeto do tooltip
                tooltipObject.SetActive(true);
                // Define o texto
                tooltipText.text = part.tooltipName;
                // Posiciona o tooltip perto do mouse
                // Adicionamos um pequeno offset para não ficar embaixo do cursor
                tooltipObject.transform.position = Input.mousePosition + new Vector3(200, -75, 0);
            }
            else
            {
                // Atingimos algo, mas não é uma parte interativa (ex: o resto do diafragma)
                tooltipObject.SetActive(false);
            }
        }
        else
        {
            // O raio não atingiu nada
            tooltipObject.SetActive(false);
        }
    }
}