using UnityEngine;

public class SphereVFXManager : MonoBehaviour
{
    public static SphereVFXManager Instance;

    [System.Serializable]
    public class ColorVFX
    {
        public SphereColor color;
        public GameObject vfxPrefab;
    }

    public ColorVFX[] vfxList;

    void Awake()
    {
        Instance = this;
    }

    public void PlayVFX(SphereColor color, Vector3 position)
    {
        foreach (var vfx in vfxList)
        {
            if (vfx.color == color)
            {
                PoolManager.Instance.SpawnFromPool(
                    vfx.vfxPrefab,
                    position,
                    Quaternion.identity);

                return;
            }
        }
    }
}
