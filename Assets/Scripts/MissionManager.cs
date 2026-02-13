using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

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
    public Button[] potions;

    [Header("Potion / Cooldown")]
    public AudioClip potionSound;
    public TMP_Text timeCountText;
    private bool potionCooldown = false;
    private float potionCooldownTime = 5f;
    public GameObject potionVFXPrefab;
    private List<Button> usedPotions = new List<Button>();


    void Awake()
    {
        Instance = this;
        currentLives = maxLives;
        UpdateLifeUI();
        UpdateUI();
        InitializePotions();
    }
    void InitializePotions()
    {
        foreach (var btn in potions)
        {
            btn.interactable = false;

            var image = btn.GetComponent<Image>();
            if (image != null)
            {
                Color c = image.color;
                c.a = 0.5f;
                image.color = c;
            }
        }
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

    public void ReportCollect(SphereColor color, bool isFusionResult = false, bool isRequiredForFusion = false)
    {
        if (gameOver) return;

        if (currentMissionIndex >= missions.Length)
        {
            HandleGameOver(true);
            return;
        }

        var mission = missions[currentMissionIndex];

        // CollectOnly görev
        if (mission.missionType == MissionType.CollectOnly)
        {
            if (color != mission.targetColor)
            {
                LoseLife();
                return;
            }

            progress++;
        }

        // FusionFree görev
        else if (mission.missionType == MissionType.FusionFree)
        {
            if (isFusionResult)
            {
                if (color == mission.targetColor)
                    progress++;
            }
            else if (isRequiredForFusion)
            {
                return;
            }
            else
            {
                LoseLife();
            }
        }


        // Görev tamamlanınca
        if (progress >= mission.amount)
        {
            currentMissionIndex++;
            progress = 0;
            currentLives = maxLives;
            UpdateLifeUI();

            IncreaseDifficultyAfterMission();
            ActivateRandomPotion();

            if (currentMissionIndex >= missions.Length)
            {
                HandleGameOver(true);
                return;
            }
        }

        UpdateUI();
    }





    void IncreaseDifficultyAfterMission()
    {
        SphereSpawner[] spawners = GameObject.FindObjectsByType<SphereSpawner>(FindObjectsSortMode.None);
        foreach (var spawner in spawners)
        {
            spawner.spawnDelay *= 0.55f;
        }
    }
    void ActivateRandomPotion()
    {
        List<Button> closedPotions = new List<Button>();
        foreach (var btn in potions)
        {
            if (!btn.interactable && !usedPotions.Contains(btn))
                closedPotions.Add(btn);
        }

        if (closedPotions.Count == 0) return;

        int index = Random.Range(0, closedPotions.Count);
        Button selected = closedPotions[index];

        selected.interactable = true;
        var image = selected.GetComponent<Image>();
        if (image != null)
        {
            Color c = image.color;
            c.a = 1f;
            image.color = c;
        }
    }

    public void UsePotion(Button btn)
    {
        if (!btn.interactable || potionCooldown) return;

        if (potionSound != null)
            AudioSource.PlayClipAtPoint(potionSound, Camera.main.transform.position);

        btn.interactable = false;
        var image = btn.GetComponent<Image>();
        if (image != null)
        {
            Color c = image.color;
            c.a = 0.5f;
            image.color = c;
        }

        if (!usedPotions.Contains(btn))
            usedPotions.Add(btn);


        if (potionVFXPrefab != null)
        {
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(btn.transform.position);
            worldPos.z = Camera.main.nearClipPlane + 0.5f;

            GameObject vfx = Instantiate(potionVFXPrefab, worldPos, Quaternion.identity);

            ParticleSystem ps = vfx.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                var main = ps.main;
                main.stopAction = ParticleSystemStopAction.Destroy;
                ps.Play();
            }
            else
            {
                Destroy(vfx, 2f);
            }
        }

        StartCoroutine(PotionCooldownRoutine());
    }



    IEnumerator PotionCooldownRoutine()
    {
        potionCooldown = true;
        float timer = potionCooldownTime;

        if (timeCountText != null)
            timeCountText.gameObject.SetActive(true);

        while (timer > 0f)
        {
            if (gameOver)
                yield break;

            if (timeCountText != null)
                timeCountText.text = Mathf.Ceil(timer).ToString();

            timer -= Time.deltaTime;
            yield return null;
        }

        if (timeCountText != null)
            timeCountText.gameObject.SetActive(false);

        potionCooldown = false;
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
