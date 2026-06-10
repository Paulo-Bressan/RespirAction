using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Tooltip("Objeto pai de todos componentes do menu de pausa")]
    [SerializeField] private GameObject pauseMenu;
    [Tooltip("Script do slider de volume")]
    [SerializeField] private VolumeSlider volumeSlider;
    [Tooltip("Nome da cena ao ser carregada com o botao de restart")]
    [SerializeField] private string cenaRestart;
    [Tooltip("Nome da cena ao ser carregada com o botao de home")]
    [SerializeField] private string cenaHome;

    // referencia, encontrada automaticamente
    private AudioManagerMenu audioManagerMenu;
    // flag de pausa, usado por scripts externos
    public bool isPaused = false;

    public void Start()
    {
        audioManagerMenu = FindFirstObjectByType<AudioManagerMenu>();
        if (!pauseMenu)
            Debug.LogWarning("[MENU] Objeto do menu de pausa faltando");
        if (!volumeSlider)
            Debug.LogWarning("[MENU] Script do slider de volume faltando");
    }

    public void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            Debug.Log("[MENU] Esc precionado");
            if (isPaused) Resume();
            else Pause();
        }
    }

    public void Pause()
    {
        Debug.Log("[MENU] Pausando jogo");
        audioManagerMenu.PlayEffectSound();
        isPaused = true;
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        Debug.Log("[MENU] Resumindo jogo");
        isPaused = false;
        audioManagerMenu.PlaySelectSound();
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
    }

    public void Restart()
    {

    Debug.Log("[MENU] Reiniciando fase (carregando cena " + cenaRestart + ")");
    isPaused = false;
    if (pauseMenu != null)
    {
        pauseMenu.SetActive(false); 
    }

    audioManagerMenu.PlaySelectSound();
    Time.timeScale = 1f;

    if (TimeManager.instance != null)
    {
        TimeManager.instance.elapsedTime = 0f; 
    }

    SceneManager.LoadScene(cenaRestart);
    
    }

    public void Home()
    {
        Debug.Log("[MENU] Retornando a home (carregando cena " + cenaHome + ")");
        audioManagerMenu.PlaySelectSound();
        Time.timeScale = 1f;
        SceneManager.LoadScene(cenaHome);
    }
}
