using UnityEngine;

public class AudioDestroyer : MonoBehaviour
{
    public GameObject audioManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioManager = GameObject.Find("MenuMusic");

        if (audioManager != null)
            Object.Destroy(audioManager);
    }
}
