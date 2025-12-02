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

    private static InteractiveTile[] allInteractiveTiles; 
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
        // Obtém as referências dos componentes
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>(); 
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Inicializa os timers e estados
        gravityFlipTimer = timeUntilFlip;
        defaultGravityScale = rb.gravityScale;
        
        // Define o ponto de respawn inicial e salva o sprite padrão
        respawnPoint = transform.position;
        if (spriteRenderer != null)
        {
            defaultSprite = spriteRenderer.sprite;
        }

        // Obtém a referência ao braço rotativo e garante que ele comece desligado
        if (armObject != null)
        {
            armRotator = armObject.GetComponent<ArmRotator>();
            armObject.SetActive(false); 
        }

        // Encontra todos os tiles interativos na cena
        allInteractiveTiles = FindObjectsOfType<InteractiveTile>();
        if (allInteractiveTiles.Length > 0)
        {
            // Escolhe um índice aleatório dentro do tamanho do array
            int randomIndex = Random.Range(0, allInteractiveTiles.Length); 
            
            currentTargetTile = allInteractiveTiles[randomIndex];
            currentTargetTile.SetAsTarget(true);
            
            Debug.Log("Tile Inicial escolhido aleatoriamente: " + currentTargetTile.name);
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
    public void TeleportToRandomCheckpoint(GameObject destroyedTile)
{
    if (allInteractiveTiles == null || allInteractiveTiles.Length == 0 || cameraTargetOverride == null)
    {
        Debug.LogWarning("Configuração de foco incompleta ou Tiles Interativos insuficientes.");
        return; 
    }
    
    // 1. Desabilita o tile que estava sendo interagido (se ainda não foi desabilitado por outra lógica)
    if (currentTargetTile != null)
    {
        currentTargetTile.SetAsTarget(false);
    }

    // 2. Remove o tile destruído da lista de tiles disponíveis
    RemoveDestroyedTile(destroyedTile);
    
    InteractiveTile nextTileTarget = null;
    int maxAttempts = 10;
    int attempts = 0;
    
    // Se não houver mais tiles
    if (allInteractiveTiles.Length == 0)
    {
        Debug.Log("Todos os tiles interativos foram destruídos!");
        return;
    }

    // Escolhe um novo alvo aleatório dos que restaram
    while (nextTileTarget == null && attempts < maxAttempts)
    {
        int randomIndex = Random.Range(0, allInteractiveTiles.Length);
        nextTileTarget = allInteractiveTiles[randomIndex];
        attempts++;
    }
    
    if (nextTileTarget != null)
    {
        // 3. Define o novo tile como o ÚNICO alvo quebrável
        nextTileTarget.SetAsTarget(true);
        currentTargetTile = nextTileTarget; // Armazena para ser desativado na próxima vez
        Debug.Log("--- PROGRESSO DO JOGO --- Novo Tile Alvo Liberado: " + nextTileTarget.name);
        Debug.Log("Novo Tile Alvo Definido: " + nextTileTarget.name);

        // 4. Inicia a rotina para focar a câmera no novo tile
        StartCoroutine(FocusCameraOnCheckpoint(nextTileTarget.transform.position));
    }
    else
    {
        Debug.LogError("Não foi possível selecionar um novo tile alvo válido.");
    }
}

    /// <summary>
    /// Remove o tile destruído da lista de tiles interativos disponíveis.
    /// </summary>
    private void RemoveDestroyedTile(GameObject tileToRemove)
    {
        // Converte o array estático para uma Lista temporária
        var tileList = new List<InteractiveTile>(allInteractiveTiles);
        
        // Encontrar e remover o tile com base na referência do GameObject
        tileList.RemoveAll(tile => tile.gameObject == tileToRemove);
        
        // Reverter para o array estático
        allInteractiveTiles = tileList.ToArray();
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