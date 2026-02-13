using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public enum MissionType
{
    CollectOnly,
    FusionFree
}

[System.Serializable]
public class Mission
{
    public SphereColor targetColor;
    public int amount;
    public MissionType missionType;
}

public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance;

    public TMP_Text missionText;
    public Mission[] missions;

    private int currentMissionIndex;
    private int progress;

    [Header("Audio")]
    public AudioClip wrongCollectSound;
    public AudioClip lostSound;
    public AudioClip winSound;

    private bool gameOver = false;

    [Header("Life")]
    public int maxLives = 3;
    private int currentLives;
    public Image[] hearts;
    public Sprite heartSprite;
    public Sprite skullSprite;

    void Awake()
    {
        Instance = this;
        currentLives = maxLives;
        UpdateLifeUI();
        UpdateUI();
    }

    void UpdateUI()
    {
        if (currentMissionIndex >= missions.Length)
        {
            missionText.text = "ALL MISSIONS COMPLETE";
            return;
        }

        if (missions == null || missions.Length == 0)
        {
            missionText.text = "NO MISSIONS";
            return;
        }

        var mission = missions[currentMissionIndex];
        missionText.text = progress + " / " + mission.amount + " " + mission.targetColor;
    }

    void LoseLife()
    {
        if (gameOver) return;

        currentLives--;
        PlaySound(wrongCollectSound);
        ScoreManager.Instance?.AddScore(-50);
        UpdateLifeUI();

        if (currentLives <= 0)
        {
            HandleGameOver(false);
        }
    }

    public void ReportCollect(SphereColor color)
    {
        if (gameOver) return;

        if (currentMissionIndex >= missions.Length)
        {
            HandleGameOver(true);
            return;
        }

        var mission = missions[currentMissionIndex];

        if (mission.missionType == MissionType.CollectOnly && color != mission.targetColor)
        {
            LoseLife();
            return;
        }

        if (color == mission.targetColor)
        {
            progress++;

            if (progress >= mission.amount)
            {
                currentMissionIndex++;
                progress = 0;

                currentLives = maxLives;
                UpdateLifeUI();

                IncreaseDifficultyAfterMission();

                if (currentMissionIndex >= missions.Length)
                {
                    HandleGameOver(true);
                }
            }

            UpdateUI();
        }
    }
    void IncreaseDifficultyAfterMission()
    {
        SphereSpawner[] spawners = GameObject.FindObjectsByType<SphereSpawner>(FindObjectsSortMode.None);
        foreach (var spawner in spawners)
        {
            spawner.spawnDelay *= 0.55f;
        }
    }

    void UpdateLifeUI()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].sprite = (i < currentLives) ? heartSprite : skullSprite;
        }
    }

    void PlaySound(AudioClip clip)
    {
        if (clip != null)
            AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position);
    }

    void HandleGameOver(bool win)
    {
        gameOver = true;

        GameManagement.Instance.SetGameActive(false);

        PlaySound(win ? winSound : lostSound);

        StartCoroutine(RestartGameRoutine());
    }

    IEnumerator RestartGameRoutine()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
