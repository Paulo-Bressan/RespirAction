using UnityEngine;

/// <summary>
/// Componente que controla um bloco individual.
/// Gerencia o sprite baseado nos vizinhos no grid (sistema de auto-tiling).
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class BlockController : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private BlockSpriteSet spriteSet;

    [Header("Debug")]
    [SerializeField] private bool showDebugInfo = false;

    // Referências
    private SpriteRenderer spriteRenderer;
    private GridManager gridManager;

    // Posição no grid
    public Vector2Int GridPosition { get; private set; }
    public int CurrentBitmask { get; private set; }

    /// <summary>
    /// Inicializa o bloco com sua posição no grid.
    /// </summary>
    public void Initialize(Vector2Int gridPosition, BlockSpriteSet blockSpriteSet)
    {
        GridPosition = gridPosition;
        spriteSet = blockSpriteSet;

        // Configura o SpriteRenderer
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }

        // Adiciona BoxCollider2D para permitir interação de clique do mouse
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider == null)
        {
            collider = gameObject.AddComponent<BoxCollider2D>();
            collider.size = Vector2.one; // Assume tamanho padrão 1x1
        }

        // Encontra o GridManager
        gridManager = FindObjectOfType<GridManager>();

        // Atualiza o sprite
        UpdateSprite();
    }

    /// <summary>
    /// Atualiza o sprite baseado nos vizinhos no grid.
    /// Deve ser chamado quando um bloco vizinho é adicionado ou removido.
    /// </summary>
    public void UpdateSprite()
    {
        if (gridManager == null || spriteSet == null)
        {
            Debug.LogWarning($"BlockController: GridManager ou SpriteSet não definido em {GridPosition}");
            return;
        }

        // Verifica vizinhos
        bool hasUp = gridManager.IsCellOccupied(GridPosition.x, GridPosition.y + 1);
        bool hasRight = gridManager.IsCellOccupied(GridPosition.x + 1, GridPosition.y);
        bool hasDown = gridManager.IsCellOccupied(GridPosition.x, GridPosition.y - 1);
        bool hasLeft = gridManager.IsCellOccupied(GridPosition.x - 1, GridPosition.y);

        // Calcula a bitmask
        CurrentBitmask = BlockSpriteSet.CalculateBitmask(hasUp, hasRight, hasDown, hasLeft);

        // Atualiza o sprite
        Sprite newSprite = spriteSet.GetSprite(CurrentBitmask);
        if (newSprite != null)
        {
            spriteRenderer.sprite = newSprite;
        }
        else
        {
            Debug.LogWarning($"BlockController: Sprite não definido para bitmask {CurrentBitmask} em {GridPosition}");
        }

        if (showDebugInfo)
        {
            Debug.Log($"Bloco em {GridPosition}: {BlockSpriteSet.GetBitmaskDescription(CurrentBitmask)}");
        }
    }

    /// <summary>
    /// Define o SpriteSet em runtime (útil para trocar o tema visual)
    /// </summary>
    public void SetSpriteSet(BlockSpriteSet newSpriteSet)
    {
        spriteSet = newSpriteSet;
        UpdateSprite();
    }

    /// <summary>
    /// Define a ordem de renderização (para sobreposição correta)
    /// </summary>
    public void SetSortingOrder(int order)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = order;
        }
    }

    /// <summary>
    /// Detecta o clique do mouse e destrói o bloco no grid.
    /// Exige que haja um Collider (adicionado no Initialize) e que a câmera consiga disparar Raycasts.
    /// </summary>
    private void OnMouseDown()
    {
        if (gridManager != null)
        {
            gridManager.DestroyBlock(GridPosition.x, GridPosition.y);
        }
    }

    #if UNITY_EDITOR
    /// <summary>
    /// Visualização de debug no editor
    /// </summary>
    void OnDrawGizmosSelected()
    {
        if (!showDebugInfo) return;

        // Desenha linhas para os vizinhos
        Gizmos.color = Color.cyan;
        float size = 0.3f;

        // Cima
        if (gridManager != null && gridManager.IsCellOccupied(GridPosition.x, GridPosition.y + 1))
        {
            Gizmos.DrawLine(transform.position, transform.position + Vector3.up * 0.5f);
        }

        // Direita
        if (gridManager != null && gridManager.IsCellOccupied(GridPosition.x + 1, GridPosition.y))
        {
            Gizmos.DrawLine(transform.position, transform.position + Vector3.right * 0.5f);
        }

        // Baixo
        if (gridManager != null && gridManager.IsCellOccupied(GridPosition.x, GridPosition.y - 1))
        {
            Gizmos.DrawLine(transform.position, transform.position + Vector3.down * 0.5f);
        }

        // Esquerda
        if (gridManager != null && gridManager.IsCellOccupied(GridPosition.x - 1, GridPosition.y))
        {
            Gizmos.DrawLine(transform.position, transform.position + Vector3.left * 0.5f);
        }
    }
    #endif
}
