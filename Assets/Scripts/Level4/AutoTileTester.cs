using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Ferramenta de debug para testar o sistema de Auto-Tiling.
/// Anexe ao GridManager ou a um objeto vazio para testar os sprites.
/// </summary>
public class AutoTileTester : MonoBehaviour
{
    [Header("Test Configuration")]
    public BlockSpriteSet spriteSetToTest;
    public bool showSpritePreviews = true;

    [Header("Test Pattern")]
    [Tooltip("Criar padrão de teste ao iniciar")]
    public bool createTestPatternOnStart = false;

    private GridManager gridManager;

    void Start()
    {
        gridManager = FindObjectOfType<GridManager>();

        if (createTestPatternOnStart)
        {
            CreateTestPattern();
        }
    }

    /// <summary>
    /// Cria um padrão de teste que demonstra todos os 16 estados.
    /// </summary>
    [ContextMenu("Create Test Pattern")]
    public void CreateTestPattern()
    {
        if (gridManager == null)
        {
            Debug.LogError("GridManager não encontrado!");
            return;
        }

        if (spriteSetToTest == null)
        {
            Debug.LogError("SpriteSet não definido!");
            return;
        }

        // Limpa blocos existentes
        ClearAllBlocks();

        // Cria padrão que mostra todos os estados
        // Padrão em formato de cruz com blocos isolados ao redor

        // Bloco central (índice 15 - todos os lados)
        CreateTestBlock(5, 5, spriteSetToTest);

        // Cruz ao redor do centro
        CreateTestBlock(5, 6, spriteSetToTest); // Cima
        CreateTestBlock(6, 5, spriteSetToTest); // Direita
        CreateTestBlock(5, 4, spriteSetToTest); // Baixo
        CreateTestBlock(4, 5, spriteSetToTest); // Esquerda

        // Blocos isolados nos cantos (índice 0)
        CreateTestBlock(0, 0, spriteSetToTest);
        CreateTestBlock(10, 0, spriteSetToTest);

        // Linha horizontal (índices 8, 10, 10, 2)
        for (int x = 1; x <= 4; x++)
        {
            CreateTestBlock(x, 2, spriteSetToTest);
        }

        // Linha vertical (índices 1, 5, 5, 4)
        for (int y = 3; y <= 6; y++)
        {
            CreateTestBlock(8, y, spriteSetToTest);
        }

        // Padrão em L
        CreateTestBlock(0, 6, spriteSetToTest);
        CreateTestBlock(1, 6, spriteSetToTest);
        CreateTestBlock(2, 6, spriteSetToTest);
        CreateTestBlock(2, 5, spriteSetToTest);
        CreateTestBlock(2, 4, spriteSetToTest);

        Debug.Log("✅ Padrão de teste criado! Verifique os sprites no Game View.");
    }

    private void CreateTestBlock(int x, int y, BlockSpriteSet spriteSet)
    {
        if (gridManager.IsCellFree(x, y))
        {
            GameObject blockObj = new GameObject($"TestBlock_{x}_{y}");
            blockObj.transform.position = new Vector3(x, y, 0);

            BlockController block = blockObj.AddComponent<BlockController>();
            block.Initialize(new Vector2Int(x, y), spriteSet);
            gridManager.RegisterBlock(x, y, block);
        }
    }

    /// <summary>
    /// Remove todos os blocos do grid.
    /// </summary>
    [ContextMenu("Clear All Blocks")]
    public void ClearAllBlocks()
    {
        if (gridManager == null) return;

        // Encontra todos os BlockControllers
        BlockController[] allBlocks = FindObjectsOfType<BlockController>();

        foreach (var block in allBlocks)
        {
            gridManager.UnregisterBlock(block.GridPosition.x, block.GridPosition.y);
            if (Application.isPlaying)
                Destroy(block.gameObject);
            else
                DestroyImmediate(block.gameObject);
        }

        Debug.Log("🧹 Todos os blocos removidos.");
    }

    /// <summary>
    /// Atualiza todos os sprites (útil após mudanças manuais).
    /// </summary>
    [ContextMenu("Refresh All Sprites")]
    public void RefreshAllSprites()
    {
        if (gridManager == null) return;
        gridManager.RefreshAllBlocks();
        Debug.Log("🔄 Sprites atualizados.");
    }

    #if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (!showSpritePreviews || spriteSetToTest == null) return;

        // Mostra preview dos sprites no Scene View
        GUIStyle style = new GUIStyle();
        style.fontSize = 10;
        style.normal.textColor = Color.white;

        for (int i = 0; i < 16; i++)
        {
            Vector3 pos = new Vector3(i * 1.2f, -3, 0);

            // Desenha índice
            UnityEditor.Handles.Label(pos + Vector3.down * 0.5f, $"[{i}]", style);

            // Desenha sprite preview (se existir)
            if (spriteSetToTest.sprites[i] != null)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawIcon(pos, spriteSetToTest.sprites[i].name, true);
            }
        }
    }
    #endif
}
