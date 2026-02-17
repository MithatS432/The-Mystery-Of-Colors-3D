using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public float amplitude = 0.12f;
    public float frequency = 8f;
    public float smoothReturnSpeed = 5f;

    private Vector3 originalPosition;
    private float noiseSeed;

    void Start()
    {
        originalPosition = transform.localPosition;
        noiseSeed = Random.Range(0f, 100f);
    }

    void Update()
    {
        if (MissionManager.Instance.IsChaosActive)
        {
            float noiseX = Mathf.PerlinNoise(Time.time * frequency, noiseSeed) - 0.5f;
            float noiseY = Mathf.PerlinNoise(noiseSeed, Time.time * frequency) - 0.5f;

            Vector3 offset = new Vector3(noiseX, noiseY, 0f) * amplitude;
            transform.localPosition = originalPosition + offset;
        }
        else
        {
            transform.localPosition = Vector3.Lerp(
                transform.localPosition,
                originalPosition,
                smoothReturnSpeed * Time.deltaTime
            );
        }
    }
}
