using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SphereSpawner : MonoBehaviour
{
    public SpawnPoint[] spawnPoints;
    public GameObject[] spheres;

    public float spawnDelay = 1f;
    private float startSpawnDelay = 2f;
    public AudioClip spawnSound;

    [Header("Difficulty")]
    public AnimationCurve spawnSpeedCurve;
    public float difficultyDuration = 60f;

    private float gameTime;
    public bool gameActive = true;

    void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        yield return new WaitForSeconds(startSpawnDelay);

        while (true)
        {
            if (!gameActive)
            {
                yield return null;
                continue;
            }

            List<SpawnPoint> shuffled = new List<SpawnPoint>(spawnPoints);
            ShuffleList(shuffled);

            foreach (SpawnPoint sp in shuffled)
            {
                if (!gameActive) break;

                gameTime += Time.deltaTime;

                float t = Mathf.Clamp01(gameTime / difficultyDuration);
                float multiplier = spawnSpeedCurve.Evaluate(t);
                float currentDelay = spawnDelay * multiplier;

                GameObject selectedSphere = spheres[Random.Range(0, spheres.Length)];

                GameObject obj = PoolManager.Instance.SpawnFromPool(
                    selectedSphere,
                    sp.transform.position,
                    Quaternion.identity);

                obj.GetComponent<Sphere>().Initialize(sp.spawnDirection, selectedSphere);

                if (spawnSound != null)
                    AudioSource.PlayClipAtPoint(spawnSound, Camera.main.transform.position);

                yield return new WaitForSeconds(currentDelay);
            }
        }
    }

    void ShuffleList(List<SpawnPoint> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            SpawnPoint temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}
