using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class VideoPlayerScript : MonoBehaviour
{
    [Tooltip("Componente de VideoPlayer")]
    [SerializeField] private VideoPlayer videoPlayer;

    [Tooltip("Define a cena a ser carregada após o vídeo")]
    [SerializeField] private string nextSceneName = "Menu";

    [Tooltip("Objeto do botao de pausa")]
    [SerializeField] private GameObject botaoPausa;

    [Tooltip("Objeto do botao de resumir")]
    [SerializeField] private GameObject botaoResumir;

    void Start()
    {
        // Certifica-se que a biblioteca UnityEngine.Video está no seu projeto.
        if (videoPlayer == null)
        {
            Debug.LogError("[CUTSCENE] VideoPlayer não está configurado. Atribua o componente VideoPlayer no Inspector.");
            return;
        }

        // Assina o evento para quando o vídeo terminar
        videoPlayer.loopPointReached += OnVideoEnd;

        // Inicia a reprodução do vídeo
        videoPlayer.time = 0;
        videoPlayer.Play();
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        Debug.Log("[CUTSCENE] Vídeo de introdução terminou. Carregando a cena: " + nextSceneName);
        
        // Carrega a próxima cena (Fase1)
        SceneManager.LoadScene(nextSceneName);
    }

    public void OnPause()
    {
        Debug.Log("[CUTSCENE] Pausando vídeo");
        botaoPausa.SetActive(false);
        botaoResumir.SetActive(true);
        videoPlayer.Pause();
    }

    public void OnResume()
    {
        Debug.Log("[CUTSCENE] Resumindo vídeo");
        botaoPausa.SetActive(true);
        botaoResumir.SetActive(false);
        videoPlayer.Play();
    }

    public void OnRestart()
    {
        Debug.Log("[CUTSCENE] Reiniciando vídeo");
        videoPlayer.time = 0;
        botaoPausa.SetActive(true);
        botaoResumir.SetActive(false);
        videoPlayer.Play();
    }
}