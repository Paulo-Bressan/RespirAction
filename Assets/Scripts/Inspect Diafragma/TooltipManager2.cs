using UnityEngine;
using TMPro; // Necessário para TextMeshPro
using UnityEngine.EventSystems; // Necessário para checar se o mouse está sobre a UI

public class TooltipManager2 : MonoBehaviour
{
    [Tooltip("A Câmera principal da cena")]
    public Camera mainCamera;

    [Tooltip("O Audio Manager da cena")]
    public AudioManagerScene audioManager;

    [Header("Tooltip UI")]
    [Tooltip("O objeto 'pai' do tooltip (que você desativou)")]
    public GameObject tooltipObject; 
    [Tooltip("O componente de texto do tooltip")]
    public TextMeshProUGUI tooltipText;

    // flags se os audios ja foram tocados alguma vez
    private bool[] wasPlayed = new bool[6];

    void Start()
    {
        // Se a câmera não foi definida, tenta pegar a câmera principal
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    void Update()
    {
        // Se o mouse estiver sobre um elemento de UI (como o texto do "Espaço"),
        // não mostre o tooltip 3D.
        if (EventSystem.current.IsPointerOverGameObject())
        {
            tooltipObject.SetActive(false);
            return;
        }

        // 1. Cria o raio a partir da câmera na posição do mouse
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // 2. Dispara o raio
        if (Physics.Raycast(ray, out hit))
        {
            // 3. Verifica se o objeto atingido tem o script 'InteractablePart'
            InteractablePart part = hit.collider.GetComponent<InteractablePart>();

            if (part != null)
            {
                // ACERTAMOS UMA PARTE INTERATIVA!
                
                // Ativa o objeto do tooltip
                tooltipObject.SetActive(true);
                // Define o texto
                tooltipText.text = part.tooltipName;
                // Posiciona o tooltip perto do mouse
                // Adicionamos um pequeno offset para não ficar embaixo do cursor
                tooltipObject.transform.position = Input.mousePosition + new Vector3(15, 15, 0);
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