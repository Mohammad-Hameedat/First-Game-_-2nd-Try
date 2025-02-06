using UnityEngine;

public class TargetFramesScript : MonoBehaviour
{
    private void Awake()
    {
        // Initial frame rate setup
        SetFrameRate(120);
    }

    private void Start()
    {
        // Ensure VSync is disabled
        QualitySettings.vSyncCount = 0;
    }

    private void Update()
    {
        // Continuously check and reapply frame rate if needed
        if (Application.targetFrameRate != 120)
        {
            SetFrameRate(120);
        }
    }

    private void SetFrameRate(int targetFPS)
    {
        Application.targetFrameRate = targetFPS;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }
}
