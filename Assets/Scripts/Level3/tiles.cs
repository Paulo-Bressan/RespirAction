using UnityEngine;

/// <summary>
/// Objeto que o jogador interage ao clicar e segurar.
/// Só permite interação se a flag 'isTarget' for verdadeira (definida pelo PlayerMovement).
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class InteractiveTile : MonoBehaviour
{
    // =================================================================
    // VARIÁVEIS DE CONFIGURAÇÃO
    // =================================================================
    [Header("Configuração de Interação")]
    [Tooltip("Tempo em segundos que o jogador precisa clicar para o Tile desaparecer.")]
    [SerializeField] private float requiredHoldTime = 2.0f;
    
    [Header("Feedback Visual")]
    [Tooltip("A cor que o tile terá enquanto o jogador estiver segurando o clique.")]
    [SerializeField] private Color interactColor = Color.yellow;
    
    [Tooltip("Flag que indica se este tile é o alvo atual para destruição.")]
    public bool isTarget = false; // FLAG CONTROLADA PELO PlayerMovement
    
    // Cores internas
    private Color defaultColor;
    private Color targetColor = Color.cyan; // Cor para indicar que é o alvo
    private Color neutralColor; 

    // Variáveis de estado e controle
    private float currentHoldTime = 0f;
    private bool isPlayerInside = false;
    private bool isInteracting = false;
    
    // Referências
    private PlayerMovement player;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            // Salva a cor inicial para voltar a ela depois
            defaultColor = spriteRenderer.color;
            neutralColor = defaultColor;
        }
        
        // Define a cor inicial corretamente (se for alvo inicial, já deve estar ciano)
        UpdateVisualFeedback();
    }

    void Update()
    {
        // Atualiza o visual (cor, progresso)
        UpdateVisualFeedback();

        // Se o player não estiver na área de trigger, ignora o input de mouse
        if (!isPlayerInside) return;
        
        // --- INÍCIO DA INTERAÇÃO (Clique Down) ---
        // SÓ PERMITE INICIAR O CLIQUE se for o alvo (isTarget)
        if (isTarget && Input.GetMouseButtonDown(0))
        {
            isInteracting = true;
            currentHoldTime = 0f;
            
            if (player != null)
            {
                // Informa o player para bloquear o movimento e ativar o braço/sprite de interação
                player.SetInteractingState(true, transform);
            }
        }

        // --- SEGURANDO O BOTÃO (Clique Contínuo) ---
        if (isInteracting && Input.GetMouseButton(0))
        {
            currentHoldTime += Time.deltaTime;
            
            // Verifica se o tempo limite foi atingido
            if (currentHoldTime >= requiredHoldTime)
            {
                isInteracting = false;
                
                // 1. Informa o jogador para parar a animação e o braço
                if (player != null)
                {
                    player.SetInteractingState(false, null);
                    // 2. Chama a função de foco de câmera para o PRÓXIMO alvo
                    player.TeleportToRandomCheckpoint(this.gameObject);
                }
                
                // 3. O Tile interativo desaparece
                gameObject.SetActive(false); 
            }
        }

        // --- SOLTOU ANTES DO TEMPO (Clique Up) ---
        if (isInteracting && Input.GetMouseButtonUp(0))
        {
            isInteracting = false;
            
            // Reseta o estado do player
            if (player != null)
            {
                 player.SetInteractingState(false, null);
            }
        }
    }
    
    /// <summary>
    /// Atualiza a cor do sprite com base no estado (neutro, alvo, interagindo).
    /// </summary>
    private void UpdateVisualFeedback()
    {
        if (spriteRenderer == null) return;

        if (isInteracting)
        {
            // Feedback de Progresso (interpola de cor neutra para interactColor)
            float progress = currentHoldTime / requiredHoldTime;
            spriteRenderer.color = Color.Lerp(neutralColor, interactColor, progress);
        }
        else if (isTarget)
        {
            // Feedback de Alvo (Cor Cyan)
            spriteRenderer.color = targetColor;
        }
        else
        {
            // Estado neutro
            spriteRenderer.color = neutralColor;
        }
    }

    // =================================================================
    // GESTÃO DE TRIGGER (Colisão)
    // =================================================================
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        // A interação só é permitida se o objeto que entra tiver a Tag "Player"
        if (other.CompareTag("player"))
        {
            isPlayerInside = true;
            player = other.GetComponent<PlayerMovement>();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("player"))
        {
            isPlayerInside = false;
            
            // Se o player sair enquanto interagia, cancela tudo
            if (isInteracting)
            {
                 isInteracting = false;
                 if (player != null)
                 {
                    player.SetInteractingState(false, null);
                 }
            }
            player = null;
        }
    }
    
    // =================================================================
    // COMUNICAÇÃO EXTERNA
    // =================================================================
    
    /// <summary>
    /// Chamado pelo PlayerMovement para definir se este tile é o alvo atual para destruição.
    /// </summary>
    public void SetAsTarget(bool isCurrentTarget)
    {
        isTarget = isCurrentTarget;
        
        // A cor será atualizada no próximo Update() pelo UpdateVisualFeedback()
    }

    // =================================================================
    // MARCADORES TEMPORÁRIOS (GIZMOS - APENAS EDITOR)
    // =================================================================
    
    private void OnDrawGizmos()
    {
        // Se este tile é o alvo atual, desenhamos um Gizmo brilhante.
        if (isTarget)
        {
            Gizmos.color = Color.red; 
            Gizmos.DrawCube(transform.position, new Vector3(1f, 1f, 0.1f)); 

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position, new Vector3(1.1f, 1.1f, 0.1f));
        }
    }
}