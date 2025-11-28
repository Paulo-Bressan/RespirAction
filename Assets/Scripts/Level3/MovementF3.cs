using UnityEngine;

/// <summary>
/// Controla a movimentação do jogador, incluindo andar, pular e a mecânica de inversão de gravidade.
/// Gerencia também o sistema de respawn e checkpoints.
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    // =================================================================
    // VARIÁVEIS DE CONFIGURAÇÃO
    // =================================================================

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
    
    // Armazena se o player está tocando o chão no frame atual.
    private bool isGrounded;

    [Header("Mecânica de Gravidade")]
    [Tooltip("Tempo em segundos até a gravidade inverter automaticamente.")]
    [SerializeField] private float timeUntilFlip = 10f;
    
    // Contador regressivo para a inversão.
    private float gravityFlipTimer;
    
    // Estado atual da gravidade (false = normal, true = invertida/teto).
    private bool isUpsideDown = false;
    
    // Armazena a escala original da gravidade para resets.
    private float defaultGravityScale;

    // =================================================================
    // REFERÊNCIAS DE COMPONENTES
    // =================================================================
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    // =================================================================
    // VARIÁVEIS DE ESTADO
    // =================================================================
    private float moveInput;     // Armazena a direção horizontal (-1 a 1)
    private Vector3 respawnPoint; // Último local seguro salvo

    void Start()
    {
        // Obtém as referências dos componentes no objeto
        rb = GetComponent<Rigidbody2D>();
        //animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Inicializa os timers e estados
        gravityFlipTimer = timeUntilFlip;
        defaultGravityScale = rb.gravityScale;
        
        // Define o ponto de respawn inicial como a posição de começo do jogo
        respawnPoint = transform.position;
    }

    void Update()
    {
        // LEITURA DE INPUT
        moveInput = Input.GetAxisRaw("Horizontal") * -1f;

        // ATUALIZAÇÃO DE ANIMAÇÃO
        // Define se está correndo se houver input diferente de zero
        //animator.SetBool("isRunning", moveInput != 0);
        // Passa a velocidade vertical para animações de pulo/queda
        //animator.SetFloat("yVelocity", rb.linearVelocity.y); // Nota: linearVelocity é usado no Unity 6+ (antigo .velocity)

        // CONTROLE DE DIREÇÃO DO SPRITE (FLIP)
        // A lógica é separada para manter o personagem olhando corretamente mesmo de cabeça para baixo
        if (isUpsideDown)
        {
            // Quando invertido, a lógica de virar o sprite também se inverte
            if (moveInput > 0) spriteRenderer.flipX = true;
            else if (moveInput < 0) spriteRenderer.flipX = false;
        }
        else
        {
            // Comportamento padrão
            if (moveInput > 0) spriteRenderer.flipX = false;
            else if (moveInput < 0) spriteRenderer.flipX = true;
        }

        // LÓGICA DO TIMER DE GRAVIDADE
        gravityFlipTimer -= Time.deltaTime;
        if (gravityFlipTimer <= 0f)
        {
            FlipGravity();
            gravityFlipTimer = timeUntilFlip; // Reinicia o timer
        }

        // LÓGICA DE PULO
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            // Se a gravidade estiver invertida, a força do pulo deve ser negativa (para baixo/teto)
            float jumpVelocity = isUpsideDown ? -jumpForce : jumpForce;
            
            // Aplica a velocidade mantendo o movimento horizontal atual
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpVelocity);
        }
    }

    void FixedUpdate()
    {
        // VERIFICAÇÃO DO CHÃO (FÍSICA)
        // Cria um pequeno círculo na posição dos pés para ver se colide com a layer do chão
        
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, whatIsGround);
        
        // Atualiza a animação baseada no estado do chão
        //animator.SetBool("isGrounded", isGrounded);

        // MOVIMENTAÇÃO
        // Aplica a velocidade horizontal direta, mantendo a velocidade vertical (gravidade/pulo)
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
    }

    /// <summary>
    /// Inverte a gravidade do personagem e rotaciona o sprite visualmente.
    /// </summary>
    void FlipGravity()
    {
        isUpsideDown = !isUpsideDown;
        
        // Inverte a escala de gravidade do Rigidbody (efeito físico)
        rb.gravityScale *= -1;
        
        // Gira o personagem 180 graus para ficar visualmente de cabeça para baixo
        transform.Rotate(0, 0, 180f);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Sistema de morte instantânea (ex: espinhos)
        if (other.CompareTag("death"))
        {
            Respawn();
        }
        // Sistema de Checkpoint
        else if (other.CompareTag("checkpoint"))
        {
            respawnPoint = other.transform.position;
            // Desativa o checkpoint visualmente para indicar que já foi pego (opcional)
            other.gameObject.SetActive(false);
        }
    }
    
    /// <summary>
    /// Reseta o jogador para o último ponto seguro e restaura a gravidade ao normal.
    /// </summary>
    public void Respawn()
    {
        transform.position = respawnPoint;
        rb.linearVelocity = Vector2.zero; // Zera a inércia para evitar que o player continue voando ao renascer

        // Se o jogador morreu enquanto estava com a gravidade invertida,
        // precisamos resetar para o estado normal para evitar bugs ao renascer.
        if (isUpsideDown)
        {
            FlipGravity(); 
        }
        
        // Garante que a rotação e gravidade estão nos valores padrão
        transform.rotation = Quaternion.identity;
        rb.gravityScale = defaultGravityScale;
        
        isUpsideDown = false;
        gravityFlipTimer = timeUntilFlip; // Reseta o timer da mecânica
    }
}