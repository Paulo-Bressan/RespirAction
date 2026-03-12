using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    private Transform customTarget; // O objeto para onde a câmera vai
    private bool isTransitioning = false; // Flag de controle
    private float smoothSpeed = 3.0f; // Ajuste para mais lento ou mais rápido
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

    public void LookAtTarget(Transform target, float waitTime)
    {
        StartCoroutine(LookAtTargetRoutine(target, waitTime));
    }

    private IEnumerator LookAtTargetRoutine(Transform target, float time)
    {
        // 1. Inicia a transição para o objeto
        isTransitioning = true;
        isFollowingPlayer = false; // Desativa o snap direto no player
        customTarget = target;

        // 2. Fica focado no objeto pelo tempo determinado
        yield return new WaitForSeconds(time);

        // 3. Inicia a volta para o player (definindo alvo como null, a lógica cai no player)
        customTarget = null;
        
        // 4. Dá um tempo para a câmera voltar suavemente ao player antes de travar nela de novo
        // Esse tempo (1.5f) garante que a câmera chegue no player via Lerp antes de ativar o Bounds
        yield return new WaitForSeconds(1.5f);

        // 5. Devolve o controle normal
        isTransitioning = false;
        isFollowingPlayer = true;
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
        //mainCamera.aspect = bounds.size.x / bounds.size.y;
    }

    void LateUpdate()
    {
        // Se estiver no modo Mapa (M), ignora tudo isso
        if (!isFollowingPlayer && !isTransitioning) return;
        
        Vector3 finalPosition = transform.position;
        float zPos = normalZPosition;

        // --- DEFINIÇÃO DO ALVO ---
        Vector3 targetPos;
        
        if (isTransitioning && customTarget != null)
        {
            // Se estamos focando no objeto
            targetPos = customTarget.position;
        }
        else if (playerTransform != null)
        {
            // Se estamos focando no player (ou voltando para ele)
            targetPos = playerTransform.position;
        }
        else
        {
            return; 
        }

        targetPos.z = zPos;

        // --- APLICAÇÃO DOS LIMITES (BOUNDS) NO ALVO ---
        // Calculamos onde a câmera DEVERIA estar dentro dos limites
        if (useBounds && boundsCollider != null)
        {
            float cameraHalfHeight = mainCamera.orthographicSize;
            float cameraHalfWidth = mainCamera.aspect * cameraHalfHeight;

            Bounds colliderBounds = boundsCollider.bounds;
            float minX = colliderBounds.min.x + cameraHalfWidth;
            float maxX = colliderBounds.max.x - cameraHalfWidth;
            float minY = colliderBounds.min.y + cameraHalfHeight;
            float maxY = colliderBounds.max.y - cameraHalfHeight;

            if (minX > maxX) targetPos.x = colliderBounds.center.x;
            else targetPos.x = Mathf.Clamp(targetPos.x, minX, maxX);

            if (minY > maxY) targetPos.y = colliderBounds.center.y;
            else targetPos.y = Mathf.Clamp(targetPos.y, minY, maxY);
        }

        // --- MOVIMENTO FINAL ---
        if (isTransitioning)
        {
            // Movimento FLUIDO (Lerp) durante a cutscene ou retornando ao player
            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * smoothSpeed);
        }
        else
        {
            // Movimento SECO (Instantâneo) durante gameplay normal (para resposta rápida)
            transform.position = targetPos;
        }
    }
}