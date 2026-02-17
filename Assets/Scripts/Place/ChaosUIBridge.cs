using UnityEngine;

public class ChaosUIBridge : MonoBehaviour
{
    public void OnChaosAnimationFinished()
    {
        MissionManager.Instance.OnChaosAnimationFinished();
    }
}
