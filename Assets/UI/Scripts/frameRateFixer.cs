using UnityEngine;

public class FrameRateLimiter : MonoBehaviour
{
    void Awake()
    {
        // Disable VSync so the targetFrameRate takes full control
        QualitySettings.vSyncCount = 0;

        // Lock frame rate to 60 FPS
        Application.targetFrameRate = 60;
    }
}
