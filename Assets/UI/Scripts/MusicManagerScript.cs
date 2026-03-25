using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [Tooltip("GameObject do MusicPlayer. Encontra automaticamente")]
    [SerializeField] private GameObject MusicPlayer;

    [Tooltip("Musica a ser tocada nesta fase")]
    [SerializeField] private AudioClip track;

    [Tooltip("Volume da musica nesta fase")]
    [SerializeField] private float volume = 0;

    void Start()
    {
        MusicPlayer = GameObject.FindGameObjectWithTag("MusicPlayer");

        if(MusicPlayer)
        {
            if(MusicPlayer.GetComponent<AudioSource>().resource != track)
            {
                MusicPlayer.GetComponent<MusicPlayerScript>().StopMusic();

                if(track)
                {
                    MusicPlayer.GetComponent<AudioSource>().resource = track;
                    MusicPlayer.GetComponent<MusicPlayerScript>().PlayMusic();
                }

                if(volume != 0)
                    MusicPlayer.GetComponent<MusicPlayerScript>().AdjustVolume(volume);
                else
                    MusicPlayer.GetComponent<MusicPlayerScript>().AdjustVolume(0.5f);
            }      
        }
    }
}
