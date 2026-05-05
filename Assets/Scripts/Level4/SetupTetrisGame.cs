using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Script de configuração rápida - Configura todo o sistema de Auto-Tiling automaticamente.
///
 /// Use este script para configurar rapidamente:
/// 1. Criar pasta de recursos
/// 2. Gerar todas as peças do Tetris
/// 3. Criar o prefab de bloco
/// 4. Configurar a cena com todos os componentes necessários
/// </summary>
public class SetupTetrisGame : MonoBehaviour
{
    #if UNITY_EDITOR

    [MenuItem("TetrisPlatformer/Setup Complete Game", false, 0)]
    public static void SetupCompleteGame()
    {
        Debug.Log("🚀 Iniciando configuração completa do Tetris Platformer...");

        // 1. Cria estrutura de pastas
        CreateFolderStructure();

        // 2. Cria o prefab de bloco
        CreateBlockPrefab();

        // 3. Gera todas as peças
        GenerateAllPieces();

        // 4. Cria objetos na cena
        SetupScene();

        Debug.Log("✅ Configuração completa! Configure o BlockSpriteSet e os sprites para começar.");
    }

    private static void CreateFolderStructure()
    {
        string[] folders = new string[]
        {
            "Assets/Scriptables",
            "Assets/Scriptables/PieceShapes",
            "Assets/Scriptables/BlockSprites",
            "Assets/Prefabs",
            "Assets/Sprites"
        };

        foreach (string folder in folders)
        {
            if (!AssetDatabase.IsValidFolder(folder))
            {
                string parent = System.IO.Path.GetDirectoryName(folder).Replace("\\", "/");
                string name = System.IO.Path.GetFileName(folder);
                AssetDatabase.CreateFolder(parent, name);
                Debug.Log($"📁 Pasta criada: {folder}");
            }
        }

        AssetDatabase.Refresh();
    }

    private static void CreateBlockPrefab()
    {
        string prefabPath = "Assets/Prefabs/Block.prefab";

        // Verifica se já existe
        if (AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath) != null)
        {
            Debug.Log("⏭️ Prefab Block já existe");
            return;
        }

        // Cria o GameObject
        GameObject block = new GameObject("Block");

        // Adiciona SpriteRenderer
        SpriteRenderer sr = block.AddComponent<SpriteRenderer>();
        sr.sortingOrder = 0;

        // Adiciona BlockController
        block.AddComponent<BlockController>();

        // Salva como prefab
        PrefabUtility.SaveAsPrefabAsset(block, prefabPath);
        DestroyImmediate(block);

        Debug.Log($"✨ Prefab criado: {prefabPath}");
    }

    private static void GenerateAllPieces()
    {
        string outputFolder = "Scriptables/PieceShapes";

        // Cores padrão do Tetris
        var pieces = new[]
        {
            (name: "Piece_I", cells: new Vector2Int[] {
                new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0), new Vector2Int(3, 0)
            }, color: new Color(0f, 1f, 1f)),

            (name: "Piece_O", cells: new Vector2Int[] {
                new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(0, 1), new Vector2Int(1, 1)
            }, color: new Color(1f, 1f, 0f)),

            (name: "Piece_T", cells: new Vector2Int[] {
                new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(2, 1), new Vector2Int(1, 0)
            }, color: new Color(0.5f, 0f, 1f)),

            (name: "Piece_S", cells: new Vector2Int[] {
                new Vector2Int(1, 1), new Vector2Int(2, 1), new Vector2Int(0, 0), new Vector2Int(1, 0)
            }, color: new Color(0f, 1f, 0f)),

            (name: "Piece_Z", cells: new Vector2Int[] {
                new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(1, 0), new Vector2Int(2, 0)
            }, color: new Color(1f, 0f, 0f)),

            (name: "Piece_L", cells: new Vector2Int[] {
                new Vector2Int(0, 1), new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0)
            }, color: new Color(1f, 0.5f, 0f)),

            (name: "Piece_J", cells: new Vector2Int[] {
                new Vector2Int(2, 1), new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0)
            }, color: new Color(0f, 0f, 1f))
        };

        foreach (var (name, cells, color) in pieces)
        {
            string fullPath = $"Assets/{outputFolder}/{name}.asset";

            // Verifica se já existe
            if (AssetDatabase.LoadAssetAtPath<PieceShape>(fullPath) != null)
            {
                Debug.Log($"⏭️ {name} já existe");
                continue;
            }

            PieceShape piece = ScriptableObject.CreateInstance<PieceShape>();
            piece.cells = cells;
            piece.debugColor = color;

            AssetDatabase.CreateAsset(piece, fullPath);
            Debug.Log($"✨ Peça criada: {name}");
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private static void SetupScene()
    {
        // Encontra ou cria o GridManager
        GridManager gridManager = Object.FindObjectOfType<GridManager>();
        if (gridManager == null)
        {
            GameObject gridObj = new GameObject("GridManager");
            gridManager = gridObj.AddComponent<GridManager>();
            Debug.Log("✨ GridManager criado na cena");
        }

        // Encontra ou cria o PieceSpawner
        PieceSpawner spawner = Object.FindObjectOfType<PieceSpawner>();
        if (spawner == null)
        {
            GameObject spawnerObj = new GameObject("PieceSpawner");
            spawner = spawnerObj.AddComponent<PieceSpawner>();
            Debug.Log("✨ PieceSpawner criado na cena");
        }

        // Configura o PieceSpawner
        spawner.grid = gridManager;

        // Carrega o prefab de bloco
        GameObject blockPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Block.prefab");
        if (blockPrefab != null)
        {
            spawner.piecePrefab = blockPrefab;
        }

        // Carrega todas as peças
        string[] guids = AssetDatabase.FindAssets("t:PieceShape", new[] { "Assets/Scriptables/PieceShapes" });
        spawner.availableShapes = new System.Collections.Generic.List<PieceShape>();

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            PieceShape shape = AssetDatabase.LoadAssetAtPath<PieceShape>(path);
            if (shape != null)
            {
                spawner.availableShapes.Add(shape);
            }
        }

        Debug.Log($"📋 {spawner.availableShapes.Count} peças carregadas no PieceSpawner");

        // Cria BlockSpriteSet vazio para o usuário configurar
        string spriteSetPath = "Assets/Scriptables/BlockSprites/StandardBlocks.asset";
        if (AssetDatabase.LoadAssetAtPath<BlockSpriteSet>(spriteSetPath) == null)
        {
            BlockSpriteSet spriteSet = ScriptableObject.CreateInstance<BlockSpriteSet>();
            AssetDatabase.CreateAsset(spriteSet, spriteSetPath);
            Debug.Log($"✨ BlockSpriteSet criado: configure os sprites em {spriteSetPath}");
        }

        EditorUtility.SetDirty(spawner);
        EditorUtility.SetDirty(gridManager);

        // Seleciona o spawner para fácil configuração
        Selection.activeGameObject = spawner.gameObject;

        Debug.Log("═════════════════════════════════════════════════════");
        Debug.Log("🎮 SETUP COMPLETO!");
        Debug.Log("═════════════════════════════════════════════════════");
        Debug.Log("PRÓXIMOS PASSOS:");
        Debug.Log("1. Crie seus 16 sprites (veja a documentação)");
        Debug.Log("2. Configure o BlockSpriteSet 'StandardBlocks'");
        Debug.Log("3. Arraste o BlockSpriteSet para o PieceSpawner");
        Debug.Log("4. Execute o jogo!");
        Debug.Log("═════════════════════════════════════════════════════");
    }

    [MenuItem("TetrisPlatformer/Create Block Prefab", false, 1)]
    public static void CreateBlockPrefab_Menu()
    {
        CreateFolderStructure();
        CreateBlockPrefab();
    }

    [MenuItem("TetrisPlatformer/Generate All Pieces", false, 2)]
    public static void GenerateAllPieces_Menu()
    {
        CreateFolderStructure();
        GenerateAllPieces();
    }

    [MenuItem("TetrisPlatformer/Create BlockSpriteSet", false, 3)]
    public static void CreateBlockSpriteSet()
    {
        string path = "Assets/Scriptables/BlockSprites/NewBlockSpriteSet.asset";
        int counter = 1;

        while (AssetDatabase.LoadAssetAtPath<BlockSpriteSet>(path) != null)
        {
            path = $"Assets/Scriptables/BlockSprites/NewBlockSpriteSet_{counter}.asset";
            counter++;
        }

        BlockSpriteSet spriteSet = ScriptableObject.CreateInstance<BlockSpriteSet>();
        AssetDatabase.CreateAsset(spriteSet, path);
        AssetDatabase.SaveAssets();

        Selection.activeObject = spriteSet;
        Debug.Log($"✨ BlockSpriteSet criado: {path}");
    }

    #endif
}
