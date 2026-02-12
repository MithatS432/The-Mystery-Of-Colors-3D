using UnityEngine;
using UnityEngine.EventSystems;

public class Sphere : MonoBehaviour
{
    [Header("Sphere Settings")]
    public SphereColor sphereColor;
    public int scoreValue = 10;

    [Header("Movement")]
    public float minSpeed = 2f;
    public float maxSpeed = 5f;

    private Rigidbody rb;
    private GameObject originalPrefab;
    private Camera mainCamera;

    [Header("Audio")]
    public AudioClip collectSound;
    public AudioClip scoreSound;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        mainCamera = Camera.main;
    }

    public void Initialize(Vector3 direction, GameObject prefab)
    {
        originalPrefab = prefab;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        float speed = Random.Range(minSpeed, maxSpeed);
        rb.linearVelocity = direction.normalized * speed;
    }

    void Update()
    {
        HandleInput();
        CheckOutOfBounds();
    }


    void HandleInput()
    {
        if (IsPointerOverUI()) return;

        // MOBİL
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.GetTouch(0).position);
            TryHit(ray);
        }

        // PC
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            TryHit(ray);
        }
    }

    bool IsPointerOverUI()
    {
        if (EventSystem.current == null) return false;

        if (Input.touchCount > 0)
            return EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);

        return EventSystem.current.IsPointerOverGameObject();
    }

    void TryHit(Ray ray)
    {
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.transform == transform)
            {
                Collect();
            }
        }
    }


    void Collect()
    {
        SphereVFXManager.Instance.PlayVFX(sphereColor, transform.position);

        PlaySound(collectSound);
        PlaySound(scoreSound);

        InventoryManager.Instance.AddSphere(sphereColor);
        ScoreManager.Instance.AddScore(scoreValue);
        PoolManager.Instance.ReturnToPool(originalPrefab, gameObject);
    }

    void PlaySound(AudioClip clip)
    {
        if (clip != null)
            AudioSource.PlayClipAtPoint(clip, transform.position);
    }


    void CheckOutOfBounds()
    {
        if (transform.position.y <= -5f)
        {
            PoolManager.Instance.ReturnToPool(originalPrefab, gameObject);
        }
    }
}
