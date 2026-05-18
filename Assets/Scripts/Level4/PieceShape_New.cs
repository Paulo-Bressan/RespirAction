using UnityEngine;

[CreateAssetMenu(menuName = "TetrisPlatformer/Piece Shape")]
public class PieceShape : ScriptableObject
{
    [Header("Shape Definition")]
    [Tooltip("Células que compõem a peça, relativas à origem (0,0)")]
    public Vector2Int[] cells;

    [Header("Visual Configuration")]
    [Tooltip("SpriteSet específico para este tipo de peça. Se vazio, usa o default do PieceSpawner.")]
    public BlockSpriteSet spriteSet;

    [Header("Debug")]
    public Color debugColor = Color.white;

    /// <summary>
    /// Retorna os limites da peça (minX, maxX, minY, maxY)
    /// </summary>
    public Vector4 GetBounds()
    {
        if (cells == null || cells.Length == 0)
            return Vector4.zero;

        int minX = int.MaxValue, maxX = int.MinValue;
        int minY = int.MaxValue, maxY = int.MinValue;

        foreach (var cell in cells)
        {
            if (cell.x < minX) minX = cell.x;
            if (cell.x > maxX) maxX = cell.x;
            if (cell.y < minY) minY = cell.y;
            if (cell.y > maxY) maxY = cell.y;
        }

        return new Vector4(minX, maxX, minY, maxY);
    }

    /// <summary>
    /// Retorna o tamanho da peça (width, height)
    /// </summary>
    public Vector2Int GetSize()
    {
        Vector4 bounds = GetBounds();
        return new Vector2Int(
            (int)(bounds.y - bounds.x) + 1,
            (int)(bounds.w - bounds.z) + 1
        );
    }
}
