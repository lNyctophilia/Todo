using UnityEngine;

public class FpsBooster : MonoBehaviour
{
    private void Start()
    {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
        QualitySettings.SetQualityLevel(0);
    }
}
