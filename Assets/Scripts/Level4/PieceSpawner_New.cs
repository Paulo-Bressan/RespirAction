using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceSpawner : MonoBehaviour
{
    [Header("References")]
    public GridManager grid;
    public List<PieceShape> availableShapes;
    public GameObject piecePrefab;
    
    [Tooltip("Gestor que roda o cronômetro desta fase.")]
    public Level4Manager levelManager;

    [Header("Visual Configuration")]
    [Tooltip("SpriteSet padrão para todas as peças. Pode ser sobrescrito por PieceShape.spriteSet")]
    public BlockSpriteSet defaultSpriteSet;

    [Header("Spawn Settings")]
    public float startSpawnDelay = 2f;
    public float minSpawnDelay = 0.4f;
    public float difficultyRamp = 0.05f; // redução do delay por spawn
    public int maxPlacementAttempts = 30;

    [Header("Spawn Height Logic")]
    public int baseSpawnY = 0;
    public bool allowStackingUpwards = true;

    private float currentDelay;

    void Start()
    {
        currentDelay = startSpawnDelay;
        
        if (levelManager == null) 
        {
            levelManager = FindObjectOfType<Level4Manager>();
        }

        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(currentDelay);
            TrySpawnPiece();
            currentDelay = Mathf.Max(minSpawnDelay, currentDelay - difficultyRamp);
        }
    }

    void TrySpawnPiece()
    {
        if (availableShapes == null || availableShapes.Count == 0) return;

        PieceShape shape = availableShapes[Random.Range(0, availableShapes.Count)];

        for (int attempt = 0; attempt < maxPlacementAttempts; attempt++)
        {
            int startX = Random.Range(0, grid.Width);

            int y = FindValidHeight(shape, startX);

            if (y != -1)
            {
                PlacePiece(shape, startX, y);
                return;
            }
        }

        Debug.Log("⚠️ Sem espaço pra spawnar peça! Grid entupido.");
        
        // Dispara o Game Over real no gerente da fase 4
        if (levelManager != null)
        {
            levelManager.TriggerGameOver();
        }
    }

    int FindValidHeight(PieceShape shape, int originX)
    {
        int maxCheckHeight = allowStackingUpwards ? grid.Height : 1;

        for (int y = baseSpawnY; y < maxCheckHeight; y++)
        {
            if (CanPlace(shape, originX, y))
                return y;
        }

        return -1;
    }

    bool CanPlace(PieceShape shape, int originX, int originY)
    {
        foreach (var cell in shape.cells)
        {
            int x = originX + cell.x;
            int y = originY + cell.y;

            if (!grid.IsCellFree(x, y))
                return false;
        }
        return true;
    }

    void PlacePiece(PieceShape shape, int originX, int originY)
    {
        // Determina qual spriteSet usar (do shape ou default)
        BlockSpriteSet spriteSetToUse = shape.spriteSet != null ? shape.spriteSet : defaultSpriteSet;

        // Instancia a peça
        GameObject pieceObj = Instantiate(piecePrefab);
        PieceInstance piece = pieceObj.GetComponent<PieceInstance>();

        if (piece != null)
        {
            piece.Init(shape, new Vector2Int(originX, originY), spriteSetToUse);
        }
        else
        {
            // Fallback caso o prefab não tenha PieceInstance
            piece = pieceObj.AddComponent<PieceInstance>();
            piece.Init(shape, new Vector2Int(originX, originY), spriteSetToUse);
        }
    }
}
