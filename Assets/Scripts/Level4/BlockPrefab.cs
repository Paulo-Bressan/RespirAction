using UnityEngine;

/// <summary>
/// Script auxiliar para criar o prefab de bloco no Unity.
/// Anexe este script a um GameObject vazio e use o menu de contexto para criar o prefab.
/// </summary>
public class BlockPrefabCreator : MonoBehaviour
{
    #if UNITY_EDITOR
    [UnityEditor.MenuItem("TetrisPlatformer/Create Block Prefab")]
    public static void CreateBlockPrefab()
    {
        // Cria o GameObject do bloco
        GameObject blockPrefab = new GameObject("Block");

        // Adiciona o SpriteRenderer
        SpriteRenderer sr = blockPrefab.AddComponent<SpriteRenderer>();
        sr.sortingOrder = 0;

        // Adiciona o BlockController
        blockPrefab.AddComponent<BlockController>();

        // Salva como prefab
        string folderPath = "Assets/Prefabs";
        if (!UnityEditor.AssetDatabase.IsValidFolder(folderPath))
        {
            UnityEditor.AssetDatabase.CreateFolder("Assets", "Prefabs");
        }

        string prefabPath = folderPath + "/Block.prefab";
        UnityEditor.PrefabUtility.SaveAsPrefabAsset(blockPrefab, prefabPath);

        // Remove da cena
        DestroyImmediate(blockPrefab);

        Debug.Log($"✅ Prefab criado em: {prefabPath}");
        UnityEditor.AssetDatabase.Refresh();
    }
    #endif
}
