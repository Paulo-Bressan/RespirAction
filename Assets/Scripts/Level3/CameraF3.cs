using UnityEngine;

/// <summary>
/// Controla o comportamento da câmera, alternando entre seguir o jogador e enquadrar uma área de limites.
/// Utiliza LateUpdate para garantir suavidade no acompanhamento do alvo.
/// Os limites da câmera são definidos por um BoxCollider2D para flexibilidade.
/// </summary>
[RequireComponent(typeof(Camera))] // Garante que sempre haverá um componente de Câmera neste objeto.
public class CameraController : MonoBehaviour
{
    // =================================================================
    // CONFIGURAÇÕES DO ALVO
    // =================================================================
    [Header("Alvo")]
    [Tooltip("Transform do jogador que a câmera deve seguir.")]
    [SerializeField] private Transform playerTransform;

    // =================================================================
    // CONFIGURAÇÕES DO MAPA E POSICIONAMENTO
    // =================================================================
    [Header("Modo Mapa")]
    [Tooltip("Tecla que alterna entre a visão do jogador e a visão do mapa.")]
    [SerializeField] private KeyCode mapKey = KeyCode.M;

    [Tooltip("Posição Z da câmera durante o gameplay normal.")]
    [SerializeField] private float normalZPosition = -10f;

    [Tooltip("Posição Z da câmera no modo mapa (afastado).")]
    [SerializeField] private float mapZPosition = -50f; 

    // =================================================================
    // LIMITES DA CÂMERA
    // =================================================================
    [Header("Limites da Câmera")]
    [Tooltip("Define se a câmera deve respeitar os limites do BoxCollider2D.")]
    [SerializeField] private bool useBounds = true;

    [Tooltip("BoxCollider2D que define a área limite para a câmera.")]
    [SerializeField] private BoxCollider2D boundsCollider;

    // --- Componentes e Estado Interno ---
    private Camera mainCamera;
    private bool isFollowingPlayer = true;
    private float gameplayOrthographicSize; // Para guardar o zoom original

    void Start()
    {
        // Obtém a referência para o componente da câmera neste GameObject
        mainCamera = GetComponent<Camera>();
        // Guarda o zoom inicial para restaurá-lo depois
        gameplayOrthographicSize = mainCamera.orthographicSize;

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
            isFollowingPlayer = !isFollowingPlayer;

            if (isFollowingPlayer)
            {
                // -- VOLTANDO A SEGUIR O JOGADOR --
                // Restaura o zoom original do gameplay
                mainCamera.orthographicSize = gameplayOrthographicSize;
                // O LateUpdate cuidará de mover a câmera de volta para o jogador
            }
            else
            {
                // -- ATIVANDO O MODO MAPA --
                FocusOnBounds();
            }
        }
    }

    /// <summary>
    /// Centraliza a câmera e ajusta o zoom para preencher a visão com o boundsCollider.
    /// Isso garante que a câmera não mostre nada fora dos limites, cortando o excesso se necessário.
    /// </summary>
    void FocusOnBounds()
    {
        if (boundsCollider == null)
        {
            // Se não há limites definidos, apenas centraliza em 0,0 com zoom padrão.
            transform.position = new Vector3(0, 0, mapZPosition);
            return;
        }

        Bounds bounds = boundsCollider.bounds;

        // Calcula o tamanho ortográfico necessário para que a LARGURA dos limites preencha a tela.
        float requiredSizeForWidth = bounds.size.x / mainCamera.aspect;

        // Calcula o tamanho ortográfico necessário para que a ALTURA dos limites preencha a tela.
        float requiredSizeForHeight = bounds.size.y;

        // Usamos o MENOR dos dois tamanhos. Isso força a câmera a dar zoom para "preencher" a tela.
        // A dimensão que for maior que a proporção da tela será cortada.
        // Dividimos por 2 porque orthographicSize é a metade da altura total.
        mainCamera.orthographicSize = Mathf.Min(requiredSizeForWidth, requiredSizeForHeight) * 0.5f;

        // Posiciona a câmera no centro dos limites.
        transform.position = new Vector3(bounds.center.x, bounds.center.y, mapZPosition);
    }

    /// <summary>
    /// LateUpdate é chamado após todos os Updates. Ideal para câmeras.
    /// </summary>
    void LateUpdate()
    {
        if (!isFollowingPlayer)
        {
            // Se não estamos seguindo o jogador, não faz nada aqui
            return;
        }

        if (playerTransform == null)
        {
            // Se o jogador não existe, não há quem seguir
            return;
        }
        
        // Posição alvo inicial é a do jogador
        Vector3 targetPosition = new Vector3(playerTransform.position.x, playerTransform.position.y, normalZPosition);

        // Aplica os limites se estiverem ativados e configurados
        if (useBounds && boundsCollider != null)
        {
            if (!mainCamera.orthographic)
            {
                Debug.LogWarning("Os limites da câmera funcionam melhor com uma câmera ortográfica. O comportamento pode ser inesperado.");
            }

            float cameraHalfHeight = mainCamera.orthographicSize;
            float cameraHalfWidth = mainCamera.aspect * cameraHalfHeight;

            Bounds colliderBounds = boundsCollider.bounds;

            float minX = colliderBounds.min.x + cameraHalfWidth;
            float maxX = colliderBounds.max.x - cameraHalfWidth;
            float minY = colliderBounds.min.y + cameraHalfHeight;
            float maxY = colliderBounds.max.y - cameraHalfHeight;

            if (minX > maxX) { minX = maxX = (colliderBounds.min.x + colliderBounds.max.x) / 2; }
            if (minY > maxY) { minY = maxY = (colliderBounds.min.y + colliderBounds.max.y) / 2; }

            targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
            targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);
        }
        
        // Atualiza a posição da câmera
        transform.position = targetPosition;
    }
}