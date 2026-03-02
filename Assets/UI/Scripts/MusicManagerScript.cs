using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private GameObject MusicPlayer;
    private MusicPlayerScript MusicPlayerScript;
    public AudioClip track;
    void Start()
    {
        MusicPlayer = GameObject.FindGameObjectWithTag("MusicPlayer");

        if(MusicPlayer)
        {
            if(MusicPlayer.GetComponent<AudioSource>().resource != track)
            {
                MusicPlayer.GetComponent<MusicPlayerScript>().StopMusic();

                if(track != null)
                {
                    MusicPlayer.GetComponent<AudioSource>().resource = track;
                    MusicPlayer.GetComponent<MusicPlayerScript>().PlayMusic();
                }
            }      
        }
    }
}
