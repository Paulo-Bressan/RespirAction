using UnityEngine;

/// <summary>
/// ScriptableObject que armazena os 16 sprites possíveis para um tipo de bloco.
/// Cada sprite corresponde a uma configuração de vizinhos (bitmask 0-15).
/// 
/// Valores da bitmask:
/// - Cima = 1
/// - Direita = 2
/// - Baixo = 4
/// - Esquerda = 8
/// 
/// Exemplos:
/// - Índice 0 (0000): Sem vizinhos - bloco isolado
/// - Índice 3 (0011): Cima + Direita = curva
/// - Índice 5 (0101): Cima + Baixo = conexão vertical
/// - Índice 10 (1010): Direita + Esquerda = conexão horizontal
/// - Índice 15 (1111): Todos os lados - centro preenchido
/// </summary>
[CreateAssetMenu(menuName = "TetrisPlatformer/Block Sprite Set")]
public class BlockSpriteSet : ScriptableObject
{
    [Header("Sprites para cada configuração de vizinhos")]
    [Tooltip("Array de 16 sprites. O índice corresponde à soma dos valores de direção dos vizinhos.")]
    public Sprite[] sprites = new Sprite[16];

    // Valores de direção para a bitmask
    public const int UP = 1;
    public const int RIGHT = 2;
    public const int DOWN = 4;
    public const int LEFT = 8;

    /// <summary>
    /// Retorna o sprite apropriado baseado na bitmask de vizinhos.
    /// </summary>
    /// <param name="bitmask">Soma dos valores de direção (0-15)</param>
    /// <returns>O sprite correspondente ou null se não definido</returns>
    public Sprite GetSprite(int bitmask)
    {
        bitmask = Mathf.Clamp(bitmask, 0, 15);
        return sprites[bitmask];
    }

    /// <summary>
    /// Calcula a bitmask baseado nos vizinhos presentes.
    /// </summary>
    public static int CalculateBitmask(bool hasUp, bool hasRight, bool hasDown, bool hasLeft)
    {
        int mask = 0;
        if (hasUp) mask |= UP;
        if (hasRight) mask |= RIGHT;
        if (hasDown) mask |= DOWN;
        if (hasLeft) mask |= LEFT;
        return mask;
    }

    /// <summary>
    /// Retorna uma descrição legível da configuração (útil para debug)
    /// </summary>
    public static string GetBitmaskDescription(int bitmask)
    {
        string desc = "";
        if ((bitmask & UP) != 0) desc += "Cima ";
        if ((bitmask & RIGHT) != 0) desc += "Direita ";
        if ((bitmask & DOWN) != 0) desc += "Baixo ";
        if ((bitmask & LEFT) != 0) desc += "Esquerda ";
        if (string.IsNullOrEmpty(desc)) desc = "Isolado";
        return $"[{bitmask}] {desc.Trim()}";
    }

    #if UNITY_EDITOR
    void OnValidate()
    {
        // Garante que o array sempre tem 16 posições
        if (sprites == null || sprites.Length != 16)
        {
            sprites = new Sprite[16];
        }
    }
    #endif
}
