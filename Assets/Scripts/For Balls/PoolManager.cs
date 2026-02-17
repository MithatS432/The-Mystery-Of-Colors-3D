using UnityEngine;
using System.Collections.Generic;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance;

    [System.Serializable]
    public class Pool
    {
        public GameObject prefab;
        public int size;
    }

    public Pool[] pools;

    private Dictionary<GameObject, Queue<GameObject>> poolDictionary;

    void Awake()
    {
        Instance = this;
        poolDictionary = new Dictionary<GameObject, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.prefab, objectPool);
        }
    }

    public GameObject SpawnFromPool(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(prefab))
            return null;

        Queue<GameObject> pool = poolDictionary[prefab];
        GameObject objectToSpawn;

        if (pool.Count == 0)
        {
            objectToSpawn = Instantiate(prefab, position, rotation);
        }
        else
        {
            objectToSpawn = pool.Dequeue();
            objectToSpawn.transform.position = position;
            objectToSpawn.transform.rotation = rotation;
            objectToSpawn.SetActive(true);
        }

        AutoReturnToPool autoReturn = objectToSpawn.GetComponent<AutoReturnToPool>();
        if (autoReturn != null)
        {
            autoReturn.SetPrefab(prefab);
        }

        return objectToSpawn;
    }


    public void ReturnToPool(GameObject prefab, GameObject obj)
    {
        if (!poolDictionary.ContainsKey(prefab))
        {
            Debug.LogWarning("Pool bulunamadı: " + prefab.name);
            Destroy(obj);
            return;
        }

        obj.SetActive(false);
        poolDictionary[prefab].Enqueue(obj);
    }

}
