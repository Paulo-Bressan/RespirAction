using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
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

    [Header("Respawn & Interação")]
    [SerializeField] private Sprite interactionSprite; 
    [SerializeField] private GameObject armObject;

    [Header("Controle de Câmera")]
    [SerializeField] private Transform cameraTargetOverride;
    [SerializeField] private float cameraFocusTime = 2.0f;

    private CameraController camController;
    private Rigidbody2D rb;
    private CapsuleCollider2D capsuleCollider; 
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private ArmRotator armRotator;

    private float moveInput;    
    private bool jumpRequest;
    private Vector3 respawnPoint; 
    private Sprite defaultSprite;

    // --- SISTEMAS MODULARES (COMPOSIÇÃO) ---
    private PlayerCollisionSystem collisionSystem;
    private PlayerGravitySystem gravitySystem;
    private PlayerInteractionSystem interactionSystem;
    private PlayerTileSystem tileSystem;

    // --- PROPRIEDADES EXPOSTAS PARA O SOUND MANAGER ---
    public bool IsGrounded => collisionSystem != null && collisionSystem.IsGrounded;
    public bool IsMoving => rb != null && Mathf.Abs(rb.linearVelocity.x) > 0.1f && (interactionSystem != null && !interactionSystem.IsMovementLocked);
    public bool IsInteracting => interactionSystem != null && interactionSystem.IsMovementLocked;
    public bool IsJumping => !IsGrounded && rb != null && gravitySystem != null && (gravitySystem.IsUpsideDown ? rb.linearVelocity.y < -0.1f : rb.linearVelocity.y > 0.1f);
    public bool IsFalling => !IsGrounded && rb != null && gravitySystem != null && (gravitySystem.IsUpsideDown ? rb.linearVelocity.y > 0.1f : rb.linearVelocity.y < -0.1f);

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        animator = GetComponent<Animator>(); 
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (armObject != null)
        {
            armRotator = armObject.GetComponent<ArmRotator>();
        }

        if (spriteRenderer != null) 
        {
            defaultSprite = spriteRenderer.sprite;
        }
    }

    void Start()
    {   
        if (Camera.main != null)
            camController = Camera.main.GetComponent<CameraController>();
        
        // Inicialização dos Sistemas Modulares
        collisionSystem = new PlayerCollisionSystem(groundCheck, groundRadius, whatIsGround, capsuleCollider, wallCheckDistance, transform);
        gravitySystem = new PlayerGravitySystem(rb, transform);
        interactionSystem = new PlayerInteractionSystem(animator, spriteRenderer, interactionSprite, defaultSprite, armObject, armRotator, rb);
        
        tileSystem = new PlayerTileSystem();
        tileSystem.Initialize(camController);
        
        respawnPoint = transform.position;
    }

    void Update()
    {
        if (interactionSystem != null && !interactionSystem.IsMovementLocked)
        {
            moveInput = Input.GetAxisRaw("Horizontal");

            if (Input.GetButtonDown("Jump") && IsGrounded)
            {
                jumpRequest = true;
            }
        }
        else
        {
            moveInput = 0f;
            jumpRequest = false;
        }

        HandleSpriteFlip();
        HandleAnimations();

        if (TimeManager.instance != null && gravitySystem != null)
        {
            gravitySystem.UpdateGravity(TimeManager.instance.timeSineWave);
        }
    }

    void FixedUpdate()
    {
        if (collisionSystem == null || gravitySystem == null) return;

        collisionSystem.CheckGround();

        if (animator != null && animator.enabled)
        {
            animator.SetBool("isGrounded", IsGrounded);
        }

        float finalXVelocity = collisionSystem.HandleWallCollision(moveInput, moveSpeed);

        rb.linearVelocity = new Vector2(finalXVelocity, rb.linearVelocity.y);

        if (jumpRequest)
        {
            float jumpVel = gravitySystem.IsUpsideDown ? -jumpForce : jumpForce;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpVel);
            jumpRequest = false; 
        }
    }

    private void HandleSpriteFlip()
    {
        if (moveInput != 0 && spriteRenderer != null && gravitySystem != null)
        {
            if (moveInput > 0)
            {
                spriteRenderer.flipX = gravitySystem.IsUpsideDown;
            }
            else if (moveInput < 0)
            {
                spriteRenderer.flipX = !gravitySystem.IsUpsideDown;
            }
        }
    }

    private void HandleAnimations()
    {
        if (animator != null && animator.enabled && gravitySystem != null && rb != null)
        {
            animator.SetBool("isRunning", moveInput != 0);

            float relativeYVelocity = rb.linearVelocity.y;
            
            if (gravitySystem.IsUpsideDown) 
            {
                relativeYVelocity *= -1; 
            }

            animator.SetFloat("yVelocity", relativeYVelocity); 
        }
    }
    
    public void UpdateCheckpoint(Vector3 newPosition)
    {
        respawnPoint = newPosition;
    }
    
    public void Respawn()
    {
        transform.position = respawnPoint;

        if (rb != null)
            rb.linearVelocity = Vector2.zero; 

        if (gravitySystem != null)
            gravitySystem.ResetOrientation();
    }

    public void SetInteractingState(bool isInteracting, Transform targetTile)
    {
        if (interactionSystem != null)
        {
            interactionSystem.SetInteractingState(isInteracting, targetTile);
        }
    }

    public void TeleportToRandomCheckpoint(GameObject destroyedTileObj)
    {
        if (tileSystem != null)
        {
            tileSystem.TeleportToRandomCheckpoint(destroyedTileObj);
        }
    }

    private IEnumerator FocusCameraOnCheckpoint(Vector3 targetPosition)
    {
        if (cameraTargetOverride != null)
        {
            cameraTargetOverride.position = targetPosition;
        }
        yield return new WaitForSeconds(cameraFocusTime);
    }

    private void OnDrawGizmos()
    {
        if (collisionSystem != null)
        {
            collisionSystem.DrawGizmos();
        }
        else
        {
            if (capsuleCollider == null) capsuleCollider = GetComponent<CapsuleCollider2D>();
            
            if (capsuleCollider != null)
            {
                Gizmos.color = Color.magenta;
                Vector2 worldScale = transform.lossyScale;
                Vector2 worldSize = capsuleCollider.size * new Vector2(Mathf.Abs(worldScale.x), Mathf.Abs(worldScale.y));
                worldSize.x *= 0.9f;
                worldSize.y *= 0.85f;

                Vector3 worldCenter = transform.TransformPoint(capsuleCollider.offset);
                Vector3 drawCenter = worldCenter + (Vector3.right * wallCheckDistance);

                Gizmos.DrawWireCube(drawCenter, new Vector3(worldSize.x, worldSize.y, 1));
            }
        }
    }
}