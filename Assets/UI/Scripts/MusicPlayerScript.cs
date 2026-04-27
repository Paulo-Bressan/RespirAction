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
        Debug.Log("[MUSICA] Tocando musica");
        audioSource.Play();
    }

    public void PauseMusic()
    {
        Debug.Log("[MUSICA] Pausando musica");
        audioSource.Play();
    }
    public void StopMusic()
    {
        Debug.Log("[MUSICA] Parando musica");
        audioSource.Stop();
    }

    public void AdjustVolume(float volume)
    {
        Debug.Log("[MUSICA] Alterando volume para" + volume);
        audioSource.volume = volume;
    }
}
