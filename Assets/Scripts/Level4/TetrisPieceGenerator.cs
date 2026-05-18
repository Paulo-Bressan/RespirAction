using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Gerador automático de todas as peças clássicas do Tetris.
/// Cria os PieceShapes como ScriptableObjects na pasta especificada.
///
/// Peças geradas:
/// - I (linha de 4)
/// - O (quadrado 2x2)
/// - T (formato T)
/// - S (formato S)
/// - Z (formato Z)
/// - L (formato L)
/// - J (formato J invertido)
/// </summary>
public class TetrisPieceGenerator : MonoBehaviour
{
    #if UNITY_EDITOR

    [Header("Configuration")]
    [Tooltip("Pasta onde os PieceShapes serão criados (relativo a Assets/)")]
    public string outputFolder = "Scriptables/PieceShapes";

    [Header("Colors")]
    public Color colorI = new Color(0f, 1f, 1f);      // Ciano
    public Color colorO = new Color(1f, 1f, 0f);      // Amarelo
    public Color colorT = new Color(0.5f, 0f, 1f);    // Roxo
    public Color colorS = new Color(0f, 1f, 0f);      // Verde
    public Color colorZ = new Color(1f, 0f, 0f);      // Vermelho
    public Color colorL = new Color(1f, 0.5f, 0f);    // Laranja
    public Color colorJ = new Color(0f, 0f, 1f);      // Azul

    [ContextMenu("Generate All Tetris Pieces")]
    public void GenerateAllPieces()
    {
        // Garante que a pasta existe
        EnsureFolderExists(outputFolder);

        // Define todas as peças
        List<PieceDefinition> pieces = new List<PieceDefinition>()
        {
            // ═══════════════════════════════════════════════════════════════
            // PEÇA I - Linha de 4 blocos
            // ═══════════════════════════════════════════════════════════════
            new PieceDefinition()
            {
                name = "Piece_I",
                cells = new Vector2Int[]
                {
                    new Vector2Int(0, 0),
                    new Vector2Int(1, 0),
                    new Vector2Int(2, 0),
                    new Vector2Int(3, 0)
                },
                color = colorI,
                description = "Linha horizontal de 4 blocos"
            },

            // ═══════════════════════════════════════════════════════════════
            // PEÇA O - Quadrado 2x2
            // ═══════════════════════════════════════════════════════════════
            new PieceDefinition()
            {
                name = "Piece_O",
                cells = new Vector2Int[]
                {
                    new Vector2Int(0, 0),
                    new Vector2Int(1, 0),
                    new Vector2Int(0, 1),
                    new Vector2Int(1, 1)
                },
                color = colorO,
                description = "Quadrado 2x2"
            },

            // ═══════════════════════════════════════════════════════════════
            // PEÇA T - Formato T
            // ═══════════════════════════════════════════════════════════════
            new PieceDefinition()
            {
                name = "Piece_T",
                cells = new Vector2Int[]
                {
                    new Vector2Int(0, 1),  // Topo esquerdo
                    new Vector2Int(1, 1),  // Topo centro
                    new Vector2Int(2, 1),  // Topo direito
                    new Vector2Int(1, 0)   // Centro baixo
                },
                color = colorT,
                description = "Formato T"
            },

            // ═══════════════════════════════════════════════════════════════
            // PEÇA S - Formato S
            // ═══════════════════════════════════════════════════════════════
            new PieceDefinition()
            {
                name = "Piece_S",
                cells = new Vector2Int[]
                {
                    new Vector2Int(1, 1),  // Topo centro
                    new Vector2Int(2, 1),  // Topo direito
                    new Vector2Int(0, 0),  // Baixo esquerdo
                    new Vector2Int(1, 0)   // Baixo centro
                },
                color = colorS,
                description = "Formato S"
            },

            // ═══════════════════════════════════════════════════════════════
            // PEÇA Z - Formato Z
            // ═══════════════════════════════════════════════════════════════
            new PieceDefinition()
            {
                name = "Piece_Z",
                cells = new Vector2Int[]
                {
                    new Vector2Int(0, 1),  // Topo esquerdo
                    new Vector2Int(1, 1),  // Topo centro
                    new Vector2Int(1, 0),  // Baixo centro
                    new Vector2Int(2, 0)   // Baixo direito
                },
                color = colorZ,
                description = "Formato Z"
            },

            // ═══════════════════════════════════════════════════════════════
            // PEÇA L - Formato L
            // ═══════════════════════════════════════════════════════════════
            new PieceDefinition()
            {
                name = "Piece_L",
                cells = new Vector2Int[]
                {
                    new Vector2Int(0, 1),  // Topo esquerdo
                    new Vector2Int(0, 0),  // Baixo esquerdo
                    new Vector2Int(1, 0),  // Baixo centro
                    new Vector2Int(2, 0)   // Baixo direito
                },
                color = colorL,
                description = "Formato L"
            },

            // ═══════════════════════════════════════════════════════════════
            // PEÇA J - Formato J (L invertido)
            // ═══════════════════════════════════════════════════════════════
            new PieceDefinition()
            {
                name = "Piece_J",
                cells = new Vector2Int[]
                {
                    new Vector2Int(2, 1),  // Topo direito
                    new Vector2Int(0, 0),  // Baixo esquerdo
                    new Vector2Int(1, 0),  // Baixo centro
                    new Vector2Int(2, 0)   // Baixo direito
                },
                color = colorJ,
                description = "Formato J (L invertido)"
            }
        };

        // Cria cada peça
        int created = 0;
        foreach (var piece in pieces)
        {
            if (CreatePieceShape(piece))
                created++;
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"✅ {created} peças criadas com sucesso em Assets/{outputFolder}/");
    }

    /// <summary>
    /// Gera também variações rotacionadas de cada peça.
    /// </summary>
    [ContextMenu("Generate All Pieces With Rotations")]
    public void GenerateAllPiecesWithRotations()
    {
        EnsureFolderExists(outputFolder);

        List<PieceDefinition> allPieces = new List<PieceDefinition>();

        // Definições base
        var basePieces = new List<(string name, Vector2Int[] cells, Color color, string desc)>
        {
            ("Piece_I", new Vector2Int[] {
                new Vector2Int(0, 0),
                new Vector2Int(1, 0),
                new Vector2Int(2, 0),
                new Vector2Int(3, 0)
            }, colorI, "I - Horizontal"),

            ("Piece_O", new Vector2Int[] {
                new Vector2Int(0, 0),
                new Vector2Int(1, 0),
                new Vector2Int(0, 1),
                new Vector2Int(1, 1)
            }, colorO, "O - Quadrado"),

            ("Piece_T", new Vector2Int[] {
                new Vector2Int(0, 1),
                new Vector2Int(1, 1),
                new Vector2Int(2, 1),
                new Vector2Int(1, 0)
            }, colorT, "T"),

            ("Piece_S", new Vector2Int[] {
                new Vector2Int(1, 1),
                new Vector2Int(2, 1),
                new Vector2Int(0, 0),
                new Vector2Int(1, 0)
            }, colorS, "S"),

            ("Piece_Z", new Vector2Int[] {
                new Vector2Int(0, 1),
                new Vector2Int(1, 1),
                new Vector2Int(1, 0),
                new Vector2Int(2, 0)
            }, colorZ, "Z"),

            ("Piece_L", new Vector2Int[] {
                new Vector2Int(0, 1),
                new Vector2Int(0, 0),
                new Vector2Int(1, 0),
                new Vector2Int(2, 0)
            }, colorL, "L"),

            ("Piece_J", new Vector2Int[] {
                new Vector2Int(2, 1),
                new Vector2Int(0, 0),
                new Vector2Int(1, 0),
                new Vector2Int(2, 0)
            }, colorJ, "J")
        };

        int created = 0;

        foreach (var (name, cells, color, desc) in basePieces)
        {
            // Cria a versão base
            CreatePieceShape(new PieceDefinition()
            {
                name = name,
                cells = cells,
                color = color,
                description = desc
            });
            created++;

            // Gera rotações 90°, 180°, 270°
            Vector2Int[] currentCells = cells;

            for (int rot = 1; rot <= 3; rot++)
            {
                currentCells = RotateCells90Degrees(currentCells);
                CreatePieceShape(new PieceDefinition()
                {
                    name = $"{name}_R{rot * 90}",
                    cells = currentCells,
                    color = color,
                    description = $"{desc} (rotação {rot * 90}°)"
                });
                created++;
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"✅ {created} peças (com rotações) criadas em Assets/{outputFolder}/");
    }

    /// <summary>
    /// Rotaciona as células 90 graus no sentido horário.
    /// </summary>
    private Vector2Int[] RotateCells90Degrees(Vector2Int[] cells)
    {
        Vector2Int[] rotated = new Vector2Int[cells.Length];

        // Encontra o bounds para normalizar
        int minX = int.MaxValue, minY = int.MaxValue;

        for (int i = 0; i < cells.Length; i++)
        {
            // Rotação 90°: (x, y) -> (-y, x)
            rotated[i] = new Vector2Int(-cells[i].y, cells[i].x);

            if (rotated[i].x < minX) minX = rotated[i].x;
            if (rotated[i].y < minY) minY = rotated[i].y;
        }

        // Normaliza para origem (0,0)
        for (int i = 0; i < rotated.Length; i++)
        {
            rotated[i] = new Vector2Int(rotated[i].x - minX, rotated[i].y - minY);
        }

        return rotated;
    }

    private void EnsureFolderExists(string folderPath)
    {
        string fullPath = "Assets";
        string[] folders = folderPath.Split('/');

        foreach (string folder in folders)
        {
            string currentPath = fullPath + "/" + folder;
            if (!AssetDatabase.IsValidFolder(currentPath))
            {
                AssetDatabase.CreateFolder(fullPath, folder);
            }
            fullPath = currentPath;
        }
    }

    private bool CreatePieceShape(PieceDefinition definition)
    {
        string fullPath = $"Assets/{outputFolder}/{definition.name}.asset";

        // Verifica se já existe
        PieceShape existing = AssetDatabase.LoadAssetAtPath<PieceShape>(fullPath);
        if (existing != null)
        {
            // Atualiza existente
            existing.cells = definition.cells;
            existing.debugColor = definition.color;
            EditorUtility.SetDirty(existing);
            Debug.Log($"🔄 Atualizado: {definition.name} - {definition.description}");
            return true;
        }

        // Cria novo
        PieceShape piece = ScriptableObject.CreateInstance<PieceShape>();
        piece.cells = definition.cells;
        piece.debugColor = definition.color;

        AssetDatabase.CreateAsset(piece, fullPath);
        Debug.Log($"✨ Criado: {definition.name} - {definition.description}");
        return true;
    }

    /// <summary>
    /// Visualiza as peças no Inspector
    /// </summary>
    [ContextMenu("Preview All Pieces")]
    public void PreviewAllPieces()
    {
        Debug.Log("═════════════════════════════════════════════════════");
        Debug.Log("PREVIEW DAS PEÇAS DO TETRIS");
        Debug.Log("═════════════════════════════════════════════════════");

        string[] pieceNames = new string[] { "Piece_I", "Piece_O", "Piece_T", "Piece_S", "Piece_Z", "Piece_L", "Piece_J" };

        foreach (string pieceName in pieceNames)
        {
            string path = $"Assets/{outputFolder}/{pieceName}.asset";
            PieceShape piece = AssetDatabase.LoadAssetAtPath<PieceShape>(path);

            if (piece != null)
            {
                Debug.Log($" {pieceName}:\n{VisualizePiece(piece.cells)}");
            }
        }

        Debug.Log("═════════════════════════════════════════════════════");
    }

    private string VisualizePiece(Vector2Int[] cells)
    {
        // Encontra bounds
        int minX = int.MaxValue, maxX = int.MinValue;
        int minY = int.MaxValue, maxY = int.MinValue;

        foreach (var cell in cells)
        {
            if (cell.x < minX) minX = cell.x;
            if (cell.x > maxX) maxX = cell.x;
            if (cell.y < minY) minY = cell.y;
            if (cell.y > maxY) maxY = cell.y;
        }

        // Cria grid visual
        string result = "";
        for (int y = maxY; y >= minY; y--)
        {
            string line = "";
            for (int x = minX; x <= maxX; x++)
            {
                bool found = false;
                foreach (var cell in cells)
                {
                    if (cell.x == x && cell.y == y)
                    {
                        found = true;
                        break;
                    }
                }
                line += found ? "■ " : "□ ";
            }
            result += line + "\n";
        }
        return result;
    }

    // Classe auxiliar para definição de peça
    private class PieceDefinition
    {
        public string name;
        public Vector2Int[] cells;
        public Color color;
        public string description;
    }

    #endif
}
