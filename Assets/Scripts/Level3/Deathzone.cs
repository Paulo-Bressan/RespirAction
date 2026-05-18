using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Objeto que força o jogador a renascer no último checkpoint ao colidir,
/// com uma transição suave de fade in/out na tela configurável pelo Inspector.
/// </summary>
public class Deathzone : MonoBehaviour
{
    [Header("Configurações do Fade")]
    [Tooltip("Duração do escurecimento da tela (ida).")]
    [SerializeField] private float fadeOutDuration = 0.5f;

    [Tooltip("Tempo em que a tela fica totalmente da cor escolhida antes do respawn.")]
    [SerializeField] private float waitTimeInBlack = 0.3f;

    [Tooltip("Duração do clareamento da tela (volta).")]
    [SerializeField] private float fadeInDuration = 0.5f;

    [Tooltip("A cor da tela de morte.")]
    [SerializeField] private Color fadeColor = Color.black;

    private static Image sharedFadeImage;
    private bool isPlayerDying = false;

    private void Start()
    {
        SetupFadeUI();
    }

    private void SetupFadeUI()
    {
        // Se a imagem compartilhada já foi criada por outra deathzone, reutiliza.
        if (sharedFadeImage != null) return;

        // Procura na cena se já existe um canvas preexistente
        GameObject existingCanvas = GameObject.Find("DeathzoneFadeCanvas");
        if (existingCanvas != null)
        {
            sharedFadeImage = existingCanvas.GetComponentInChildren<Image>();
            return;
        }

        // Caso não exista, cria um dinamicamente
        GameObject canvasObj = new GameObject("DeathzoneFadeCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999; // Fica na frente de tudo

        canvasObj.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;

        GameObject imageObj = new GameObject("FadeImage");
        imageObj.transform.SetParent(canvasObj.transform, false);
        sharedFadeImage = imageObj.AddComponent<Image>();

        // Preenche a tela toda
        RectTransform rectTransform = sharedFadeImage.GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.sizeDelta = Vector2.zero;
        rectTransform.anchoredPosition = Vector2.zero;

        // Começa transparente
        Color startColor = fadeColor;
        startColor.a = 0f;
        sharedFadeImage.color = startColor;

        // Para evitar perda ao trocar de cena caso seja persistente
        DontDestroyOnLoad(canvasObj);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isPlayerDying) return;

        // Verifica se o objeto que colidiu é o Jogador
        if (other.CompareTag("player") || other.CompareTag("Player"))
        {
            PlayerMovement player = other.GetComponent<PlayerMovement>();

            if (player != null)
            {
                StartCoroutine(DeathTransitionRoutine(player));
            }
        }
    }

    private IEnumerator DeathTransitionRoutine(PlayerMovement player)
    {
        isPlayerDying = true;

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        Animator anim = player.GetComponent<Animator>();

        // Pausa movimento (simples)
        if (rb != null) rb.linearVelocity = Vector2.zero;
        if (anim != null) anim.enabled = false;
        player.enabled = false;

        // 1. Fade Out
        float elapsed = 0f;
        Color c = fadeColor;
        
        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Clamp01(elapsed / fadeOutDuration);
            if (sharedFadeImage != null) sharedFadeImage.color = c;
            yield return null;
        }

        c.a = 1f;
        if (sharedFadeImage != null) sharedFadeImage.color = c;

        // 2. Aguarda um pouco com tela fechada
        yield return new WaitForSeconds(waitTimeInBlack);

        // Dá de fato o respawn
        player.Respawn();
        if (rb != null) rb.linearVelocity = Vector2.zero;

        // 3. Fade In
        elapsed = 0f;
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            c.a = 1f - Mathf.Clamp01(elapsed / fadeInDuration);
            if (sharedFadeImage != null) sharedFadeImage.color = c;
            yield return null;
        }

        c.a = 0f;
        if (sharedFadeImage != null) sharedFadeImage.color = c;

        // Restaura player
        player.enabled = true;
        if (anim != null) anim.enabled = true;
        isPlayerDying = false;
    }
}