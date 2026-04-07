using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManagerMenu : MonoBehaviour
{
    public static AudioManagerMenu instance;

    // Referência para o componente AudioSource
    private AudioSource audioSource;

    [SerializeField] private AudioClip selectSound;
    [SerializeField] private AudioClip effectSound;

    private void Awake()
    {
        // Configuração do Singleton
        if (instance != null)
            Destroy(gameObject);
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);

            // Pega o componente AudioSource anexado a este GameObject
            audioSource = GetComponent<AudioSource>();
        }
    }

    // Métodos públicos para tocar cada som
    public void PlaySelectSound()
    {
        if (selectSound)
            audioSource.PlayOneShot(selectSound);
        else Debug.LogWarning("[AUDIO] SelectSound faltando");
    }

    public void PlayEffectSound()
    {
        if (effectSound)
            audioSource.PlayOneShot(effectSound);
        else Debug.LogWarning("[AUDIO] EffectSound faltando");
    }
}
