using System;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager instance;

    public float elapsedTime;                       // tempo em segundos (float)
    public float timeSineWave;                      // tempo na funcao seno
    public float sineSpeedDiv = (float)Math.PI/2;   // velocidade do seno (menor num = mais veloz)

    public event Action<float> OnMinutePassed;      // evento para cada minuto

    private float minuteCheck;

    private void Awake()
    {
        // Configuração do Singleton
        if (instance != null)
            Destroy(gameObject);
        else
        {
            instance = this;
            //DontDestroyOnLoad(this.gameObject);
        }

        elapsedTime = 0f;
        minuteCheck = 60f;
    }

    // Update is called once per frame
    void Update()
    {
        // Update the elapsed time every frame, independent of CPU performance.
        elapsedTime += Time.deltaTime;
        timeSineWave = (float)Math.Sin((elapsedTime / sineSpeedDiv));

        if (elapsedTime >= minuteCheck)
        {
            OnMinutePassed?.Invoke(minuteCheck);
            minuteCheck += 60f; // Set next minute threshold
        }
    }
}
