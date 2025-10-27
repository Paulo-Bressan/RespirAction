using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManagerMenu : MonoBehaviour
{
    public static AudioManagerMenu instance;

    // Referência para o componente AudioSource
    private AudioSource audioSource;

    public AudioClip selectSound;

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

    // Métodos públicos para tocar cada som
    public void PlaySelectSound()
    {
        if (selectSound != null)
        {
            audioSource.PlayOneShot(selectSound);
        }
    }

}
