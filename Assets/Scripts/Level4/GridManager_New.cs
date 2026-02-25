using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    [Header("Grid Size")]
    public int width = 12;
    public int height = 8;
    public int defeatHeight = 7;

    private int[,] grid; // 0 = vazio, 1 = ocupado

    // Referências aos BlockControllers em cada célula
    private BlockController[,] blockControllers;

    public int Width => width;
    public int Height => height;

    void Awake()
    {
        grid = new int[width, height];
        blockControllers = new BlockController[width, height];
    }

    public bool IsInsideGrid(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }

    public bool IsCellFree(int x, int y)
    {
        if (!IsInsideGrid(x, y)) return false;
        return grid[x, y] == 0;
    }

    /// <summary>
    /// Verifica se uma célula está ocupada (para cálculo de vizinhos)
    /// </summary>
    public bool IsCellOccupied(int x, int y)
    {
        if (!IsInsideGrid(x, y)) return false;
        return grid[x, y] != 0;
    }

    /// <summary>
    /// Marca uma célula como ocupada e armazena a referência ao bloco.
    /// NOTA: Use RegisterBlock() para notificar vizinhos automaticamente.
    /// </summary>
    public void SetCell(int x, int y, int value)
    {
        if (!IsInsideGrid(x, y)) return;
        grid[x, y] = value;
    }

    /// <summary>
    /// Registra um bloco no grid e notifica os vizinhos para atualizar seus sprites.
    /// </summary>
    public void RegisterBlock(int x, int y, BlockController block)
    {
        if (!IsInsideGrid(x, y)) return;

        grid[x, y] = 1;
        blockControllers[x, y] = block;

        // Notifica os vizinhos para atualizar seus sprites
        NotifyNeighbors(x, y);
    }

    /// <summary>
    /// Remove um bloco do grid e notifica os vizinhos.
    /// </summary>
    public void UnregisterBlock(int x, int y)
    {
        if (!IsInsideGrid(x, y)) return;

        grid[x, y] = 0;
        blockControllers[x, y] = null;

        // Notifica os vizinhos para atualizar seus sprites
        NotifyNeighbors(x, y);
    }

    /// <summary>
    /// Retorna o BlockController em uma posição específica.
    /// </summary>
    public BlockController GetBlockController(int x, int y)
    {
        if (!IsInsideGrid(x, y)) return null;
        return blockControllers[x, y];
    }

    /// <summary>
    /// Destrói fisicamente o bloco e remove-o do grid.
    /// Os vizinhos serão notificados pelo UnregisterBlock para atualizarem seus sprites.
    /// </summary>
    public void DestroyBlock(int x, int y)
    {
        if (!IsInsideGrid(x, y)) return;

        BlockController block = GetBlockController(x, y);
        if (block != null)
        {
            // Remove do grid (isto notifica os vizinhos para fechar "pontas soltas")
            UnregisterBlock(x, y);
            
            // Destrói o GameObject
            Destroy(block.gameObject);
        }
    }

    /// <summary>
    /// Notifica os 4 vizinhos ortogonais para atualizarem seus sprites.
    /// </summary>
    private void NotifyNeighbors(int x, int y)
    {
        // Direções: cima, direita, baixo, esquerda
        Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int(0, 1),   // Cima
            new Vector2Int(1, 0),   // Direita
            new Vector2Int(0, -1),  // Baixo
            new Vector2Int(-1, 0)   // Esquerda
        };

        foreach (var dir in directions)
        {
            int neighborX = x + dir.x;
            int neighborY = y + dir.y;

            if (IsInsideGrid(neighborX, neighborY))
            {
                BlockController neighbor = blockControllers[neighborX, neighborY];
                if (neighbor != null)
                {
                    neighbor.UpdateSprite();
                }
            }
        }
    }

    /// <summary>
    /// Atualiza todos os blocos no grid (útil para mudança de tema visual)
    /// </summary>
    public void RefreshAllBlocks()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (blockControllers[x, y] != null)
                {
                    blockControllers[x, y].UpdateSprite();
                }
            }
        }
    }

    public bool CheckDefeat()
    {
        for (int x = 0; x < width; x++)
        {
            if (grid[x, defeatHeight] != 0)
                return true;
        }
        return false;
    }

    #if UNITY_EDITOR
    /// <summary>
    /// Visualização de debug do grid no editor
    /// </summary>
    void OnDrawGizmos()
    {
        if (grid == null) return;

        // Desenha o grid
        Gizmos.color = Color.gray;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 center = new Vector3(x, y, 0);
                Gizmos.DrawWireCube(center, Vector3.one * 0.95f);

                // Destaca células ocupadas
                if (grid[x, y] != 0)
                {
                    Gizmos.color = new Color(0.2f, 0.6f, 0.8f, 0.3f);
                    Gizmos.DrawCube(center, Vector3.one * 0.9f);
                    Gizmos.color = Color.gray;
                }
            }
        }

        // Linha de derrota
        Gizmos.color = Color.red;
        Vector3 defeatLineStart = new Vector3(-0.5f, defeatHeight, 0);
        Vector3 defeatLineEnd = new Vector3(width - 0.5f, defeatHeight, 0);
        Gizmos.DrawLine(defeatLineStart, defeatLineEnd);
    }
    #endif
}
