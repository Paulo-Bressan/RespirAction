using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    [Tooltip("Mixer de audio")]
    [SerializeField] private AudioMixer myMixer;
    [Tooltip("Objeto do slider de musica")]
    [SerializeField] private Slider musicVolSlider;
    [Tooltip("Objeto do slider de efeitos sonoros")]
    [SerializeField] private Slider soundVolSlider;

    void Start()
    {
        if (!PlayerPrefs.HasKey("musicVolume"))
            PlayerPrefs.SetFloat("musicVolume", 0f);
        if (!PlayerPrefs.HasKey("soundVolume"))
            PlayerPrefs.SetFloat("soundVolume", 0f);

        if (!myMixer)
            Debug.LogWarning("[AUDIO] Mixer faltando");
        if (!musicVolSlider || !soundVolSlider)
            Debug.LogWarning("[AUDIO] Sliders faltando");
        else
            LoadPrefs();
    }

    public void ChangeMusicVolume()
    {
        Debug.Log("[AUDIO] valor do volume de musica alterado para " + musicVolSlider.value);
        myMixer.SetFloat("MusicVol", musicVolSlider.value);
        PlayerPrefs.SetFloat("musicVolume", musicVolSlider.value);
    }

    public void ChangeSoundVolume()
    {
        Debug.Log("[AUDIO] valor do volume de som alterado para " +  soundVolSlider.value);
        myMixer.SetFloat("SoundVol", soundVolSlider.value);
        PlayerPrefs.SetFloat("soundVolume", soundVolSlider.value);
    }

    public void LoadPrefs()
    {
        musicVolSlider.value = PlayerPrefs.GetFloat("musicVolume");
        soundVolSlider.value = PlayerPrefs.GetFloat("soundVolume");
    }
}
