using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [Header("Alvo")]
    [SerializeField] private Transform playerTransform;

    [Header("Modo Mapa")]
    [SerializeField] private KeyCode mapKey = KeyCode.M;
    [SerializeField] private float normalZPosition = -10f;
    [SerializeField] private float mapZPosition = -50f; 

    [Header("Limites da Câmera")]
    [SerializeField] private bool useBounds = true;
    [SerializeField] private BoxCollider2D boundsCollider;

    private Camera mainCamera;
    private bool isFollowingPlayer = true;
    private float gameplayOrthographicSize; 

    void Start()
    {
        mainCamera = GetComponent<Camera>();
        gameplayOrthographicSize = mainCamera.orthographicSize;

        if (playerTransform != null)
        {
            transform.position = new Vector3(playerTransform.position.x, playerTransform.position.y, normalZPosition);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(mapKey))
        {
            isFollowingPlayer = !isFollowingPlayer;

            if (isFollowingPlayer)
            {
                ReturnToGameplay();
            }
            else
            {
                FocusOnBoundsStretch();
            }
        }
    }

    void ReturnToGameplay()
    {
        // Reseta o aspecto para o padrão do monitor/janela
        mainCamera.ResetAspect();
        mainCamera.orthographicSize = gameplayOrthographicSize;
    }

    void FocusOnBoundsStretch()
    {
        if (boundsCollider == null)
        {
            transform.position = new Vector3(0, 0, mapZPosition);
            return;
        }

        Bounds bounds = boundsCollider.bounds;

        // Centraliza a câmera
        transform.position = new Vector3(bounds.center.x, bounds.center.y, mapZPosition);

        // 1. Define a altura exata da câmera para bater com a altura do mapa
        mainCamera.orthographicSize = bounds.size.y * 0.5f;

        // 2. Força o Aspect Ratio da câmera ser idêntico ao do Mapa.
        // Isso causará a distorção (esticamento) visual se a tela for diferente do mapa.
        mainCamera.aspect = bounds.size.x / bounds.size.y;
    }

    void LateUpdate()
    {
        if (!isFollowingPlayer) return;
        if (playerTransform == null) return;
        
        Vector3 targetPosition = new Vector3(playerTransform.position.x, playerTransform.position.y, normalZPosition);

        if (useBounds && boundsCollider != null)
        {
            float cameraHalfHeight = mainCamera.orthographicSize;
            float cameraHalfWidth = mainCamera.aspect * cameraHalfHeight;

            Bounds colliderBounds = boundsCollider.bounds;

            float minX = colliderBounds.min.x + cameraHalfWidth;
            float maxX = colliderBounds.max.x - cameraHalfWidth;
            float minY = colliderBounds.min.y + cameraHalfHeight;
            float maxY = colliderBounds.max.y - cameraHalfHeight;

            if (minX > maxX) targetPosition.x = colliderBounds.center.x;
            else targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);

            if (minY > maxY) targetPosition.y = colliderBounds.center.y;
            else targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);
        }
        
        transform.position = targetPosition;
    }
}