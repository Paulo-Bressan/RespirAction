using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class PlayerMovementF4 : MonoBehaviour
{
    [Header("Movimento Básico (Sem Física)")]
    [SerializeField] private float moveSpeed = 8f;

    [Header("Limites do Mapa")]
    [Tooltip("Colisor estático que define a área onde o jogador DEVE FICAR dentro.")]
    [SerializeField] private BoxCollider2D mapBounds;

    [Header("Interação com Blocos (Visuais)")]
    [Tooltip("Alcance MÁXIMO do braço do player para quebrar um bloco.")]
    public float maxInteractionDistance = 3.5f;

    [Tooltip("O rotator que aponta o braço para os blocos.")]
    public ArmRotator armRotator;
    [Tooltip("Objeto que segura a arte do braço (ativado apenas ao mirar).")]
    [SerializeField] private GameObject armObject;
    [Tooltip("Sprite do personagem COM o braço recolhido (usado na mira).")]
    [SerializeField] private Sprite interactionSprite;

    private Sprite defaultSprite;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private Vector2 moveInput;
    private bool isInteracting;

    void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null) 
        {
            defaultSprite = spriteRenderer.sprite;
        }

        if (armObject != null)
        {
            armObject.SetActive(false);
        }

        // --- SISTEMA ANTI-BUGS DE TRANSIÇÃO ---
        // Como você não quer mais física, destruir o Rigidbody e Colliders antigos na largada
        // assim previne que a Unity trave o personagem misteriosamente.
        Rigidbody2D rbLocal = GetComponent<Rigidbody2D>();
        if (rbLocal != null) Destroy(rbLocal);
        
        CapsuleCollider2D capLocal = GetComponent<CapsuleCollider2D>();
        if (capLocal != null) Destroy(capLocal);
    }

    void Start()
    {
        // Força a gravidade ser 0!
        // NOTE: The original Awake method destroys Rigidbody2D components.
        // If a Rigidbody2D is added back or exists, this code will apply.
        // Otherwise, 'rb' would need to be declared and assigned.
        // For faithful injection, assuming 'rb' is accessible if intended.
        // However, given the destruction in Awake, this part might be problematic.
        // Proceeding with faithful injection as requested.
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous; 
        }

        // Impede que o colisor gigante do mapa roube os cliques do mouse destinados aos blocos
        if (mapBounds != null && mapBounds.gameObject != this.gameObject)
        {
            mapBounds.isTrigger = true; // Não colide com física
            mapBounds.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        }
    }

    // --- PROPRIEDADE PÚBLICA PARA A CÂMERA ACESSAR OS LIMITES ---
    public BoxCollider2D GetMapBounds()
    {
        return mapBounds;
    }

    void Update()
    {
        if (isInteracting)
        {
            moveInput = Vector2.zero;
            return; // Se estiver socando, bloqueia o fluxo de movimento
        }

        // 1. Pega os botões pressionados e "espreme" para valer no máximo 1
        moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

        // 2. Modifica a posição da imagem puramente pelo código e relógio do jogo (Sem Físicas)
        transform.position += (Vector3)(moveInput * moveSpeed * Time.deltaTime);

        // 3. Checa a trava de paredes matemáticas
        ApplyMapBounds();

        // 4. Desenha as Animações
        HandleSpriteFlip();
        HandleAnimations();
    }

    private void ApplyMapBounds()
    {
        if (mapBounds != null)
        {
            // Aborta a trava se as configurações no Inspector estiverem zeradas/erradas
            if (mapBounds.gameObject == this.gameObject) return;
            if (mapBounds.size.x <= 0.1f || mapBounds.size.y <= 0.1f) return;

            Bounds b = mapBounds.bounds;
            Vector3 clampedPos = transform.position;
            
            // Força a posição do Player a nunca passar dos limites do Box
            clampedPos.x = Mathf.Clamp(clampedPos.x, b.min.x, b.max.x);
            clampedPos.y = Mathf.Clamp(clampedPos.y, b.min.y, b.max.y);
            
            transform.position = clampedPos;
        }
    }

    public void StartInteraction(Transform targetBlock, float actionDelay)
    {
        StartCoroutine(InteractionRoutine(targetBlock, actionDelay));
    }

    private IEnumerator InteractionRoutine(Transform targetBlock, float actionDelay)
    {
        if (isInteracting) yield break;

        isInteracting = true;
        moveInput = Vector2.zero;

        // Desliga a animação base de correr e bota a imagem "mutilada" + braço atirador rotativo
        if (animator != null) animator.enabled = false;
        
        if (spriteRenderer != null && interactionSprite != null)
        {
            spriteRenderer.sprite = interactionSprite;
        }

        if (armObject != null && armRotator != null)
        {
            armObject.SetActive(true);
            armRotator.SetTarget(targetBlock);
        }

        yield return new WaitForSeconds(actionDelay);

        // Fim da ação de soco, reverte.
        if (armObject != null && armRotator != null)
        {
            armObject.SetActive(false);
            armRotator.SetTarget(null);
        }

        if (animator != null) animator.enabled = true;
        if (spriteRenderer != null) spriteRenderer.sprite = defaultSprite;

        isInteracting = false;
    }

    private void HandleSpriteFlip()
    {
        if (moveInput.x != 0 && spriteRenderer != null)
        {
            spriteRenderer.flipX = moveInput.x < 0; // Se andar para a esquerda, espelha!
        }
    }

    private void HandleAnimations()
    {
        if (animator != null && animator.enabled)
        {
            // Garante que o painel Animator antigo receba dados limpos sem dar Crash por falta do Rigidbody
            animator.SetBool("isRunning", moveInput.magnitude > 0);
            animator.SetBool("isGrounded", true); 
            animator.SetFloat("yVelocity", 0f);   
        }
    }
}
