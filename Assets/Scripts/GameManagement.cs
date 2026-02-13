using UnityEngine;
using UnityEngine.UI;

public class GameManagement : MonoBehaviour
{
    public static GameManagement Instance;

    [Header("UI Panels")]
    public GameObject pausePanel;
    public GameObject inventoryPanel;
    public Button exitButton;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        ResumeGame();
    }


    public void PauseButton()
    {
        pausePanel.SetActive(true);
        inventoryPanel.SetActive(false);
        Time.timeScale = 0f;
    }

    public void InventoryButton()
    {
        inventoryPanel.SetActive(true);
        pausePanel.SetActive(false);
        Time.timeScale = 0f;
    }

    public void CloseInventory()
    {
        ResumeGame();
    }

    public void ResumeGame()
    {
        pausePanel.SetActive(false);
        inventoryPanel.SetActive(false);
        Time.timeScale = 1f;
    }
    public void ExitGame()
    {
#if UNITY_EDITOR
    UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }



    public void SetGameActive(bool isActive)
    {
        SphereSpawner[] spawners = GameObject.FindObjectsByType<SphereSpawner>(FindObjectsSortMode.None);
        foreach (var spawner in spawners)
        {
            spawner.gameActive = isActive;
        }

        Sphere[] spheres = GameObject.FindObjectsByType<Sphere>(FindObjectsSortMode.None);
        foreach (var sphere in spheres)
        {
            sphere.enabled = isActive;
            if (!isActive)
            {
                Rigidbody rb = sphere.GetComponent<Rigidbody>();
                if (rb != null) rb.linearVelocity = Vector3.zero;
            }
        }
    }

}
