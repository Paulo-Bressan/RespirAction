using UnityEngine;
using System.Collections.Generic;

// --- COMPONENTES LÓGICOS (COMPOSIÇÃO) ---
// Estes sistemas dividem as responsabilidades do PlayerMovement original.

public class PlayerCollisionSystem
{
    private Transform groundCheck;
    private float groundRadius;
    private LayerMask whatIsGround;
    private CapsuleCollider2D capsuleCollider;
    private float wallCheckDistance;
    private Transform transform;

    public bool IsGrounded { get; private set; }

    public PlayerCollisionSystem(Transform groundCheck, float groundRadius, LayerMask whatIsGround, CapsuleCollider2D capsuleCollider, float wallCheckDistance, Transform transform)
    {
        this.groundCheck = groundCheck;
        this.groundRadius = groundRadius;
        this.whatIsGround = whatIsGround;
        this.capsuleCollider = capsuleCollider;
        this.wallCheckDistance = wallCheckDistance;
        this.transform = transform;
    }

    public void CheckGround()
    {
        IsGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, whatIsGround);
    }

    public float HandleWallCollision(float moveInput, float moveSpeed)
    {
        float finalXVelocity = moveInput * moveSpeed;

        if (moveInput != 0 && capsuleCollider != null)
        {
            Vector2 worldScale = transform.lossyScale;
            Vector2 worldSize = capsuleCollider.size * new Vector2(Mathf.Abs(worldScale.x), Mathf.Abs(worldScale.y));
            worldSize.x *= 0.9f;
            worldSize.y *= 0.85f;

            Vector2 worldCenter = transform.TransformPoint(capsuleCollider.offset);
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
                if (!IsGrounded || Mathf.Abs(wallHit.normal.x) > 0.5f)
                {
                    finalXVelocity = 0f;
                }
            }
        }
        return finalXVelocity;
    }
    
    public void DrawGizmos()
    {
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

public class PlayerGravitySystem
{
    private Rigidbody2D rb;
    private Transform transform;
    private float defaultGravityScale;
    
    public bool IsUpsideDown { get; private set; }

    public PlayerGravitySystem(Rigidbody2D rb, Transform transform)
    {
        this.rb = rb;
        this.transform = transform;
        this.defaultGravityScale = rb.gravityScale;
        this.IsUpsideDown = false;
    }

    public void UpdateGravity(float sineValue)
    {
        rb.gravityScale = defaultGravityScale * sineValue;

        if (sineValue < 0 && !IsUpsideDown)
            FlipOrientation();
        else if (sineValue > 0 && IsUpsideDown)
            FlipOrientation();
    }

    public void FlipOrientation()
    {
        IsUpsideDown = !IsUpsideDown;
        transform.Rotate(0, 0, 180f);
    }

    public void ResetOrientation()
    {
        if (IsUpsideDown)
            FlipOrientation();
        transform.rotation = Quaternion.identity;
        IsUpsideDown = false;
    }
}

public class PlayerInteractionSystem
{
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Sprite interactionSprite;
    private Sprite defaultSprite;
    private GameObject armObject;
    private ArmRotator armRotator;
    private Rigidbody2D rb;

    public bool IsMovementLocked { get; private set; }

    public PlayerInteractionSystem(Animator animator, SpriteRenderer spriteRenderer, Sprite interactionSprite, Sprite defaultSprite, GameObject armObject, ArmRotator armRotator, Rigidbody2D rb)
    {
        this.animator = animator;
        this.spriteRenderer = spriteRenderer;
        this.interactionSprite = interactionSprite;
        this.defaultSprite = defaultSprite;
        this.armObject = armObject;
        this.armRotator = armRotator;
        this.rb = rb;

        this.IsMovementLocked = false;
        
        if (this.armObject != null)
            this.armObject.SetActive(false);
    }

    public void SetInteractingState(bool isInteracting, Transform targetTile)
    {
        IsMovementLocked = isInteracting;
        
        if (isInteracting)
        {
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
}

public class PlayerTileSystem
{
    private List<InteractiveTile> allInteractiveTiles;
    private InteractiveTile currentTargetTile;
    private CameraController camController;

    public void Initialize(CameraController camController)
    {
        this.camController = camController;
        allInteractiveTiles = new List<InteractiveTile>(Object.FindObjectsByType<InteractiveTile>(FindObjectsSortMode.None));

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

            currentTargetTile = startingTile != null ? startingTile : allInteractiveTiles[0];
            currentTargetTile.SetAsTarget(true);
        }
    }

    public void TeleportToRandomCheckpoint(GameObject destroyedTileObj)
    {
        if (allInteractiveTiles == null || allInteractiveTiles.Count == 0) return;

        allInteractiveTiles.RemoveAll(tile => tile.gameObject == destroyedTileObj);

        if (allInteractiveTiles.Count > 0)
        {
            int randomIndex = Random.Range(0, allInteractiveTiles.Count);
            InteractiveTile nextTileTarget = allInteractiveTiles[randomIndex];

            if (nextTileTarget != null)
            {
                currentTargetTile = nextTileTarget;
                currentTargetTile.SetAsTarget(true);
                if (camController != null)
                {
                    camController.LookAtTarget(nextTileTarget.transform, 2.0f);
                }
            }
        }
    }
}
