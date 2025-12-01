using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

public class FixVisibilityTool : EditorWindow
{
    [MenuItem("Ferramentas/Corrigir Visibilidade (Bounds e Z)")]
    static void FixVisibility()
    {
        // 1. Corrigir Tilemaps (Muitas vezes o Tilemap 'pensa' que é menor do que é)
        Tilemap[] tilemaps = FindObjectsOfType<Tilemap>();
        foreach (var tilemap in tilemaps)
        {
            // Força o Tilemap a recalcular o tamanho baseado nos tiles pintados
            tilemap.CompressBounds(); 
            
            // Opcional: Garante que o Tilemap está no Z=0 se for puramente 2D
            // (Comenta as 3 linhas abaixo se usares profundidade customizada)
            Vector3 pos = tilemap.transform.position;
            pos.z = 0;
            tilemap.transform.position = pos;

            EditorUtility.SetDirty(tilemap);
        }

        // 2. Opcional: Corrigir Sprites (Plataformas soltas)
        SpriteRenderer[] sprites = FindObjectsOfType<SpriteRenderer>();
        foreach (var sprite in sprites)
        {
            // Garante que sprites soltos também estão no Z=0 para não serem cortados pela câmara
            Vector3 pos = sprite.transform.position;
            pos.z = 0;
            sprite.transform.position = pos;
            
            EditorUtility.SetDirty(sprite);
        }

        Debug.Log($"Visibilidade corrigida! {tilemaps.Length} Tilemaps recalculados e {sprites.Length} Sprites alinhados.");
    }
}