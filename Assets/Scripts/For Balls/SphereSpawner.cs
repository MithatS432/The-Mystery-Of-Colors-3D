using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SphereSpawner : MonoBehaviour
{
    [Header("Spawn Points")]
    public SpawnPoint[] normalSpawnPoints;
    public SpawnPoint[] chaosSpawnPoints;

    [Header("Spheres")]
    public GameObject[] spheres;

    [Header("Timing")]
    public float baseSpawnDelay = 1f;
    private float startSpawnDelay = 1f;

    [Header("Difficulty")]
    public AnimationCurve spawnSpeedCurve;
    public float difficultyDuration = 60f;

    public AudioClip spawnSound;

    private float gameTime;
    private bool gameActive = false;
    private bool chaosActive = false;

    private float difficultyMultiplier = 1f;
    private float chaosMultiplier = 1f;

    void Start()
    {
        gameActive = true;
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (!gameActive)
            yield return null;

        yield return new WaitForSeconds(startSpawnDelay);

        while (true)
        {
            if (!gameActive)
            {
                yield return null;
                continue;
            }

            gameTime += Time.deltaTime;

            float curveT = Mathf.Clamp01(gameTime / difficultyDuration);
            difficultyMultiplier = spawnSpeedCurve.Evaluate(curveT);

            float currentDelay =
                baseSpawnDelay *
                difficultyMultiplier *
                chaosMultiplier;

            SpawnPoint[] activePoints = chaosActive
                ? GetCombinedSpawnPoints()
                : normalSpawnPoints;

            List<SpawnPoint> shuffled = new List<SpawnPoint>(activePoints);
            ShuffleList(shuffled);

            foreach (SpawnPoint sp in shuffled)
            {
                if (!gameActive) break;

                GameObject selectedSphere = GetSpawnableSphere();

                GameObject obj = PoolManager.Instance.SpawnFromPool(
                    selectedSphere,
                    sp.transform.position,
                    Quaternion.identity
                );

                obj.GetComponent<Sphere>()
                    .Initialize(sp.spawnDirection, selectedSphere);

                if (spawnSound != null)
                    AudioSource.PlayClipAtPoint(
                        spawnSound,
                        Camera.main.transform.position
                    );

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

    SpawnPoint[] GetCombinedSpawnPoints()
    {
        SpawnPoint[] combined =
            new SpawnPoint[normalSpawnPoints.Length + chaosSpawnPoints.Length];

        normalSpawnPoints.CopyTo(combined, 0);
        chaosSpawnPoints.CopyTo(combined, normalSpawnPoints.Length);

        return combined;
    }

    GameObject GetSpawnableSphere()
    {
        if (!MissionManager.Instance.IsFilterActive)
            return spheres[Random.Range(0, spheres.Length)];

        var mission =
            MissionManager.Instance.missions[
                MissionManager.Instance.CurrentMissionIndex
            ];

        List<GameObject> valid = new List<GameObject>();

        foreach (var prefab in spheres)
        {
            Sphere sphereComponent = prefab.GetComponent<Sphere>();

            bool isRelevant =
                InventoryManager.Instance
                .IsColorRelevantRecursive(
                    mission.targetColor,
                    sphereComponent.sphereColor
                );

            if (isRelevant)
                valid.Add(prefab);
        }

        if (valid.Count == 0)
            return spheres[Random.Range(0, spheres.Length)];

        return valid[Random.Range(0, valid.Count)];
    }


    public void SetSpawnerActive(bool active)
    {
        gameActive = active;
    }

    public void SetChaosActive(bool active)
    {
        chaosActive = active;
    }

    public void SetChaosMultiplier(float multiplier)
    {
        chaosMultiplier = multiplier;
    }

    public void SetDifficultyMultiplier(float multiplier)
    {
        difficultyMultiplier = multiplier;
    }

    public void ResetMultipliers()
    {
        difficultyMultiplier = 1f;
        chaosMultiplier = 1f;
        gameTime = 0f;
    }
}
