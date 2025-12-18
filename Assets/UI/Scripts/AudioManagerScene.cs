using UnityEngine;

public class AudioManagerScene : MonoBehaviour
{
    // Referência para o componente AudioSource
    private AudioSource audioSource;

    public AudioClip clip0;
    public AudioClip clip1;
    public AudioClip clip2;
    public AudioClip clip3;
    public AudioClip clip4;
    public AudioClip clip5;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound(int n)
    {
        if (audioSource != null)
        {
            switch (n)
            {
                case 0:
                    if (clip0) audioSource.PlayOneShot(clip0);
                    else Debug.Log("Clip 0 Missing");
                    break;
                case 1:
                    if (clip1) audioSource.PlayOneShot(clip1);
                    else Debug.Log("Clip 1 Missing");
                    break;
                case 2:
                    if (clip2) audioSource.PlayOneShot(clip2);
                    else Debug.Log("Clip 2 Missing");
                    break;
                case 3:
                    if (clip3) audioSource.PlayOneShot(clip3);
                    else Debug.Log("Clip 3 Missing");
                    break;
                case 4:
                    if (clip4) audioSource.PlayOneShot(clip4);
                    else Debug.Log("Clip 4 Missing");
                    break;
                case 5:
                    if (clip5) audioSource.PlayOneShot(clip5);
                    else Debug.Log("Clip 5 Missing");
                    break;
            }
        }
        else Debug.Log("AudioSource Missing");
        
    }
}
