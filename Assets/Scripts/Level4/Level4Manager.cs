using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Level4Manager : MonoBehaviour
{
    [Header("Survival Settings")]
    [Tooltip("Tempo em segundos que o jogador precisa sobreviver.")]
    [SerializeField] private float survivalTime = 60f;
    
    [Header("Transition Settings")]
    [Tooltip("Nome da cena a carregar em caso de vitória (tempo esgotado).")]
    [SerializeField] private string nextSceneName;
    [Tooltip("Nome da cena em caso de falha (Game Over, topo atingido). Na maioria dos casos é a mesma cena.")]
    [SerializeField] private string currentSceneName;

    private float _timeRemaining;
    private bool _gameOverTriggered = false;

    void Start()
    {
        _timeRemaining = survivalTime;

        if (string.IsNullOrEmpty(currentSceneName))
        {
            currentSceneName = SceneManager.GetActiveScene().name;
        }
    }

    void Update()
    {
        if (_gameOverTriggered) return;

        _timeRemaining -= Time.deltaTime;

        if (Mathf.CeilToInt(_timeRemaining) % 5 == 0) // Log de vez em quando
        {
            // Debug.Log($"Tempo sobrando: {Mathf.CeilToInt(_timeRemaining)}s");
        }

        if (_timeRemaining <= 0f)
        {
            TriggerVictory();
        }
    }

    public void TriggerGameOver()
    {
        if (_gameOverTriggered) return;
        _gameOverTriggered = true;

        Debug.Log("GAME OVER! A pilha entupiu. Reiniciando a fase...");
        StartCoroutine(RestartRoutine());
    }

    private void TriggerVictory()
    {
        if (_gameOverTriggered) return;
        _gameOverTriggered = true;

        Debug.Log("VITÓRIA! Tempo esgotado e a pilha não entupiu.");
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }

    private IEnumerator RestartRoutine()
    {
        // Add death transition / fade in future if needed
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(currentSceneName);
    }
}
