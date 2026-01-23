using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour
{
    private float horizontalInput;
    private bool jumpRequest;
    private List<InteractiveTile> allInteractiveTiles = new List<InteractiveTile>(); 
    
    private InteractiveTile currentTargetTile = null;

    [Header("Movimento")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Pulo & Detecção de Chão")]
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private float groundRadius = 0.2f;

    [Header("Detecção de Parede (Anti-Flick)")]
    [Tooltip("Distância do raio a partir da borda do collider.")]
    [SerializeField] private float wallCheckDistance = 0.05f;
    
    private bool isGrounded;

    [Header("Mecânica de Gravidade")]
    private bool isUpsideDown = false;
    private float defaultGravityScale;

    [Header("Respawn & Interação")]
    [SerializeField] private Sprite interactionSprite; 
    [SerializeField] private GameObject armObject;

    [Header("Controle de Câmera")]
    [SerializeField] private Transform cameraTargetOverride;
    [SerializeField] private float cameraFocusTime = 2.0f;
    
    private Vector3 respawnPoint; 
    private Sprite defaultSprite;
    
    private Rigidbody2D rb;
    private CapsuleCollider2D capsuleCollider; 
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private ArmRotator armRotator;

    private float moveInput;    
    private bool isMovementLocked = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>(); 
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        if (capsuleCollider == null)
        {
            Debug.LogError("PlayerMovement precisa de um CapsuleCollider2D!");
        }

        defaultGravityScale = rb.gravityScale;
        
        respawnPoint = transform.position;
        if (spriteRenderer != null) defaultSprite = spriteRenderer.sprite;

        if (armObject != null)
        {
            armRotator = armObject.GetComponent<ArmRotator>();
            armObject.SetActive(false); 
        }

        allInteractiveTiles = new List<InteractiveTile>(FindObjectsByType<InteractiveTile>(FindObjectsSortMode.None));

        foreach (var tile in allInteractiveTiles)
        {
            tile.SetAsTarget(false);
        }

        if (allInteractiveTiles.Count > 0)
        {
            InteractiveTile startingTile = null;

            foreach (var tile in allInteractiveTiles)
            {
                if (tile.name.Trim().Equals("tileHurt (1)")) 
                {
                    startingTile = tile;
                    break; 
                }
            }

            if (startingTile != null)
            {
                currentTargetTile = startingTile;
            }
            else
            {
                currentTargetTile = allInteractiveTiles[0];
            }

            currentTargetTile.SetAsTarget(true);
        }
    }

    void Update()
    {
        if (!isMovementLocked)
        {
            moveInput = Input.GetAxisRaw("Horizontal");

            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                jumpRequest = true;
            }
        }
        else
        {
            moveInput = 0f;
            jumpRequest = false;
        }

        if (moveInput != 0)
        {
            if (moveInput > 0)
            {
                spriteRenderer.flipX = isUpsideDown;
            }
            else if (moveInput < 0)
            {
                spriteRenderer.flipX = !isUpsideDown;
            }
        }
        
        if (animator != null && animator.enabled)
        {
            animator.SetBool("isRunning", moveInput != 0);

            float relativeYVelocity = rb.linearVelocity.y;

            if (isUpsideDown) 
            {
                relativeYVelocity *= -1; 
            }

            animator.SetFloat("yVelocity", relativeYVelocity); 
        }

        if (TimeManager.instance != null)
        {
            float sineValue = TimeManager.instance.timeSineWave;

            rb.gravityScale = defaultGravityScale * sineValue;

            if (sineValue < 0 && !isUpsideDown)
            {
                FlipOrientation();
            }
            else if (sineValue > 0 && isUpsideDown)
            {
                FlipOrientation();
            }
        }
    }

    void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, whatIsGround);

        if (animator != null && animator.enabled)
        {
            animator.SetBool("isGrounded", isGrounded);
        }

        float finalXVelocity = moveInput * moveSpeed;
        
        // --- LÓGICA DE DETECÇÃO DE PAREDE (CORRIGIDA COM ESCALA) ---
        if (moveInput != 0 && capsuleCollider != null)
        {
            // 1. Calcula o tamanho REAL no mundo (Tamanho Local * Escala do Objeto)
            Vector2 worldScale = transform.lossyScale;
            Vector2 worldSize = capsuleCollider.size * new Vector2(Mathf.Abs(worldScale.x), Mathf.Abs(worldScale.y));

            // 2. Reduz levemente para evitar colisão com chão e teto
            worldSize.x *= 0.9f;  // Mais estreito
            worldSize.y *= 0.85f; // Mais baixo (crucial para terreno irregular)

            // 3. Calcula o centro REAL no mundo (considerando rotação e offset)
            Vector2 worldCenter = transform.TransformPoint(capsuleCollider.offset);

            // 4. Define direção baseada no input (ignora rotação do personagem para ser consistente)
            Vector2 direction = moveInput > 0 ? Vector2.right : Vector2.left;

            RaycastHit2D wallHit = Physics2D.CapsuleCast(
                worldCenter, 
                worldSize, 
                capsuleCollider.direction, 
                0f, 
                direction, 
                wallCheckDistance, 
                whatIsGround
            );

            if (wallHit.collider != null)
            {
                // Verifica se a colisão é uma parede vertical (ignora rampas suaves)
                if (!isGrounded || Mathf.Abs(wallHit.normal.x) > 0.5f)
                {
                    finalXVelocity = 0f;
                }
            }
        }

        rb.linearVelocity = new Vector2(finalXVelocity, rb.linearVelocity.y);

        if (jumpRequest)
        {
            float jumpVel = isUpsideDown ? -jumpForce : jumpForce;
            
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpVel);
            
            jumpRequest = false; 
        }
    }

    void FlipOrientation()
    {
        isUpsideDown = !isUpsideDown;
        transform.Rotate(0, 0, 180f);
    }
    
    public void UpdateCheckpoint(Vector3 newPosition)
    {
        respawnPoint = newPosition;
    }
    
    public void Respawn()
    {
        transform.position = respawnPoint;
        rb.linearVelocity = Vector2.zero; 

        if (isUpsideDown)
        {
            FlipOrientation(); 
        }

        transform.rotation = Quaternion.identity;
        isUpsideDown = false;
    }

    public void SetInteractingState(bool isInteracting, Transform targetTile)
    {
        if (isInteracting)
        {
            isMovementLocked = true;
            rb.linearVelocity = Vector2.zero;
            
            if (animator != null) animator.enabled = false; 
            if (spriteRenderer != null && interactionSprite != null)
                spriteRenderer.sprite = interactionSprite;
            
            if (armObject != null && armRotator != null)
            {
                armObject.SetActive(true);
                armRotator.SetTarget(targetTile);
            }
        }
        else
        {
            isMovementLocked = false;
            
            if (animator != null) animator.enabled = true; 
            if (spriteRenderer != null)
                spriteRenderer.sprite = defaultSprite; 
            
            if (armObject != null && armRotator != null)
            {
                armObject.SetActive(false);
                armRotator.SetTarget(null);
            }
        }
    }

    public void TeleportToRandomCheckpoint(GameObject destroyedTileObj)
    {
        if (allInteractiveTiles == null || allInteractiveTiles.Count == 0)
        {
            return;
        }

        allInteractiveTiles.RemoveAll(tile => tile.gameObject == destroyedTileObj);

        if (allInteractiveTiles.Count > 0)
        {
            int randomIndex = Random.Range(0, allInteractiveTiles.Count);
            InteractiveTile nextTileTarget = allInteractiveTiles[randomIndex];

            if (nextTileTarget != null)
            {
                currentTargetTile = nextTileTarget;
                currentTargetTile.SetAsTarget(true);
            }
        }
    }

    private IEnumerator FocusCameraOnCheckpoint(Vector3 targetPosition)
    {
        cameraTargetOverride.position = targetPosition;
        yield return new WaitForSeconds(cameraFocusTime);
    }

    // GIZMOS CORRIGIDOS PARA REFLETIR O TAMANHO REAL NO MUNDO
    private void OnDrawGizmos()
    {
        if (capsuleCollider == null) capsuleCollider = GetComponent<CapsuleCollider2D>();
        
        if (capsuleCollider != null)
        {
            Gizmos.color = Color.magenta;

            // 1. Recalcula o tamanho baseando-se na escala global do objeto
            Vector2 worldScale = transform.lossyScale;
            Vector2 worldSize = capsuleCollider.size * new Vector2(Mathf.Abs(worldScale.x), Mathf.Abs(worldScale.y));
            
            // Aplica as reduções usadas na lógica física
            worldSize.x *= 0.9f;
            worldSize.y *= 0.85f;

            // 2. Calcula o centro em coordenadas de mundo
            Vector3 worldCenter = transform.TransformPoint(capsuleCollider.offset);

            // 3. Define a posição de desenho baseada na direção que o player olharia (direita por padrão para debug)
            // Nota: No gizmo desenhamos estático à direita para visualização, 
            // mas em jogo ele muda com o input.
            Vector3 drawCenter = worldCenter + (Vector3.right * wallCheckDistance);

            Gizmos.DrawWireCube(drawCenter, new Vector3(worldSize.x, worldSize.y, 1));
        }
    }
}