using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private static MusicManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // não destrói ao trocar de cena
        }
        else
        {
            Destroy(gameObject); // evita duplicatas
        }
    }
}
