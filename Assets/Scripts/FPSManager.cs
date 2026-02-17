using UnityEngine;

public class FPSManager : MonoBehaviour
{
    [Header("FPS Settings")]
    public int targetFPS = 60;

    [Header("Graphics Settings")]
    [Range(0, 4)]
    public int antiAliasing = 0;
    public float shadowDistance = 15f;
    public bool useSoftShadows = false;

    void Awake()
    {
        Application.targetFrameRate = targetFPS;
        QualitySettings.vSyncCount = 0;

        QualitySettings.shadowDistance = shadowDistance;
        QualitySettings.antiAliasing = antiAliasing;
        QualitySettings.shadows = useSoftShadows ? ShadowQuality.All : ShadowQuality.HardOnly;

        QualitySettings.realtimeReflectionProbes = false;
        QualitySettings.billboardsFaceCameraPosition = true;

        Time.fixedDeltaTime = 0.02f;

        GameObject[] allObjects = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        foreach (var go in allObjects)
        {
            if (go.isStatic)
                StaticBatchingUtility.Combine(go);
        }

        Debug.Log("Mobile FPSManager initialized with target FPS: " + targetFPS);
    }
}
