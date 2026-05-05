using UnityEngine;
using System.Collections.Generic;

public class PieceInstance : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private BlockSpriteSet defaultSpriteSet;

    public PieceShape shape;
    public Vector2Int origin;

    private GridManager gridManager;
    private List<BlockController> blocks = new List<BlockController>();

    /// <summary>
    /// Inicializa a peça com sua forma e posição.
    /// </summary>
    public void Init(PieceShape newShape, Vector2Int newOrigin)
    {
        shape = newShape;
        origin = newOrigin;

        transform.position = new Vector3(origin.x, origin.y, 0);

        // Encontra o GridManager
        gridManager = FindObjectOfType<GridManager>();

        // Cria os blocos visuais
        CreateBlocks();
    }

    /// <summary>
    /// Inicializa com um spriteSet específico (sobrescreve o default).
    /// </summary>
    public void Init(PieceShape newShape, Vector2Int newOrigin, BlockSpriteSet spriteSet)
    {
        defaultSpriteSet = spriteSet;
        Init(newShape, newOrigin);
    }

    /// <summary>
    /// Cria os blocos visuais para cada célula da peça.
    /// </summary>
    private void CreateBlocks()
    {
        if (shape == null || gridManager == null)
        {
            Debug.LogError("PieceInstance: Shape ou GridManager não definido!");
            return;
        }

        if (defaultSpriteSet == null)
        {
            Debug.LogWarning("PieceInstance: Nenhum BlockSpriteSet definido. Os blocos ficarão sem sprite!");
        }

        // Remove possiveis colliders ou corpos do objeto "Pai" para ele ser apenas uma pasta vazia e não causar explosões de física
        Collider2D parentCol = GetComponent<Collider2D>();
        if (parentCol != null) Destroy(parentCol);
        Rigidbody2D parentRb = GetComponent<Rigidbody2D>();
        if (parentRb != null) Destroy(parentRb);

        int tetrisLayer = LayerMask.NameToLayer("TetrisBlocks");

        foreach (var cell in shape.cells)
        {
            int x = origin.x + cell.x;
            int y = origin.y + cell.y;

            // Cria o GameObject do bloco
            GameObject blockObj = new GameObject($"Block_{x}_{y}");
            blockObj.transform.parent = transform;
            blockObj.transform.localPosition = new Vector3(cell.x, cell.y, 0);
            
            if (tetrisLayer != -1) 
            {
                blockObj.layer = tetrisLayer;
            }

            // Adiciona o BlockController
            BlockController block = blockObj.AddComponent<BlockController>();
            block.Initialize(new Vector2Int(x, y), defaultSpriteSet);

            // Registra no GridManager
            gridManager.RegisterBlock(x, y, block);

            blocks.Add(block);
        }
    }

    /// <summary>
    /// Atualiza os sprites de todos os blocos desta peça.
    /// </summary>
    public void RefreshBlocks()
    {
        foreach (var block in blocks)
        {
            block.UpdateSprite();
        }
    }

    /// <summary>
    /// Define um novo spriteSet para todos os blocos desta peça.
    /// </summary>
    public void SetSpriteSet(BlockSpriteSet newSpriteSet)
    {
        defaultSpriteSet = newSpriteSet;
        foreach (var block in blocks)
        {
            block.SetSpriteSet(newSpriteSet);
        }
    }

    /// <summary>
    /// Remove todos os blocos do grid e destroy a peça.
    /// </summary>
    public void DestroyPiece()
    {
        if (gridManager != null)
        {
            foreach (var block in blocks)
            {
                gridManager.UnregisterBlock(block.GridPosition.x, block.GridPosition.y);
            }
        }

        Destroy(gameObject);
    }

    /// <summary>
    /// Retorna a lista de blocos desta peça.
    /// </summary>
    public List<BlockController> GetBlocks()
    {
        return new List<BlockController>(blocks);
    }
}
