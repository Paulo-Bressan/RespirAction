using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicPlayerScript : MonoBehaviour
{
    public static MusicPlayerScript instance;

    private AudioSource audioSource;

    void Awake()
    {
        // Configuração do Singleton
        if (instance != null)
            Destroy(gameObject);
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        audioSource = GetComponent<AudioSource>();
    }

    public void PlayMusic()
    {
        Debug.Log("Tocando musica");
        audioSource.Play();
    }

    public void PauseMusic()
    {
        Debug.Log("Pausando musica");
        audioSource.Play();
    }
    public void StopMusic()
    {
        Debug.Log("Parando musica");
        audioSource.Stop();
    }
}
