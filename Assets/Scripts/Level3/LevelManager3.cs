using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement; // 1. Adicionamos a biblioteca de Cenas aqui!

public class LevelManager3 : MonoBehaviour
{
    public float timerLength;
    private float remainingTime;
    public TextMeshProUGUI timerText;

    [SerializeField] private string gameOverScene;

    [SerializeField] private string victoryScene;

    // 2. Trava de segurança para não carregar a cena infinitamente
    private bool isGameOver = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (timerLength == 0) timerLength = 300f;
        remainingTime = timerLength;
    }

    // Update is called once per frame
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
            FimDeJogo();
        }
    }

    void FimDeJogo()
    {
        isGameOver = true; // Ativamos a trava de segurança

        if (gameOverScene != "")
            SceneManager.LoadScene(gameOverScene);
        else
            SceneManager.LoadScene("Menu");
    }

    public void Vitoria()
    {
        if (victoryScene != "")
            SceneManager.LoadScene(victoryScene);
        else
            SceneManager.LoadScene("Menu");
    }


    /*
    Receita de miojo para quem nao sabia

    1 Coloque o fogo para ferver.
    2 Em seguida coloque as 4 colheres de catchup e mexa.
    3 Depois quando a água estiver fervendo ponhe o miojo.
    4 Em seguida coloque o tempero e as colheres de pimenta.
    5 Depois rale a mussarela em cima do miojo.
    6 Bom apetite!
     */
}