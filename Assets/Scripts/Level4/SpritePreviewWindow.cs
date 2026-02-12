using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// Janela do Editor para visualizar e testar sprites de auto-tiling.
/// Permite ver como os sprites se conectam antes de executar o jogo.
///
 /// Acesse via menu: Window → TetrisPlatformer → Sprite Preview Window
///
 /// IMPORTANTE: Este script deve ficar em uma pasta chamada "Editor" no seu projeto Unity.
/// </summary>
public class SpritePreviewWindow : EditorWindow
{
    // ═══════════════════════════════════════════════════════════════════════════
    // CONFIGURAÇÕES
    // ═══════════════════════════════════════════════════════════════════════════

    private BlockSpriteSet spriteSet;
    private int selectedSpriteIndex = -1;

    // Configurações de visualização
    private int previewGridSize = 5;
    private int[,] previewGrid;
    private bool showGridLines = true;
    private float spriteScale = 2f;

    // Scroll positions
    private Vector2 mainScrollPosition;

    // Cores
    private Color gridLineColor = new Color(0.3f, 0.3f, 0.3f, 0.5f);
    private Color selectedColor = new Color(0.5f, 0.8f, 1f, 0.3f);

    // Presets de teste
    private int selectedPreset = 0;
    private readonly string[] presetNames = new string[]
    {
        "Linha Horizontal",
        "Linha Vertical",
        "Cruz",
        "Quadrado",
        "Peça T",
        "Peça L",
        "Peça S",
        "Formato U",
        "Formato C",
        "Padrão Diagonal",
        "Labirinto Simples",
        "Customizado"
    };

    // Controle de drag
    private bool isDragging = false;
    private int dragValue = 1;

    // ═══════════════════════════════════════════════════════════════════════════
    // INICIALIZAÇÃO
    // ═══════════════════════════════════════════════════════════════════════════

    [MenuItem("Window/TetrisPlatformer/Sprite Preview Window")]
    public static void ShowWindow()
    {
        SpritePreviewWindow window = GetWindow<SpritePreviewWindow>("Sprite Preview");
        window.minSize = new Vector2(900, 700);
        window.Show();
    }

    private void OnEnable()
    {
        previewGrid = new int[previewGridSize, previewGridSize];
        LoadPreset(0);
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // INTERFACE PRINCIPAL
    // ═══════════════════════════════════════════════════════════════════════════

    private void OnGUI()
    {
        mainScrollPosition = EditorGUILayout.BeginScrollView(mainScrollPosition);

        // Título
        EditorGUILayout.Space(10);
        GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 18,
            alignment = TextAnchor.MiddleCenter
        };
        GUILayout.Label("🧩 Visualizador de Sprites Auto-Tiling", titleStyle);
        GUILayout.Label("Visualize e teste seus sprites antes de executar o jogo", EditorStyles.centeredGreyMiniLabel);
        EditorGUILayout.Space(10);

        // Área de seleção do SpriteSet
        DrawSpriteSetSelection();

        EditorGUILayout.Space(10);

        if (spriteSet == null)
        {
            EditorGUILayout.HelpBox("Selecione um BlockSpriteSet para visualizar os sprites.\n\n" +
                "Se não tiver um, use os botões abaixo para criar.", MessageType.Info);
            DrawQuickCreateButtons();
        }
        else
        {
            // Divide em duas colunas principais
            EditorGUILayout.BeginHorizontal();

            // ═══════════════════════════════════════════════════════════════════════
            // COLUNA ESQUERDA: Grade de sprites
            // ═══════════════════════════════════════════════════════════════════════
            EditorGUILayout.BeginVertical(GUILayout.Width(380));
            DrawAllSpritesGrid();
            DrawSelectedSpriteDetails();
            EditorGUILayout.EndVertical();

            // ═══════════════════════════════════════════════════════════════════════
            // COLUNA DIREITA: Preview interativo
            // ═══════════════════════════════════════════════════════════════════════
            EditorGUILayout.BeginVertical();
            DrawInteractivePreview();
            DrawPresetButtons();
            DrawProblemDetector();
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.Space(15);
        DrawHelpSection();

        EditorGUILayout.EndScrollView();
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // SELEÇÃO DE SPRITESET
    // ═══════════════════════════════════════════════════════════════════════════

    private void DrawSpriteSetSelection()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("📦 Block SpriteSet", EditorStyles.boldLabel, GUILayout.Width(150));

        spriteSet = (BlockSpriteSet)EditorGUILayout.ObjectField(
            spriteSet,
            typeof(BlockSpriteSet),
            false,
            GUILayout.Height(30)
        );
        EditorGUILayout.EndHorizontal();

        // Status dos sprites
        if (spriteSet != null)
        {
            int defined = 0;
            for (int i = 0; i < 16; i++)
            {
                if (spriteSet.sprites[i] != null) defined++;
            }

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(150);

            if (defined == 16)
            {
                EditorGUILayout.HelpBox($"✅ Todos os 16 sprites definidos!", MessageType.Info);
            }
            else
            {
                EditorGUILayout.HelpBox($"⚠️ Apenas {defined}/16 sprites definidos", MessageType.Warning);
            }
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndVertical();
    }

    private void DrawQuickCreateButtons()
    {
        EditorGUILayout.Space(20);
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Criar BlockSpriteSet Vazio", GUILayout.Width(180), GUILayout.Height(40)))
        {
            string path = EditorUtility.SaveFilePanelInProject(
                "Criar BlockSpriteSet",
                "NewBlockSpriteSet",
                "asset",
                "Escolha onde salvar o BlockSpriteSet"
            );

            if (!string.IsNullOrEmpty(path))
            {
                BlockSpriteSet newSet = ScriptableObject.CreateInstance<BlockSpriteSet>();
                AssetDatabase.CreateAsset(newSet, path);
                AssetDatabase.SaveAssets();
                spriteSet = newSet;
                Debug.Log($"✅ BlockSpriteSet criado em: {path}");
            }
        }

        GUILayout.Space(10);

        

        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space(20);
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // GRADE DE SPRITES
    // ═══════════════════════════════════════════════════════════════════════════

    private void DrawAllSpritesGrid()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("🎨 Todos os Sprites (Clique para selecionar)", EditorStyles.boldLabel);

        EditorGUILayout.Space(5);

        // Grade 4x4
        for (int row = 0; row < 4; row++)
        {
            EditorGUILayout.BeginHorizontal();

            for (int col = 0; col < 4; col++)
            {
                int index = row * 4 + col;
                DrawSpriteCell(index);
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndVertical();
    }

    private void DrawSpriteCell(int index)
    {
        float cellSize = 75f;

        EditorGUILayout.BeginVertical(GUILayout.Width(cellSize + 5));

        // Background color para seleção
        Color originalBg = GUI.backgroundColor;
        if (selectedSpriteIndex == index)
        {
            GUI.backgroundColor = new Color(0.3f, 0.7f, 1f, 0.5f);
        }

        // Área do sprite
        Rect cellRect = GUILayoutUtility.GetRect(cellSize, cellSize, GUILayout.Width(cellSize), GUILayout.Height(cellSize));

        // Desenha fundo
        GUI.Box(cellRect, "", GUI.skin.button);

        // Desenha o sprite
        if (spriteSet.sprites[index] != null)
        {
            Sprite sprite = spriteSet.sprites[index];
            Texture2D tex = sprite.texture;
            Rect texRect = sprite.rect;

            Vector2 uvMin = new Vector2(texRect.x / tex.width, texRect.y / tex.height);
            Vector2 uvMax = new Vector2((texRect.x + texRect.width) / tex.width,
                                        (texRect.y + texRect.height) / tex.height);

            // Ajusta para orientação correta
            Rect uvRect = new Rect(uvMin.x, uvMin.y, uvMax.x - uvMin.x, uvMax.y - uvMin.y);

            GUI.DrawTextureWithTexCoords(cellRect, tex, uvRect);
        }
        else
        {
            // Placeholder para sprite não definido
            EditorGUI.DrawRect(cellRect, new Color(0.25f, 0.25f, 0.25f));

            GUIStyle style = new GUIStyle(EditorStyles.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 24,
                normal = { textColor = new Color(0.5f, 0.5f, 0.5f) }
            };
            GUI.Label(cellRect, "?", style);
        }

        // Número do índice
        GUIStyle indexStyle = new GUIStyle(EditorStyles.label)
        {
            alignment = TextAnchor.LowerCenter,
            fontSize = 10,
            fontStyle = FontStyle.Bold,
            normal = { textColor = Color.white }
        };
        GUI.Label(new Rect(cellRect.x, cellRect.yMax - 15, cellRect.width, 15), index.ToString(), indexStyle);

        // Descrição das direções
        string directions = GetDirectionsLabel(index);
        GUIStyle dirStyle = new GUIStyle(EditorStyles.label)
        {
            alignment = TextAnchor.UpperCenter,
            fontSize = 8,
            normal = { textColor = new Color(0.7f, 0.7f, 0.7f) }
        };
        GUI.Label(new Rect(cellRect.x, cellRect.y + 2, cellRect.width, 12), directions, dirStyle);

        GUI.backgroundColor = originalBg;

        // Clique para selecionar
        if (Event.current.type == EventType.MouseDown && cellRect.Contains(Event.current.mousePosition))
        {
            selectedSpriteIndex = index;
            Repaint();
        }

        EditorGUILayout.EndVertical();
    }

    private string GetDirectionsLabel(int index)
    {
        string dirs = "";
        if ((index & 1) != 0) dirs += "↑";
        if ((index & 2) != 0) dirs += "→";
        if ((index & 4) != 0) dirs += "↓";
        if ((index & 8) != 0) dirs += "←";
        return dirs.Length > 0 ? dirs : "○";
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // DETALHES DO SPRITE SELECIONADO
    // ═══════════════════════════════════════════════════════════════════════════

    private void DrawSelectedSpriteDetails()
    {
        if (selectedSpriteIndex < 0)
        {
            EditorGUILayout.HelpBox("Selecione um sprite para ver detalhes", MessageType.Info);
            return;
        }

        EditorGUILayout.Space(10);
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("🔍 Sprite Selecionado", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();

        // Preview grande
        Rect previewRect = GUILayoutUtility.GetRect(120, 120, GUILayout.Width(120), GUILayout.Height(120));

        if (spriteSet.sprites[selectedSpriteIndex] != null)
        {
            Sprite sprite = spriteSet.sprites[selectedSpriteIndex];
            Texture2D tex = sprite.texture;
            Rect texRect = sprite.rect;

            EditorGUI.DrawRect(previewRect, new Color(0.2f, 0.2f, 0.2f));

            Vector2 uvMin = new Vector2(texRect.x / tex.width, texRect.y / tex.height);
            Vector2 uvMax = new Vector2((texRect.x + texRect.width) / tex.width,
                                        (texRect.y + texRect.height) / tex.height);

            GUI.DrawTextureWithTexCoords(previewRect, tex, new Rect(uvMin.x, uvMin.y, uvMax.x - uvMin.x, uvMax.y - uvMin.y));
        }
        else
        {
            EditorGUI.DrawRect(previewRect, new Color(0.3f, 0.3f, 0.3f));
            GUIStyle style = new GUIStyle(EditorStyles.label) { alignment = TextAnchor.MiddleCenter };
            GUI.Label(previewRect, "Não definido", style);
        }

        // Informações
        EditorGUILayout.BeginVertical();

        EditorGUILayout.LabelField($"<b>Índice:</b> {selectedSpriteIndex}", new GUIStyle(EditorStyles.label) { richText = true });
        EditorGUILayout.LabelField($"<b>Bitmask:</b> {selectedSpriteIndex} ({System.Convert.ToString(selectedSpriteIndex, 2).PadLeft(4, '0')})", new GUIStyle(EditorStyles.label) { richText = true });
        EditorGUILayout.LabelField($"<b>Vizinhos:</b> {GetBitmaskDescription(selectedSpriteIndex)}", new GUIStyle(EditorStyles.label) { richText = true });

        EditorGUILayout.Space(5);

        if (spriteSet.sprites[selectedSpriteIndex] != null)
        {
            Sprite sprite = spriteSet.sprites[selectedSpriteIndex];
            EditorGUILayout.LabelField($"<b>Nome:</b> {sprite.name}", new GUIStyle(EditorStyles.label) { richText = true });
            EditorGUILayout.LabelField($"<b>Tamanho:</b> {sprite.rect.width}x{sprite.rect.height} px", new GUIStyle(EditorStyles.label) { richText = true });
            EditorGUILayout.LabelField($"<b>Pivot:</b> ({sprite.pivot.x:F1}, {sprite.pivot.y:F1})", new GUIStyle(EditorStyles.label) { richText = true });
        }
        else
        {
            EditorGUILayout.HelpBox("⚠️ Sprite não definido!", MessageType.Warning);
        }

        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
    }

    private string GetBitmaskDescription(int mask)
    {
        List<string> dirs = new List<string>();
        if ((mask & 1) != 0) dirs.Add("Cima");
        if ((mask & 2) != 0) dirs.Add("Direita");
        if ((mask & 4) != 0) dirs.Add("Baixo");
        if ((mask & 8) != 0) dirs.Add("Esquerda");
        return dirs.Count > 0 ? string.Join(", ", dirs) : "Nenhum (isolado)";
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // PREVIEW INTERATIVO
    // ═══════════════════════════════════════════════════════════════════════════

    private void DrawInteractivePreview()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("🎮 Preview Interativo (Clique para desenhar)", EditorStyles.boldLabel);

        // Controles
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Tamanho:", GUILayout.Width(60));
        previewGridSize = EditorGUILayout.IntSlider(previewGridSize, 3, 8, GUILayout.Width(150));
        GUILayout.Label("Zoom:", GUILayout.Width(50));
        spriteScale = EditorGUILayout.Slider(spriteScale, 1f, 3f, GUILayout.Width(150));
        showGridLines = EditorGUILayout.Toggle("Grid", showGridLines);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(5);

        // Área de preview
        float cellSize = 32f * spriteScale;
        float gridWidth = previewGridSize * cellSize;
        float gridHeight = previewGridSize * cellSize;

        Rect containerRect = GUILayoutUtility.GetRect(gridWidth + 20, gridHeight + 20);

        // Fundo escuro
        EditorGUI.DrawRect(new Rect(containerRect.x, containerRect.y, gridWidth + 20, gridHeight + 20),
            new Color(0.12f, 0.12f, 0.12f));

        float startX = containerRect.x + 10;
        float startY = containerRect.y + 10;

        // Processa eventos do mouse
        ProcessMouseEvents(startX, startY, cellSize);

        // Desenha o grid
        for (int y = 0; y < previewGridSize; y++)
        {
            for (int x = 0; x < previewGridSize; x++)
            {
                Rect cellRect = new Rect(
                    startX + x * cellSize,
                    startY + (previewGridSize - 1 - y) * cellSize,
                    cellSize,
                    cellSize
                );

                // Desenha a célula
                if (previewGrid[x, y] == 1)
                {
                    // Célula ocupada - desenha sprite com bitmask
                    int bitmask = CalculateBitmask(x, y);
                    DrawSpriteInPreviewCell(cellRect, bitmask);
                }
                else
                {
                    // Célula vazia
                    EditorGUI.DrawRect(cellRect, new Color(0.18f, 0.18f, 0.18f));
                }

                // Linhas do grid
                if (showGridLines)
                {
                    Handles.color = new Color(0.3f, 0.3f, 0.3f, 0.5f);
                    Handles.DrawPolyLine(
                        new Vector3(cellRect.x, cellRect.y, 0),
                        new Vector3(cellRect.xMax, cellRect.y, 0),
                        new Vector3(cellRect.xMax, cellRect.yMax, 0),
                        new Vector3(cellRect.x, cellRect.yMax, 0),
                        new Vector3(cellRect.x, cellRect.y, 0)
                    );
                }
            }
        }

        // Mostra bitmask no canto
        GUIStyle infoStyle = new GUIStyle(EditorStyles.label)
        {
            normal = { textColor = new Color(0.6f, 0.6f, 0.6f) },
            fontSize = 10
        };
        GUI.Label(new Rect(containerRect.xMax - 100, containerRect.y + 5, 95, 20),
            $"Grid: {previewGridSize}x{previewGridSize}", infoStyle);

        EditorGUILayout.EndVertical();
    }

    private void ProcessMouseEvents(float startX, float startY, float cellSize)
    {
        Event e = Event.current;

        if (e.type == EventType.MouseDown || (e.type == EventType.MouseDrag && isDragging))
        {
            Vector2 mousePos = e.mousePosition;

            for (int y = 0; y < previewGridSize; y++)
            {
                for (int x = 0; x < previewGridSize; x++)
                {
                    Rect cellRect = new Rect(
                        startX + x * cellSize,
                        startY + (previewGridSize - 1 - y) * cellSize,
                        cellSize,
                        cellSize
                    );

                    if (cellRect.Contains(mousePos))
                    {
                        if (e.type == EventType.MouseDown)
                        {
                            isDragging = true;
                            dragValue = (previewGrid[x, y] == 1) ? 0 : 1;
                        }
                        previewGrid[x, y] = dragValue;
                        Repaint();
                        e.Use();
                        return;
                    }
                }
            }
        }

        if (e.type == EventType.MouseUp)
        {
            isDragging = false;
        }
    }

    private void DrawSpriteInPreviewCell(Rect cellRect, int bitmask)
    {
        if (spriteSet.sprites[bitmask] != null)
        {
            Sprite sprite = spriteSet.sprites[bitmask];
            Texture2D tex = sprite.texture;
            Rect texRect = sprite.rect;

            Vector2 uvMin = new Vector2(texRect.x / tex.width, texRect.y / tex.height);
            Vector2 uvMax = new Vector2((texRect.x + texRect.width) / tex.width,
                                        (texRect.y + texRect.height) / tex.height);

            GUI.DrawTextureWithTexCoords(cellRect, tex, new Rect(uvMin.x, uvMin.y, uvMax.x - uvMin.x, uvMax.y - uvMin.y));

            // Mostra o bitmask no canto da célula
            GUIStyle maskStyle = new GUIStyle(EditorStyles.label)
            {
                fontSize = Mathf.RoundToInt(8 * spriteScale),
                normal = { textColor = new Color(1f, 1f, 1f, 0.7f) }
            };
            GUI.Label(new Rect(cellRect.x + 2, cellRect.y + 2, 20, 15), bitmask.ToString(), maskStyle);
        }
        else
        {
            // Fallback colorido
            float hue = bitmask / 16f;
            Color fallbackColor = Color.HSVToRGB(hue, 0.6f, 0.7f);
            EditorGUI.DrawRect(cellRect, fallbackColor);

            GUIStyle style = new GUIStyle(EditorStyles.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = Mathf.RoundToInt(14 * spriteScale),
                normal = { textColor = Color.white }
            };
            GUI.Label(cellRect, bitmask.ToString(), style);
        }
    }

    private int CalculateBitmask(int x, int y)
    {
        int mask = 0;

        if (y + 1 < previewGridSize && previewGrid[x, y + 1] == 1) mask |= 1;  // Cima
        if (x + 1 < previewGridSize && previewGrid[x + 1, y] == 1) mask |= 2;  // Direita
        if (y - 1 >= 0 && previewGrid[x, y - 1] == 1) mask |= 4;                // Baixo
        if (x - 1 >= 0 && previewGrid[x - 1, y] == 1) mask |= 8;               // Esquerda

        return mask;
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // PRESETS
    // ═══════════════════════════════════════════════════════════════════════════

    private void DrawPresetButtons()
    {
        EditorGUILayout.Space(5);
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("📋 Presets de Teste", EditorStyles.boldLabel);

        int newPreset = GUILayout.SelectionGrid(selectedPreset, presetNames, 3);

        if (newPreset != selectedPreset)
        {
            selectedPreset = newPreset;
            previewGrid = new int[previewGridSize, previewGridSize];
            LoadPreset(selectedPreset);
        }

        // Botões de ação
        EditorGUILayout.Space(5);
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("🗑️ Limpar", GUILayout.Height(25)))
        {
            for (int x = 0; x < previewGridSize; x++)
                for (int y = 0; y < previewGridSize; y++)
                    previewGrid[x, y] = 0;
        }

        if (GUILayout.Button("📦 Preencher", GUILayout.Height(25)))
        {
            for (int x = 0; x < previewGridSize; x++)
                for (int y = 0; y < previewGridSize; y++)
                    previewGrid[x, y] = 1;
        }

        if (GUILayout.Button("🔲 Bordas", GUILayout.Height(25)))
        {
            for (int x = 0; x < previewGridSize; x++)
            {
                for (int y = 0; y < previewGridSize; y++)
                {
                    previewGrid[x, y] = (x == 0 || x == previewGridSize - 1 ||
                                        y == 0 || y == previewGridSize - 1) ? 1 : 0;
                }
            }
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
    }

    private void LoadPreset(int presetIndex)
    {
        switch (presetIndex)
        {
            case 0: // Linha Horizontal
                for (int x = 0; x < previewGridSize; x++)
                    previewGrid[x, previewGridSize / 2] = 1;
                break;

            case 1: // Linha Vertical
                for (int y = 0; y < previewGridSize; y++)
                    previewGrid[previewGridSize / 2, y] = 1;
                break;

            case 2: // Cruz
                for (int i = 0; i < previewGridSize; i++)
                {
                    previewGrid[i, previewGridSize / 2] = 1;
                    previewGrid[previewGridSize / 2, i] = 1;
                }
                break;

            case 3: // Quadrado
                int half = previewGridSize / 2;
                for (int x = half - 1; x <= half + 1; x++)
                    for (int y = half - 1; y <= half + 1; y++)
                        if (x >= 0 && x < previewGridSize && y >= 0 && y < previewGridSize)
                            previewGrid[x, y] = 1;
                break;

            case 4: // Peça T
                previewGrid[previewGridSize / 2 - 1, previewGridSize - 2] = 1;
                previewGrid[previewGridSize / 2, previewGridSize - 2] = 1;
                previewGrid[previewGridSize / 2 + 1, previewGridSize - 2] = 1;
                previewGrid[previewGridSize / 2, previewGridSize - 3] = 1;
                break;

            case 5: // Peça L
                previewGrid[previewGridSize / 2 - 1, previewGridSize - 2] = 1;
                previewGrid[previewGridSize / 2 - 1, previewGridSize - 3] = 1;
                previewGrid[previewGridSize / 2, previewGridSize - 3] = 1;
                previewGrid[previewGridSize / 2 + 1, previewGridSize - 3] = 1;
                break;

            case 6: // Peça S
                previewGrid[previewGridSize / 2, previewGridSize - 2] = 1;
                previewGrid[previewGridSize / 2 + 1, previewGridSize - 2] = 1;
                previewGrid[previewGridSize / 2 - 1, previewGridSize - 3] = 1;
                previewGrid[previewGridSize / 2, previewGridSize - 3] = 1;
                break;

            case 7: // Formato U
                for (int y = 0; y < previewGridSize - 1; y++)
                {
                    previewGrid[1, y] = 1;
                    previewGrid[previewGridSize - 2, y] = 1;
                }
                previewGrid[previewGridSize / 2, 0] = 1;
                if (previewGridSize >= 5)
                {
                    previewGrid[2, 0] = 1;
                }
                break;

            case 8: // Formato C
                for (int y = 0; y < previewGridSize; y++)
                    previewGrid[1, y] = 1;
                previewGrid[2, previewGridSize - 1] = 1;
                previewGrid[2, 0] = 1;
                break;

            case 9: // Diagonal
                for (int i = 0; i < previewGridSize; i++)
                    previewGrid[i, i] = 1;
                break;

            case 10: // Labirinto Simples
                for (int x = 0; x < previewGridSize; x++)
                {
                    previewGrid[x, previewGridSize - 1] = 1;
                    if (x % 2 == 0)
                    {
                        for (int y = previewGridSize - 2; y >= previewGridSize - 3; y--)
                            previewGrid[x, y] = 1;
                    }
                }
                break;

            case 11: // Customizado
                break;
        }
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // DETECTOR DE PROBLEMAS
    // ═══════════════════════════════════════════════════════════════════════════

    private void DrawProblemDetector()
    {
        EditorGUILayout.Space(5);
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("🔧 Detector de Problemas", EditorStyles.boldLabel);

        List<string> problems = new List<string>();

        // Verifica sprites faltando
        int missingCount = 0;
        for (int i = 0; i < 16; i++)
        {
            if (spriteSet.sprites[i] == null) missingCount++;
        }

        if (missingCount > 0)
        {
            problems.Add($"• {missingCount} sprites não definidos");
        }

        // Verifica tamanhos inconsistentes
        if (spriteSet.sprites[0] != null)
        {
            int refWidth = Mathf.RoundToInt(spriteSet.sprites[0].rect.width);
            int refHeight = Mathf.RoundToInt(spriteSet.sprites[0].rect.height);

            for (int i = 1; i < 16; i++)
            {
                if (spriteSet.sprites[i] != null)
                {
                    int w = Mathf.RoundToInt(spriteSet.sprites[i].rect.width);
                    int h = Mathf.RoundToInt(spriteSet.sprites[i].rect.height);

                    if (w != refWidth || h != refHeight)
                    {
                        problems.Add($"• Sprite {i} tem tamanho diferente ({w}x{h} vs {refWidth}x{refHeight})");
                    }
                }
            }
        }

        // Verifica problemas no preview atual
        int blockCount = 0;
        for (int x = 0; x < previewGridSize; x++)
            for (int y = 0; y < previewGridSize; y++)
                if (previewGrid[x, y] == 1) blockCount++;

        if (blockCount == 0)
        {
            problems.Add("• Nenhum bloco no preview. Use os presets ou desenhe.");
        }

        // Mostra resultados
        if (problems.Count == 0)
        {
            EditorGUILayout.HelpBox("✅ Nenhum problema detectado!\nSeus sprites parecem estar configurados corretamente.", MessageType.Info);
        }
        else
        {
            EditorGUILayout.HelpBox(string.Join("\n", problems), MessageType.Warning);
        }

        EditorGUILayout.EndVertical();
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // AJUDA
    // ═══════════════════════════════════════════════════════════════════════════

    private void DrawHelpSection()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        EditorGUILayout.LabelField("💡 Como Usar", EditorStyles.boldLabel);

        EditorGUILayout.Space(5);

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("1.", GUILayout.Width(15));
        GUILayout.Label("Selecione ou crie um BlockSpriteSet");
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("2.", GUILayout.Width(15));
        GUILayout.Label("Clique nos sprites à esquerda para ver detalhes");
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("3.", GUILayout.Width(15));
        GUILayout.Label("Desenhe no grid de preview para testar conexões");
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("4.", GUILayout.Width(15));
        GUILayout.Label("Observe os números (bitmask) em cada bloco - eles indicam qual sprite é usado");
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("5.", GUILayout.Width(15));
        GUILayout.Label("Se houver gaps visuais, ajuste os sprites correspondentes");
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(10);

        // Tabela de referência rápida
        EditorGUILayout.LabelField("📊 Tabela de Referência Rápida", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();

        // Coluna 1
        EditorGUILayout.BeginVertical();
        for (int i = 0; i < 8; i++)
        {
            EditorGUILayout.LabelField($"{i:D2}: {GetDirectionsLabel(i)}", EditorStyles.miniLabel);
        }
        EditorGUILayout.EndVertical();

        // Coluna 2
        EditorGUILayout.BeginVertical();
        for (int i = 8; i < 16; i++)
        {
            EditorGUILayout.LabelField($"{i:D2}: {GetDirectionsLabel(i)}", EditorStyles.miniLabel);
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(5);
        GUILayout.Label("Bitmask: ↑=1, →=2, ↓=4, ←=8 (soma para obter o índice)", EditorStyles.centeredGreyMiniLabel);

        EditorGUILayout.EndVertical();
    }
}
