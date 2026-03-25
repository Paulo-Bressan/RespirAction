using UnityEngine;
using System.Collections;
using System.Collections.Generic; // Necessário para usar List<T>

/// <summary>
/// Controla a movimentação do jogador, incluindo andar, pular e a mecânica de inversão de gravidade.
/// Gerencia também o sistema de respawn, checkpoints e o estado de interação.
/// </summary>
public class PlayerMovementF2 : MonoBehaviour
{
    // =================================================================
    // VARIÁVEIS DE CONFIGURAÇÃO DE FÍSICA E MOVIMENTO
    // =================================================================
    private float horizontalInput;
    //public event Action levelFinish;
    private bool jumpRequest;

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


    // =================================================================
    // REFERÊNCIAS DE COMPONENTES
    // =================================================================
    [Header("Referencias de componentes")]
    [SerializeField] private AudioManagerScene audioManagerScene = null;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

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
        
        // Respawn e Sprite
        respawnPoint = transform.position;
        if (spriteRenderer != null) defaultSprite = spriteRenderer.sprite;

        // Braço Mecânico
        if (armObject != null)
        {
            armObject.SetActive(false); 
        }

        // Checagem de receferncia de audiomanager
        if (!audioManagerScene)
            Debug.Log("[PLAYER] Audiomanager faltando!");
    }

    void Update()
    {
        // --- 1. LEITURA DE INPUT ---
        if (!isMovementLocked)
        {
            moveInput = Input.GetAxisRaw("Horizontal");

            // DEBUG 1: Verifica se o teclado está funcionando e se o jogo não está travado
            if (moveInput != 0) 
            {
                Debug.Log($"[INPUT] Tecla detectada! Valor: {moveInput}");
            }

            // Captura o pulo
            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                jumpRequest = true;
                Debug.Log("[INPUT] Botão de Pulo pressionado!");
                audioManagerScene.PlaySound(4);
            }
        }
        else
        {
            moveInput = 0f;
            jumpRequest = false;
            // DEBUG 2: Avisa se o movimento estiver bloqueado por interação
            //Debug.LogWarning("[STATUS] O movimento está BLOQUEADO (isMovementLocked = true)");
        }

        // --- 2. CONTROLE DE DIREÇÃO DO SPRITE (FLIP) ---
        if (moveInput != 0)
        {
            if (moveInput > 0)
            {
                // Movendo para DIREITA (World)
                // Se normal: flipX false. Se invertido: flipX true (para desinverter a rotação).
                spriteRenderer.flipX = false;
            }
            else if (moveInput < 0)
            {
                // Movendo para ESQUERDA (World)
                // Se normal: flipX true. Se invertido: flipX false.
                spriteRenderer.flipX = true;
            }
        }
        
        // --- 3. ATUALIZAÇÃO DE ANIMAÇÃO ---
        if (animator != null && animator.enabled)
        {
            animator.SetBool("isRunning", moveInput != 0);

            // TRUQUE: Calculamos a velocidade local relativa
            // Se estiver de cabeça para baixo (gravidade invertida), invertemos o sinal da velocidade
            float relativeYVelocity = rb.linearVelocity.y;

            animator.SetFloat("yVelocity", relativeYVelocity); 
        }
        
    }

    void FixedUpdate()
    {
        
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, whatIsGround);

        // Atualiza o Animator para ele saber se está voando ou no chão
        if (animator != null && animator.enabled)
        {
            animator.SetBool("isGrounded", isGrounded);
        }

        // =================================================================
        // 2. APLICAÇÃO DE MOVIMENTO FÍSICO
        // =================================================================

        // Aplica o movimento horizontal
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        // Aplica o pulo (se solicitado no Update)
        if (jumpRequest)
        {
            float jumpVel = jumpForce;
            
            // Mantém o X, muda o Y
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpVel);
            
            Debug.Log($"[FÍSICA] Pulo executado! Força: {jumpVel}");
            
            jumpRequest = false; // Consome o input
        }
    
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

        transform.rotation = Quaternion.identity;
        
        
    }

    // =================================================================
    // GESTÃO DE INTERAÇÃO E CÂMERA
    // =================================================================

    /// <summary>
    /// Controla o estado de interação do jogador (troca de sprite/animação e braço).
    /// </summary>
    public void SetInteractingState(bool isInteracting)
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
            
            // 3. Ativa o Braço
            if (armObject != null)
            {
                armObject.SetActive(true);
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
            
            // 3. Desativa o Braço
            if (armObject != null)
            {
                armObject.SetActive(false);
            }
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