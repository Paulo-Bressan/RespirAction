using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    // Referência para o componente AudioSource
    private AudioSource audioSource;

    public AudioClip grabSound;
    public AudioClip releaseSound;
    public AudioClip correctSound;

    private void Awake()
    {
        // Configuração do Singleton
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }

        // Pega o componente AudioSource anexado a este GameObject
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayGrabSound()
    {
        if (grabSound != null)
        {
            audioSource.PlayOneShot(grabSound);
        }
    }

    public void PlayReleaseSound()
    {
        if (releaseSound != null)
        {
            audioSource.PlayOneShot(releaseSound);
        }
    }

    public void PlayCorrectSound()
    {
        if (correctSound != null)
        {
            audioSource.PlayOneShot(correctSound);
        }
    }
}
