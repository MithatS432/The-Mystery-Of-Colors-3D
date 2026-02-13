using UnityEngine;
using System.Collections;

public class AutoReturnToPool : MonoBehaviour
{
    private GameObject prefabReference;
    private ParticleSystem ps;
    private bool initialized = false;

    void Awake()
    {
        ps = GetComponent<ParticleSystem>();
    }

    public void SetPrefab(GameObject prefab)
    {
        prefabReference = prefab;
        initialized = true;
    }

    void OnEnable()
    {
        if (!initialized) return;

        ps.Clear();
        ps.Play();
        StartCoroutine(ReturnRoutine());
    }

    IEnumerator ReturnRoutine()
    {
        yield return new WaitUntil(() => !ps.IsAlive(true));

        PoolManager.Instance.ReturnToPool(prefabReference, gameObject);
    }
}
