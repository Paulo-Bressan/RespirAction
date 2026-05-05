using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class Level4Manager : MonoBehaviour
{
    [Header("Configuração de tempo")]
    [Tooltip("Tempo em segundos que o jogador precisa sobreviver.")]
    [SerializeField] private float timerLength = 60f;
    [Tooltip("Tempo em segundos que sobra para acabar (apenas para visualização)")]
    [SerializeField] private float remainingTime = 60f;
    [Tooltip("Objeto de texto do timer na UI")]
    [SerializeField] private TextMeshProUGUI timerText;


    [Header("Transition Settings")]
    [Tooltip("Nome da cena a carregar em caso de vitória (tempo esgotado).")]
    [SerializeField] private string nextSceneName;
    [Tooltip("Nome da cena em caso de falha (Game Over, topo atingido).")]
    [SerializeField] private string gameOverSceneName;

    // Trava de segurança para não carregar a cena infinitamente
    private bool isGameOver = false;


    void Start()
    {
        remainingTime = timerLength;

        if (string.IsNullOrEmpty(gameOverSceneName))
            gameOverSceneName = "Menu";
    }

    void Update()
    {
        // Se o jogo ja acabou, ignoramos o resto do codigo
        if (isGameOver) return;

        // Atualiza o tempo restante
        if (remainingTime > 0)
            remainingTime = timerLength - TimeManager.instance.elapsedTime;

        if (TimeManager.instance != null && timerText != null)
        {
            // Usei Mathf.Max(0, ...) para o texto não mostrar tempo negativo (ex: -00:01)
            int minutes = Mathf.FloorToInt(Mathf.Max(0, remainingTime) / 60);
            int seconds = Mathf.FloorToInt(Mathf.Max(0, remainingTime) % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
        // 3. Verifica se o tempo acabou
        if (remainingTime <= 0)
        {
            TriggerVictory();
        }
    }

    public void TriggerGameOver()
    {
        if (isGameOver) return;
        isGameOver = true;

        Debug.Log("GAME OVER! A pilha entupiu. Prosseguindo para tela GameOver...");
        StartCoroutine(GameOverRoutine());
    }

    private void TriggerVictory()
    {
        if (isGameOver) return;
        isGameOver = true;

        Debug.Log("VITÓRIA! Tempo esgotado e a pilha não entupiu.");
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }

    private IEnumerator GameOverRoutine()
    {
        // Add death transition / fade in future if needed
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(gameOverSceneName);
    }
}
