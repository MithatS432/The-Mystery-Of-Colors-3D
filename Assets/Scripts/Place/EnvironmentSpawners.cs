using UnityEngine;

public class EnvironmentSpawners : MonoBehaviour
{
    public GameObject butterflyPrefab;
    public GameObject beePrefab;

    public Transform butterflySpawnPoint;
    public Transform beeSpawnPoint;

    public AudioManager audioManager;

    [SerializeField] private float delay = 10f;

    [SerializeField] private float repeatRate = 30f;

    void Start()
    {
        InvokeRepeating(nameof(SpawnButterfly), delay + 1f, repeatRate);
        InvokeRepeating(nameof(SpawnBee), delay, repeatRate);
    }

    void SpawnButterfly()
    {
        Quaternion rot = Quaternion.Euler(0, 90f, 0);

        Instantiate(
            butterflyPrefab,
            butterflySpawnPoint.position,
            rot
        );
    }



    void SpawnBee()
    {
        Instantiate(beePrefab, beeSpawnPoint.position, Quaternion.identity);

        if (audioManager != null)
            audioManager.PlayBeeSound();
    }
}
