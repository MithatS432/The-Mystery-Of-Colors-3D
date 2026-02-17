using UnityEngine;
using UnityEngine.Rendering;

public class VolumeSafeAccess : MonoBehaviour
{
    private Volume[] allVolumes;

    void Awake()
    {
        allVolumes = GameObject.FindObjectsByType<Volume>(FindObjectsSortMode.None);

        foreach (var vol in allVolumes)
        {
            if (vol != null)
            {
                vol.enabled = true;
            }
        }
    }

    void Update()
    {
        foreach (var vol in allVolumes)
        {
            if (vol != null)
            {
                vol.weight = Mathf.PingPong(Time.time, 1f);
            }
        }
    }
}
