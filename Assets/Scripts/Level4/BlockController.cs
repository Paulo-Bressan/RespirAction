using UnityEngine;
using System.Collections;

/// <summary>
/// Componente que controla um bloco individual.
/// Gerencia o sprite baseado nos vizinhos no grid (sistema de auto-tiling).
/// Adicionadas funções para delay e efeitos visuais básicos de quebra na Fase 4.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class BlockController : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private BlockSpriteSet spriteSet;

    [Header("Interaction Settings")]
    [Tooltip("Tempo (s) entre o clique do player e a destruição real do bloco. Usado para animação de soco.")]
    [SerializeField] private float breakDelay = 0.4f;

    [Header("Debug")]
    [SerializeField] private bool showDebugInfo = false;

    // Referências
    private SpriteRenderer spriteRenderer;
    private GridManager gridManager;
    private PlayerMovementF4 player;

    // Estado local para evitar cliques contínuos durante o delay
    private bool isBreaking = false;

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
        BoxCollider2D myCollider = GetComponent<BoxCollider2D>();
        if (myCollider == null)
        {
            myCollider = gameObject.AddComponent<BoxCollider2D>();
            myCollider.size = Vector2.one; // Assume tamanho padrão 1x1
        }

        // Encontra o GridManager e Player
        gridManager = FindObjectOfType<GridManager>();
        player = FindObjectOfType<PlayerMovementF4>();

        // Atualiza o sprite
        UpdateSprite();
    }

    /// <summary>
    /// Atualiza o sprite baseado nos vizinhos no grid.
    /// </summary>
    public void UpdateSprite()
    {
        if (gridManager == null || spriteSet == null)
        {
            if (showDebugInfo) Debug.LogWarning($"BlockController: GridManager ou SpriteSet não definido em {GridPosition}");
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
        else if (showDebugInfo)
        {
            Debug.LogWarning($"BlockController: Sprite não definido para bitmask {CurrentBitmask} em {GridPosition}");
        }
    }

    public void SetSpriteSet(BlockSpriteSet newSpriteSet)
    {
        spriteSet = newSpriteSet;
        UpdateSprite();
    }

    public void SetSortingOrder(int order)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sortingOrder = order;
        }
    }

    /// <summary>
    /// Detecta clique do mouse. Se o jogador tiver distância, avisa o player para animar
    /// por 'breakDelay' segundos e depois a gente se destrói.
    /// </summary>
    private void OnMouseDown()
    {
        if (gridManager == null || isBreaking) return;

        if (player != null)
        {
            float distance = Vector2.Distance(player.transform.position, this.transform.position);

            if (distance <= player.maxInteractionDistance)
            {
                // Dá ordem ao Player para rotacionar o braço / tocar animação pelo tempo breakDelay
                player.StartInteraction(this.transform, breakDelay);

                // Começa o processo local de quebra
                StartCoroutine(BreakSequenceRoutine());
            }
        }
        else
        {
            gridManager.DestroyBlock(GridPosition.x, GridPosition.y);
        }
    }

    /// <summary>
    /// Aguarda o decorrer do soco/interação, cria as partículas baseadas na cor primária,
    /// e se apaga do Tetris de vez.
    /// </summary>
    private IEnumerator BreakSequenceRoutine()
    {
        isBreaking = true;
        
        yield return new WaitForSeconds(breakDelay);

        SpawnBreakParticles();

        if (gridManager != null)
        {
            gridManager.DestroyBlock(GridPosition.x, GridPosition.y);
        }
    }

    /// <summary>
    /// Cria programaticamente um gerador de partículas que suga um pixel da nossa arte
    /// </summary>
    private void SpawnBreakParticles()
    {
        if (spriteRenderer == null || spriteRenderer.sprite == null) return;

        // Criar OBJ de Partícula Limpo
        GameObject vfxObject = new GameObject("BlockBreakVFX");
        vfxObject.transform.position = transform.position;

        ParticleSystem ps = vfxObject.AddComponent<ParticleSystem>();
        
        // Pega a cor predominante ou apenas amosta o centro do sprite
        Color blockColor = Color.white;
        try 
        {
            Texture2D tex = spriteRenderer.sprite.texture;
            Rect rect = spriteRenderer.sprite.textureRect;
            // Lê o pixel do centro da imagem pra tentar ser a cor principal do bloco
            blockColor = tex.GetPixel((int)(rect.x + (rect.width/2)), (int)(rect.y + (rect.height/2)));
        } 
        catch { /* Catch falhas se sprite não for legível CPU */ }

        // Configura Partículas (Quadradinhos)
        var main = ps.main;
        var em = ps.emission;
        var shape = ps.shape;
        var renderer = ps.GetComponent<ParticleSystemRenderer>();

        main.duration = 0.5f;
        main.startLifetime = 0.6f;
        main.startSpeed = new ParticleSystem.MinMaxCurve(2f, 5f);
        main.startSize = 0.25f;
        main.loop = false;
        main.playOnAwake = true;
        main.startColor = new ParticleSystem.MinMaxGradient(blockColor, Color.white);
        
        // Estilo e material para parecer "quadrado 2D pixel art" sem blur
        renderer.material = new Material(Shader.Find("Sprites/Default"));

        em.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0.0f, 12) });

        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.5f;

        // Auto destroy após 1 sec
        Destroy(vfxObject, 1.0f);
    }

    #if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (!showDebugInfo) return;

        Gizmos.color = Color.cyan;
        if (gridManager != null && gridManager.IsCellOccupied(GridPosition.x, GridPosition.y + 1)) Gizmos.DrawLine(transform.position, transform.position + Vector3.up * 0.5f);
        if (gridManager != null && gridManager.IsCellOccupied(GridPosition.x + 1, GridPosition.y)) Gizmos.DrawLine(transform.position, transform.position + Vector3.right * 0.5f);
        if (gridManager != null && gridManager.IsCellOccupied(GridPosition.x, GridPosition.y - 1)) Gizmos.DrawLine(transform.position, transform.position + Vector3.down * 0.5f);
        if (gridManager != null && gridManager.IsCellOccupied(GridPosition.x - 1, GridPosition.y)) Gizmos.DrawLine(transform.position, transform.position + Vector3.left * 0.5f);
    }
    #endif
}
