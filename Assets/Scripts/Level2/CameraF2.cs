using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Controla o comportamento da câmera, alternando entre seguir o jogador e enquadrar uma área de limites.
/// Utiliza LateUpdate para garantir suavidade no acompanhamento do alvo.
/// </summary>
[RequireComponent(typeof(Camera))] // Garante que sempre haverá um componente de Câmera neste objeto.
public class CameraControllerF2 : MonoBehaviour
{
    // =================================================================
    // CONFIGURAÇÕES DO ALVO
    // =================================================================
    [Header("Alvo")]
    [Tooltip("Transform do jogador que a câmera deve seguir.")]
    [SerializeField] private Transform playerTransform;

    [Tooltip("Transform da visão de mapa.")]
    [SerializeField] private Transform mapTransform;

    [Tooltip("Transform da área de zoom predefinida.")]
    [SerializeField] private Transform zoomTransform;

    // =================================================================
    // CONFIGURAÇÕES DO MAPA E POSICIONAMENTO
    // =================================================================
    [Header("Modo Mapa")]
    [Tooltip("Tecla que alterna entre a visão do jogador e a visão do mapa.")]
    [SerializeField] private KeyCode mapKey = KeyCode.M;

    [Tooltip("Posição Z da câmera durante o gameplay normal.")]
    [SerializeField] private float normalZPosition = -10f;

    [Tooltip("Tamanho da câmera durante visualização do mapa.")]
    [SerializeField] private float mapCamSize = 3;

    [Tooltip("Tamanho da câmera durante visualização da cabeça.")]
    [SerializeField] private float zoomCamSize = 3;

    // --- Componentes e Estado Interno ---
    private Camera mainCamera;
    private bool mapFocus = false;
    private bool headFocus = false;
    private float gameplayCamSize; // Para guardar o zoom original
    private Vector3 targetPosition; // Posição alvo para "animação"

    void Start()
    {
        // Obtém a referência para o componente da câmera neste GameObject
        mainCamera = GetComponent<Camera>();
        // Guarda o zoom inicial para restaurá-lo depois
        gameplayCamSize = mainCamera.orthographicSize;

        // Garante que a câmera comece na posição correta do jogador se ele existir
        if (playerTransform != null)
        {
            transform.position = new Vector3(playerTransform.position.x, playerTransform.position.y, normalZPosition);
        }
    }

    void Update()
    {
        // Verifica se a tecla de mapa foi pressionada
        if (Input.GetKeyDown(mapKey))
        {
            mapFocus = !mapFocus;
            if (mapFocus)
                Debug.Log("[CAMERA] Focando camera no mapa");
            else
                Debug.Log("[CAMERA] Tirando foco do mapa");
        }
    }

    public void EnterFocus()
    {
        // apenas entra no foco de cabeça se mapa não está aberto
        if (!mapFocus)
        {
            Debug.Log("[CAMERA] Focando camera na cabeça");
            headFocus = true;
        }
    }

    public void LeaveFocus()
    {
        Debug.Log("[CAMERA] Tirando foco da cabeça");
        headFocus = false;
    }

    /// <summary>
    /// LateUpdate é chamado após todos os Updates. Ideal para câmeras.
    /// </summary>
    void LateUpdate()
    {
        if (mapFocus)
        {
            // Posição alvo é o mapa
            targetPosition = mapTransform.position;
            mainCamera.orthographicSize = mapCamSize;
        }
        else if (headFocus)
        {
            // Posição alvo é a cabeça
            targetPosition = zoomTransform.position;
            mainCamera.orthographicSize = zoomCamSize;
        }
        else
        {
            // Posição alvo é a do jogador
            // Se o jogador não existe, não há quem seguir
            if (playerTransform == null) return;

            targetPosition.x = playerTransform.position.x;
            targetPosition.y = playerTransform.position.y;
            targetPosition.z = normalZPosition;

            mainCamera.orthographicSize = gameplayCamSize;
        }
        
        // faz "animação" de acompanhamento
        transform.position = Vector3.Lerp(transform.position, targetPosition, 0.05f);
    }
}