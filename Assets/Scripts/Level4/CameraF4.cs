using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraF4 : MonoBehaviour
{
    [Header("Target & Movement")]
    [Tooltip("O jogador ou objeto que a câmera vai seguir.")]
    [SerializeField] private Transform target;

    [Tooltip("Velocidade com a qual a câmera segue o alvo.")]
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10f);

    [Header("Camera Bounds")]
    [Tooltip("Se ativado, a câmera não sairá do Collider especificado (Tenta pegar o do Player automaticamente).")]
    [SerializeField] private bool useBounds = true;
    
    [Tooltip("Opcional: O BoxCollider2D delimitando o cenário. Se vazio, a câmera pegará os Limites configurados dentro do Player.")]
    [SerializeField] private BoxCollider2D boundsCollider;

    private Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;

        // Auto-busca o map bounds do Player caso o campo na Câmera esteja vazio
        if (useBounds && boundsCollider == null)
        {
            PlayerMovementF4 player = target.GetComponent<PlayerMovementF4>();
            if (player != null)
            {
                boundsCollider = player.GetMapBounds();
            }
        }

        // Limita a posição baseada nos Bounds
        if (useBounds && boundsCollider != null)
        {
            float cameraHalfHeight = cam.orthographicSize;
            float cameraHalfWidth = cam.aspect * cameraHalfHeight;

            Bounds colliderBounds = boundsCollider.bounds;

            float minX = colliderBounds.min.x + cameraHalfWidth;
            float maxX = colliderBounds.max.x - cameraHalfWidth;
            float minY = colliderBounds.min.y + cameraHalfHeight;
            float maxY = colliderBounds.max.y - cameraHalfHeight;

            float clampX = desiredPosition.x;
            if (minX > maxX) clampX = colliderBounds.center.x; // Centraliza se estourar
            else clampX = Mathf.Clamp(desiredPosition.x, minX, maxX);

            float clampY = desiredPosition.y;
            if (minY > maxY) clampY = colliderBounds.center.y; // Centraliza se estourar
            else clampY = Mathf.Clamp(desiredPosition.y, minY, maxY);

            desiredPosition = new Vector3(clampX, clampY, desiredPosition.z);
        }

        // Suaviza o movimento
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
    }
}
