using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Guia visual e ferramenta para criar sprites corretamente alinhados.
/// Use este script para entender exatamente como cada sprite deve ser desenhado.
/// </summary>
public class SpriteAlignmentGuide : MonoBehaviour
{
    #if UNITY_EDITOR

    [Header("Configuração do Sprite")]
    [Tooltip("Tamanho do sprite em pixels")]
    public int spriteSize = 32;

    [Tooltip("Espessura da 'rama' ou 'tubo' em pixels")]
    public int branchThickness = 8;

    [Tooltip("Padding das bordas (onde a conexão acontece)")]
    public int connectionPadding = 2;

    [Header("Preview")]
    public bool showGridPreview = true;
    public BlockSpriteSet spriteSetToPreview;

    [ContextMenu("Mostrar Guia Completo no Console")]
    public void ShowCompleteGuide()
    {
        Debug.Log(@"
═══════════════════════════════════════════════════════════════════════════════
                    GUIA DE SPRITES - SISTEMA DE AUTO-TILING
═══════════════════════════════════════════════════════════════════════════════

╔═════════════════════════════════════════════════════════════════════════════╗
║  PROBLEMA: PONTAS ABERTAS NAS CONEXÕES                                      ║
╚═════════════════════════════════════════════════════════════════════════════╝

ISSO ACONTECE QUANDO OS SPRITES NÃO ESTÃO ALINHADOS!

┌─────────────────────────────────────────────────────────────────────────────┐
│                              O PROBLEMA:                                    │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                             │
│    Sprite A tem conexão aqui:     Sprite B tem conexão aqui:               │
│    ┌────────┐                     ┌────────┐                                │
│    │    ████│ ← ponta             │████    │ ← ponta                       │
│    │        │                     │        │                               │
│    └────────┘                     └────────┘                                │
│         ↓                              ↓                                    │
│    Quando juntos, ficam com GAP:                                            │
│    ┌────────┐┌────────┐                                                    │
│    │    ████│████    │  ← GAP no meio!                                     │
│    │        │        │                                                     │
│    └────────┘└────────┘                                                    │
│                                                                             │
├─────────────────────────────────────────────────────────────────────────────┤
│                              A SOLUÇÃO:                                     │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                             │
│    ✓ A conexão deve SAIR da borda (não parar antes)                        │
│    ✓ A conexão deve ter SEMPRE a mesma espessura                           │
│    ✓ A conexão deve estar SEMPRE na mesma posição Y/X                      │
│                                                                             │
│    Sprite A correto:              Sprite B correto:                         │
│    ┌────────┐                     ┌────────┐                                │
│    │    ████│████ ← continua      ████│████ │ ← continua                   │
│    │        │                     │        │                               │
│    └────────┘                     └────────┘                                │
│         ↓                              ↓                                    │
│    Juntos, formam conexão contínua:                                         │
│    ┌────────┐┌────────┐                                                    │
│    │    ████████      │  ← Conexão perfeita!                               │
│    │        │        │                                                     │
│    └────────┘└────────┘                                                    │
│                                                                             │
└─────────────────────────────────────────────────────────────────────────────┘


╔═════════════════════════════════════════════════════════════════════════════╗
║  TIPO: BLOCOS RAMIFICADOS (ESTILO TUBO/GALHO)                               ║
╚═════════════════════════════════════════════════════════════════════════════╝

Em vez de blocos maciços, desenhe 'tubos' ou 'galhos':

┌─────────────────────────────────────────────────────────────────────────────┐
│  BLOCO MACIÇO (errado para seu caso)      BLOCO RAMA (correto)              │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                             │
│  ┌────────┐                               ┌────────┐                        │
│  │████████│                               │        │                        │
│  │████████│  ← todo preenchido            │  ████  │  ← apenas uma linha   │
│  │████████│                               │  ████  │                        │
│  │████████│                               │        │                        │
│  └────────┘                               └────────┘                        │
│                                                                             │
│  CONEXÕES:                                                                     │
│                                                                             │
│  ┌────────┐    ┌────────┐                  ┌────────┐    ┌────────┐        │
│  │████████│    │████████│                  │        │    │  ████  │        │
│  │████████│████│████████│  ← maciço        │  ████  │████│  ████  │ ← rama │
│  │████████│    │████████│                  │        │    │  ████  │        │
│  │████████│    │████████│                  │        │    │        │        │
│  └────────┘    └────────┘                  └────────┘    └────────┘        │
│                                                                             │
└─────────────────────────────────────────────────────────────────────────────┘


╔═════════════════════════════════════════════════════════════════════════════╗
║  COMO DESENHAR OS 16 SPRITES (ESTILO RAMA/TUBO)                             ║
╚═════════════════════════════════════════════════════════════════════════════╝

REGRAS IMPORTANTES:
• A 'rama' deve ter sempre a mesma ESPESSURA (ex: 8 pixels)
• A 'rama' deve estar CENTRALIZADA no sprite
• As conexões devem SAIR PELA BORDA (não parar antes!)
• Para blocos 'vazios' (sem conexões), pode ser um quadrado vazio ou nó


┌─────────────────────────────────────────────────────────────────────────────┐
│ ÍNDICE 0: ISOLADO (sem vizinhos)                                            │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                             │
│  Sprite 32x32:                                                              │
│  ┌──────────────────┐                                                       │
│  │                  │                                                       │
│  │                  │                                                       │
│  │      ████        │                                                       │
│  │      ████        │  ← Um pequeno 'nó' ou quadrado central                │
│  │      ████        │                                                       │
│  │                  │                                                       │
│  │                  │                                                       │
│  └──────────────────┘                                                       │
│                                                                             │
│  OU: um quadrado vazio com borda (como uma caixa)                          │
│                                                                             │
└─────────────────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────────────────┐
│ ÍNDICE 1: CIMA (vizinho apenas em cima)                                     │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                             │
│  ┌──────────────────┐                                                       │
│  │      ████        │  ← Conexão SAI pela borda de cima!                    │
│  │      ████        │                                                       │
│  │      ████        │                                                       │
│  │      ████        │                                                       │
│  │      ████        │                                                       │
│  │                  │                                                       │
│  │                  │  ← Parte de baixo é 'ponta' arredondada               │
│  └──────────────────┘                                                       │
│                                                                             │
│  NOTA: A conexão DEVE sair pela borda superior para conectar com o bloco   │
│        que está acima!                                                      │
│                                                                             │
└─────────────────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────────────────┐
│ ÍNDICE 2: DIREITA (vizinho apenas à direita)                                │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                             │
│  ┌──────────────────┐                                                       │
│  │                  │                                                       │
│  │                  │                                                       │
│  │            ████████████  ← Conexão SAI pela borda direita!              │
│  │            ████████████                                                  │
│  │                  │                                                       │
│  │                  │                                                       │
│  └──────────────────┘                                                       │
│    ↑                                                                        │
│    Ponta arredondada à esquerda                                             │
│                                                                             │
└─────────────────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────────────────┐
│ ÍNDICE 3: CIMA + DIREITA (canto)                                            │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                             │
│  ┌──────────────────┐                                                       │
│  │      ████████████│  ← Conexão sai por CIMA                               │
│  │      ████████████│                                                       │
│  │      ████████████│  ← E também sai pela DIREITA                         │
│  │      ████████████│                                                      │
│  │      ████        │                                                       │
│  │      ████        │                                                       │
│  │      ████        │  ← Curva em L (formato de C invertido)               │
│  └──────────────────┘                                                       │
│                                                                             │
│  IMPORTANTE: A curva deve ter espessura CONSISTENTE!                        │
│                                                                             │
└─────────────────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────────────────┐
│ ÍNDICE 5: CIMA + BAIXO (conexão vertical)                                   │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                             │
│  ┌──────────────────┐                                                       │
│  │      ████        │  ← Conexão sai por CIMA                               │
│  │      ████        │                                                       │
│  │      ████        │                                                       │
│  │      ████        │                                                       │
│  │      ████        │                                                       │
│  │      ████        │                                                       │
│  │      ████        │  ← Conexão sai por BAIXO                              │
│  └──────────────────┘                                                       │
│                                                                             │
│  É uma linha vertical CENTRALIZADA                                          │
│                                                                             │
└─────────────────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────────────────┐
│ ÍNDICE 10: ESQUERDA + DIREITA (conexão horizontal)                          │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                             │
│  ┌──────────────────┐                                                       │
│  │                  │                                                       │
│  │                  │                                                       │
│  ████████████████████  ← Linha atravessa TODO o sprite                     │
│  ████████████████████                                                     │
│  │                  │                                                       │
│  │                  │                                                       │
│  └──────────────────┘                                                       │
│                                                                             │
│  A linha deve sair PELA BORDA de ambos os lados!                            │
│                                                                             │
└─────────────────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────────────────┐
│ ÍNDICE 15: TODOS OS LADOS (cruz)                                            │
├─────────────────────────────────────────────────────────────────────────────┤
│                                                                             │
│  ┌──────────────────┐                                                       │
│  │      ████        │  ← Cima                                               │
│  │      ████        │                                                       │
│  │      ████        │                                                       │
│  ████████████████████  ← Esquerda + Direita                                │
│  ████████████████████                                                     │
│  │      ████        │                                                       │
│  │      ████        │  ← Baixo                                              │
│  └──────────────────┘                                                       │
│                                                                             │
│  Uma cruz com espessura consistente em todas as direções                    │
│                                                                             │
└─────────────────────────────────────────────────────────────────────────────┘


╔═════════════════════════════════════════════════════════════════════════════╗
║  TABELA COMPLETA DOS 16 SPRITES                                             ║
╚═════════════════════════════════════════════════════════════════════════════╝

┌────────┬─────────────┬─────────────────────────────────────────────────────┐
│ ÍNDICE │ VIZINHOS    │ COMO DESENHAR                                       │
├────────┼─────────────┼─────────────────────────────────────────────────────┤
│   0    │ nenhum      │ Pequeno quadrado/nó central (ou caixa vazia)        │
│   1    │ cima        │ Linha vertical saindo por cima, ponta arredondada   │
│   2    │ direita     │ Linha horizontal saindo pela direita               │
│   3    │ cima+dir    │ Curva em L (C invertido)                           │
│   4    │ baixo       │ Linha vertical saindo por baixo                    │
│   5    │ cima+baixo  │ Linha vertical atravessando                        │
│   6    │ dir+baixo   │ Curva em L (canto superior esquerdo)               │
│   7    │ cima+dir+bx │ T apontando para esquerda                          │
│   8    │ esquerda    │ Linha horizontal saindo pela esquerda              │
│   9    │ cima+esq    │ Curva em L (canto inferior direito)                │
│  10    │ dir+esq     │ Linha horizontal atravessando                      │
│  11    │ cima+dir+esq│ T apontando para baixo                             │
│  12    │ baixo+esq   │ Curva em L (canto superior direito)                │
│  13    │ cima+bx+esq │ T apontando para direita                           │
│  14    │ dir+bx+esq  │ T apontando para cima                              │
│  15    │ todos       │ Cruz (todas as direções)                           │
└────────┴─────────────┴─────────────────────────────────────────────────────┘


╔═════════════════════════════════════════════════════════════════════════════╗
║  DICAS IMPORTANTES PARA EVITAR 'PONTAS ABERTAS'                             ║
╚═════════════════════════════════════════════════════════════════════════════╝

1. PIXEL PERFEITO
   • Use um tamanho de sprite que seja múltiplo de 2 (16, 32, 64)
   • Se a rama tem 8px de espessura, ela DEVE estar exatamente no centro
   • Para sprite 32x32 com rama 8px: a rama vai de x=12 até x=20

2. CONEXÕES DEVEM SAIR PELA BORDA
   ❌ ERRADO:           ✓ CORRETO:
   ┌────────┐          ┌────────┐
   │    ████│          │    ████│████ ← continua FORA do sprite
   │    ████│          │    ████│████
   │    ████│          │    ████│████
   │      │ │          │        │
   └────────┘          └────────┘

3. COORDENADAS DAS CONEXÕES (para sprite 32x32, rama 8px):
   • Centro X: 12 a 20 (8 pixels de largura)
   • Centro Y: 12 a 20 (8 pixels de altura)
   • Conexão superior: Y=0 até Y=20, X=12 a 20
   • Conexão inferior: Y=12 até Y=32, X=12 a 20
   • Conexão esquerda: X=0 até X=20, Y=12 a 20
   • Conexão direita: X=12 até X=32, Y=12 a 20

4. TESTE SEUS SPRITES:
   • Coloque dois sprites lado a lado no editor de imagens
   • As conexões DEVEM se encontrar perfeitamente
   • Não deve haver gaps ou sobreposição

═══════════════════════════════════════════════════════════════════════════════
");
    }

    [ContextMenu("Gerar Grade de Referência")]
    public void GenerateReferenceGrid()
    {
        // Cria uma textura com a grade de referência
        Texture2D grid = new Texture2D(spriteSize * 4, spriteSize * 4);

        for (int i = 0; i < 16; i++)
        {
            int x = (i % 4) * spriteSize;
            int y = (3 - (i / 4)) * spriteSize;

            // Preenche com cor de fundo
            for (int px = 0; px < spriteSize; px++)
            {
                for (int py = 0; py < spriteSize; py++)
                {
                    grid.SetPixel(x + px, y + py, new Color(0.2f, 0.2f, 0.2f));
                }
            }

            // Desenha guia de centro
            int centerStart = (spriteSize - branchThickness) / 2;
            int centerEnd = centerStart + branchThickness;

            // Desenha área central (onde a rama deve estar)
            for (int px = centerStart; px < centerEnd; px++)
            {
                for (int py = centerStart; py < centerEnd; py++)
                {
                    grid.SetPixel(x + px, y + py, new Color(0.4f, 0.4f, 0.4f));
                }
            }

            // Desenha conexões baseado no índice
            Color connectionColor = new Color(0.3f, 0.6f, 0.9f);

            // Cima (bit 0)
            if ((i & 1) != 0)
            {
                for (int px = centerStart; px < centerEnd; px++)
                {
                    for (int py = 0; py < centerEnd; py++)
                    {
                        grid.SetPixel(x + px, y + py, connectionColor);
                    }
                }
            }

            // Direita (bit 1)
            if ((i & 2) != 0)
            {
                for (int px = centerStart; px < spriteSize; px++)
                {
                    for (int py = centerStart; py < centerEnd; py++)
                    {
                        grid.SetPixel(x + px, y + py, connectionColor);
                    }
                }
            }

            // Baixo (bit 2)
            if ((i & 4) != 0)
            {
                for (int px = centerStart; px < centerEnd; px++)
                {
                    for (int py = centerStart; py < spriteSize; py++)
                    {
                        grid.SetPixel(x + px, y + py, connectionColor);
                    }
                }
            }

            // Esquerda (bit 3)
            if ((i & 8) != 0)
            {
                for (int px = 0; px < centerEnd; px++)
                {
                    for (int py = centerStart; py < centerEnd; py++)
                    {
                        grid.SetPixel(x + px, y + py, connectionColor);
                    }
                }
            }

            // Escreve o número do índice
            string num = i.ToString();
            for (int c = 0; c < num.Length; c++)
            {
                DrawDigit(grid, x + 2 + c * 6, y + spriteSize - 10, num[c]);
            }
        }

        grid.Apply();

        // Salva a textura
        string path = "Assets/Sprites/SpriteReferenceGrid.png";
        System.IO.Directory.CreateDirectory("Assets/Sprites");

        byte[] bytes = grid.EncodeToPNG();
        System.IO.File.WriteAllBytes(path, bytes);

        AssetDatabase.Refresh();

        Debug.Log($"✅ Grade de referência gerada em: {path}");
        Debug.Log("Use como guia para criar seus sprites no programa de sua preferência!");
    }

    private void DrawDigit(Texture2D tex, int x, int y, char digit)
    {
        // Simplificado - apenas marca o local
        Color white = Color.white;
        int[,] pattern = GetDigitPattern(digit);

        for (int py = 0; py < 7; py++)
        {
            for (int px = 0; px < 5; px++)
            {
                if (pattern[py, px] == 1)
                {
                    if (x + px < tex.width && y + py < tex.height)
                    {
                        tex.SetPixel(x + px, y + py, white);
                    }
                }
            }
        }
    }

    private int[,] GetDigitPattern(char digit)
    {
        // Padrões de dígitos 5x7
        return digit switch
        {
            '0' => new int[,] {
                {0,1,1,1,0},
                {1,0,0,0,1},
                {1,0,0,0,1},
                {1,0,0,0,1},
                {1,0,0,0,1},
                {1,0,0,0,1},
                {0,1,1,1,0}
            },
            '1' => new int[,] {
                {0,0,1,0,0},
                {0,1,1,0,0},
                {0,0,1,0,0},
                {0,0,1,0,0},
                {0,0,1,0,0},
                {0,0,1,0,0},
                {0,1,1,1,0}
            },
            _ => new int[,] {
                {1,1,1,1,1},
                {1,0,0,0,1},
                {1,0,0,0,1},
                {1,0,0,0,1},
                {1,0,0,0,1},
                {1,0,0,0,1},
                {1,1,1,1,1}
            }
        };
    }

    [ContextMenu("Criar Sprites de Exemplo (Rama)")]
    public void CreateExampleBranchSprites()
    {
        System.IO.Directory.CreateDirectory("Assets/Sprites/BranchExample");

        for (int i = 0; i < 16; i++)
        {
            Texture2D sprite = CreateBranchSprite(i);
            byte[] bytes = sprite.EncodeToPNG();
            string path = $"Assets/Sprites/BranchExample/BranchSprite_{i:D2}.png";
            System.IO.File.WriteAllBytes(path, bytes);
        }

        AssetDatabase.Refresh();
        Debug.Log("✅ 16 sprites de exemplo criados em Assets/Sprites/BranchExample/");
        Debug.Log("⚠️ Estes são sprites BÁSICOS de referência. Melhore-os no seu editor de imagens!");
    }

    private Texture2D CreateBranchSprite(int index)
    {
        Texture2D tex = new Texture2D(spriteSize, spriteSize);
        Color background = new Color(0, 0, 0, 0); // Transparente
        Color branch = new Color(0.6f, 0.4f, 0.2f); // Marrom (cor de galho)

        // Preenche com transparente
        for (int x = 0; x < spriteSize; x++)
        {
            for (int y = 0; y < spriteSize; y++)
            {
                tex.SetPixel(x, y, background);
            }
        }

        int centerStart = (spriteSize - branchThickness) / 2;
        int centerEnd = centerStart + branchThickness;

        // Desenha conexões baseado no bitmask
        // Cima (bit 0)
        if ((index & 1) != 0)
        {
            DrawBranchSegment(tex, centerStart, 0, centerEnd, centerEnd, branch);
        }

        // Direita (bit 1)
        if ((index & 2) != 0)
        {
            DrawBranchSegment(tex, centerStart, centerStart, spriteSize, centerEnd, branch);
        }

        // Baixo (bit 2)
        if ((index & 4) != 0)
        {
            DrawBranchSegment(tex, centerStart, centerStart, centerEnd, spriteSize, branch);
        }

        // Esquerda (bit 3)
        if ((index & 8) != 0)
        {
            DrawBranchSegment(tex, 0, centerStart, centerEnd, centerEnd, branch);
        }

        // Centro (sempre preenche se houver qualquer conexão)
        if (index > 0)
        {
            DrawBranchSegment(tex, centerStart, centerStart, centerEnd, centerEnd, branch);
        }
        else
        {
            // Bloco isolado - desenha um pequeno quadrado central
            int nodeSize = branchThickness;
            int nodeStart = (spriteSize - nodeSize) / 2;
            DrawBranchSegment(tex, nodeStart, nodeStart, nodeStart + nodeSize, nodeStart + nodeSize, branch);
        }

        tex.Apply();
        return tex;
    }

    private void DrawBranchSegment(Texture2D tex, int x1, int y1, int x2, int y2, Color color)
    {
        for (int x = x1; x < x2; x++)
        {
            for (int y = y1; y < y2; y++)
            {
                if (x >= 0 && x < spriteSize && y >= 0 && y < spriteSize)
                {
                    tex.SetPixel(x, y, color);
                }
            }
        }
    }

    #endif
}
