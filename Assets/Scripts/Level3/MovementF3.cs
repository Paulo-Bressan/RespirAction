using UnityEngine;
using System.Collections;
using System.Collections.Generic; // Necessário para usar List<T>

/// <summary>
/// Controla a movimentação do jogador, incluindo andar, pular e a mecânica de inversão de gravidade.
/// Gerencia também o sistema de respawn, checkpoints e o estado de interação.
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    // =================================================================
    // VARIÁVEIS DE CONFIGURAÇÃO DE FÍSICA E MOVIMENTO
    // =================================================================

    private List<InteractiveTile> allInteractiveTiles = new List<InteractiveTile>(); 
    
    private InteractiveTile currentTargetTile = null;

    [Header("Movimento")]
    [Tooltip("Velocidade horizontal do personagem.")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Pulo & Detecção de Chão")]
    [Tooltip("Força aplicada ao pular.")]
    [SerializeField] private float jumpForce = 10f;
    
    [Tooltip("Objeto vazio (filho do player) posicionado nos pés para detectar o chão.")]
    [SerializeField] private Transform groundCheck;
    
    [Tooltip("Define quais layers são consideradas 'chão' (ex: Ground, Plataformas).")]
    [SerializeField] private LayerMask whatIsGround;
    
    [Tooltip("Raio do círculo de colisão para verificar o chão.")]
    [SerializeField] private float groundRadius = 0.2f;
    
    private bool isGrounded;

    [Header("Mecânica de Gravidade")]
    [Tooltip("Tempo em segundos até a gravidade inverter automaticamente.")]
    [SerializeField] private float timeUntilFlip = 10f;
    
    private float gravityFlipTimer;
    private bool isUpsideDown = false;
    private float defaultGravityScale;

    // =================================================================
    // VARIÁVEIS DE RESPawn / CHECKPOINT / INTERAÇÃO
    // =================================================================
    [Header("Respawn & Interação")]
    [Tooltip("Sprite estático do player a ser usado durante a interação (Atribua no Inspector).")]
    [SerializeField] private Sprite interactionSprite; 
    
    [Tooltip("Referência ao objeto do braço que deve rotacionar (Atribua no Inspector).")]
    [SerializeField] private GameObject armObject;

    [Header("Controle de Câmera")]
    [Tooltip("Objeto vazio que a câmera deve seguir para pre-visualizar o checkpoint.")]
    [SerializeField] private Transform cameraTargetOverride;

    [Tooltip("Tempo em segundos que a câmera deve permanecer no novo checkpoint.")]
    [SerializeField] private float cameraFocusTime = 2.0f;
    
    private Vector3 respawnPoint; 
    private Sprite defaultSprite;
    
    // Lista estática para todos os tiles interativos na cena
    
    
    // =================================================================
    // REFERÊNCIAS DE COMPONENTES
    // =================================================================
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private ArmRotator armRotator;

    // =================================================================
    // VARIÁVEIS DE ESTADO
    // =================================================================
    private float moveInput;    
    private bool isMovementLocked = false;

    void Start()
    {
        // --- 1. Inicialização de Componentes ---
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>(); 
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Timers e Gravidade
        gravityFlipTimer = timeUntilFlip;
        defaultGravityScale = rb.gravityScale;
        
        // Respawn e Sprite
        respawnPoint = transform.position;
        if (spriteRenderer != null) defaultSprite = spriteRenderer.sprite;

        // Braço Mecânico
        if (armObject != null)
        {
            armRotator = armObject.GetComponent<ArmRotator>();
            armObject.SetActive(false); 
        }

        // --- 2. Configuração dos Tiles Interativos ---

        // Converte o array encontrado para uma Lista manipulável
        allInteractiveTiles = new List<InteractiveTile>(FindObjectsByType<InteractiveTile>(FindObjectsSortMode.None));

        // [IMPORTANTE] Força TODOS os tiles a ficarem TRAVADOS (não interativos) inicialmente.
        // Isso resolve o problema de tiles ficarem ativos indevidamente.
        foreach (var tile in allInteractiveTiles)
        {
            tile.SetAsTarget(false);
        }

        // Log de conferência
        Debug.Log($"[Start] Tiles encontrados e resetados: {allInteractiveTiles.Count}");

        if (allInteractiveTiles.Count > 0)
        {
            InteractiveTile startingTile = null;

            // Busca especificamente pelo tile chamado "tileHurt (1)"
            foreach (var tile in allInteractiveTiles)
            {
                // .Trim() remove espaços invisíveis que podem causar erro na busca
                if (tile.name.Trim().Equals("tileHurt (1)")) 
                {
                    startingTile = tile;
                    break; 
                }
            }

            // Define quem será o alvo ativo
            if (startingTile != null)
            {
                currentTargetTile = startingTile;
                Debug.Log("🎯 Alvo Inicial Definido: " + currentTargetTile.name);
            }
            else
            {
                // Fallback: Se não achar o nome exato, pega o primeiro da lista
                Debug.LogWarning("⚠️ 'tileHurt (1)' não encontrado. Usando o primeiro da lista.");
                currentTargetTile = allInteractiveTiles[0];
            }

            // [IMPORTANTE] Só agora ativamos o tile escolhido
            currentTargetTile.SetAsTarget(true);
        }
        else
        {
            Debug.LogError("⛔ ERRO: Nenhum InteractiveTile encontrado na cena!");
        }
    }

    void Update()
    {
        // --- 1. LEITURA DE INPUT ---
        if (!isMovementLocked)
        {
            moveInput = Input.GetAxisRaw("Horizontal");
        }
        else
        {
            moveInput = 0f; 
        }

        // --- 2. CONTROLE DE DIREÇÃO DO SPRITE (FLIP) ---
        if (moveInput != 0)
        {
            // Vira para a esquerda se o input for negativo (assumindo que o sprite padrão olha para a direita)
            if (moveInput < 0) spriteRenderer.flipX = true; 
            else if (moveInput > 0) spriteRenderer.flipX = false;
        }
        
        // --- 3. ATUALIZAÇÃO DE ANIMAÇÃO ---
        if (animator != null && animator.enabled)
        {
            animator.SetBool("isRunning", moveInput != 0);
            animator.SetFloat("yVelocity", rb.linearVelocity.y); 
        }

        // --- 4. LÓGICA DO TIMER DE GRAVIDADE ---
        gravityFlipTimer -= Time.deltaTime;
        if (gravityFlipTimer <= 0f)
        {
            FlipGravity();
            gravityFlipTimer = timeUntilFlip; 
        }

        // --- 5. LÓGICA DE PULO ---
        if (Input.GetButtonDown("Jump") && isGrounded && !isMovementLocked)
        {
            float jumpVelocity = isUpsideDown ? -jumpForce : jumpForce;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpVelocity); 
        }
    }

    void FixedUpdate()
    {
        // --- 1. VERIFICAÇÃO DO CHÃO (FÍSICA) ---
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, whatIsGround);
        
        if (animator != null && animator.enabled)
        {
            animator.SetBool("isGrounded", isGrounded);
        }
        
        // --- 2. MOVIMENTAÇÃO ---
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y); 
    }

    /// <summary>
    /// Inverte a gravidade do personagem e rotaciona o sprite visualmente.
    /// </summary>
    void FlipGravity()
    {
        isUpsideDown = !isUpsideDown;
        rb.gravityScale *= -1;
        transform.Rotate(0, 0, 180f);
    }
    
    // =================================================================
    // MÉTODOS DE CHECKPOINT E RESPAWN
    // =================================================================

    /// <summary>
    /// Atualiza o ponto de respawn para a posição fornecida.
    /// </summary>
    public void UpdateCheckpoint(Vector3 newPosition)
    {
        respawnPoint = newPosition;
    }
    
    /// <summary>
    /// Reseta o jogador para o último ponto seguro e restaura a gravidade ao normal.
    /// </summary>
    public void Respawn()
    {
        transform.position = respawnPoint;
        rb.linearVelocity = Vector2.zero; 

        if (isUpsideDown)
        {
            FlipGravity(); 
        }

        transform.rotation = Quaternion.identity;
        rb.gravityScale = defaultGravityScale;
        isUpsideDown = false;
        
        gravityFlipTimer = timeUntilFlip; 
    }

    // =================================================================
    // GESTÃO DE INTERAÇÃO E CÂMERA
    // =================================================================

    /// <summary>
    /// Controla o estado de interação do jogador (troca de sprite/animação e braço).
    /// </summary>
    public void SetInteractingState(bool isInteracting, Transform targetTile)
    {
        if (isInteracting)
        {
            // 1. BLOQUEIA O MOVIMENTO
            isMovementLocked = true;
            rb.linearVelocity = Vector2.zero;
            
            // 2. Desativa o Animator e troca para o sprite estático
            if (animator != null) animator.enabled = false; 
            if (spriteRenderer != null && interactionSprite != null)
                spriteRenderer.sprite = interactionSprite;
            
            // 3. Ativa o Braço e seta o alvo
            if (armObject != null && armRotator != null)
            {
                armObject.SetActive(true);
                armRotator.SetTarget(targetTile); // O braço trava a rotação para o tile
            }
        }
        else
        {
            // 1. DESBLOQUEIA O MOVIMENTO
            isMovementLocked = false;
            
            // 2. Ativa o Animator e volta para o sprite padrão
            if (animator != null) animator.enabled = true; 
            if (spriteRenderer != null)
                spriteRenderer.sprite = defaultSprite; 
            
            // 3. Desativa o Braço e remove o alvo
            if (armObject != null && armRotator != null)
            {
                armObject.SetActive(false);
                armRotator.SetTarget(null);
            }
        }
    }

    /// <summary>
    /// Encontra um Tile Interativo aleatório (que não seja o destruído) e move a câmera para pre-visualizá-lo.
    /// </summary>
    public void TeleportToRandomCheckpoint(GameObject destroyedTileObj)
    {
        // Verifica se a lista existe
        if (allInteractiveTiles == null || allInteractiveTiles.Count == 0)
        {
            Debug.Log("🎉 Todos os tiles já foram finalizados.");
            return;
        }

        // =================================================================
        // LÓGICA DE REMOÇÃO (Substitui a função RemoveDestroyedTile)
        // =================================================================
        // Como 'allInteractiveTiles' agora é uma List<>, podemos remover direto.
        // O RemoveAll procura na lista quem tem o mesmo GameObject e remove.
        allInteractiveTiles.RemoveAll(tile => tile.gameObject == destroyedTileObj);

        // =================================================================
        // LÓGICA DE SORTEIO DO PRÓXIMO
        // =================================================================
        if (allInteractiveTiles.Count > 0)
        {
            // Sorteia um índice com base no tamanho atual da lista
            int randomIndex = Random.Range(0, allInteractiveTiles.Count);
            InteractiveTile nextTileTarget = allInteractiveTiles[randomIndex];

            if (nextTileTarget != null)
            {
                currentTargetTile = nextTileTarget;
                currentTargetTile.SetAsTarget(true);
                
                Debug.Log("🎲 Novo Alvo Sorteado: " + currentTargetTile.name);

                // Foca a câmera
                //StartCoroutine(FocusCameraOnCheckpoint(currentTargetTile.transform.position));
            }
        }
        else
        {
            Debug.Log("🏆 Fase Concluída! Lista vazia.");
        }
    }


    

    /// <summary>
    /// Coroutine para focar a câmera no novo checkpoint e retornar.
    /// </summary>
    private IEnumerator FocusCameraOnCheckpoint(Vector3 targetPosition)
    {
        // 1. Mover o alvo de override para a nova posição
        cameraTargetOverride.position = targetPosition;
        
        // TODO: Aqui você deve adicionar a lógica do seu script de Câmera para seguir o cameraTargetOverride
        // Ex: CameraFollowScript.Instance.SetTarget(cameraTargetOverride); 
        
        yield return new WaitForSeconds(cameraFocusTime);

        // TODO: Retornar o alvo da câmera para o Jogador (transform)
        // Ex: CameraFollowScript.Instance.SetTarget(transform); 

        // Opcional: Para evitar NullReference se o player foi movido/destruído
        if(this != null) Debug.Log("Câmera retornando ao Jogador.");
    }
}