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
    public AudioClip chaosSound;


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

    [Header("Potion States")]
    private bool balancePotionActive = false;
    private bool magnetPotionActive = false;

    public bool IsMagnetActive => magnetPotionActive;
    public int CurrentMissionIndex => currentMissionIndex;
    private bool filterPotionActive = false;
    public bool IsFilterActive => filterPotionActive;

    [Header("Colour Chaos")]
    private int totalPotionsUsed = 0;
    private bool chaosActive = false;

    public bool IsChaosActive => chaosActive;
    public GameObject chaosButton;
    public float chaosSpawnMultiplier = 0.2f;

    [Header("Spawner")]
    public SphereSpawner spawner;

    void Awake()
    {
        if (spawner != null)
        {
            spawner.SetSpawnerActive(true);
            spawner.SetChaosActive(false);
        }

        Instance = this;
        currentLives = maxLives;
        UpdateLifeUI();
        UpdateUI();
        InitializePotions();

        spawner.SetSpawnerActive(true);
        spawner.SetChaosActive(false);
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

        if (balancePotionActive)
            return;

        currentLives--;
        PlaySound(wrongCollectSound);
        ScoreManager.Instance?.AddScore(-50);
        UpdateLifeUI();

        if (currentLives <= 0)
        {
            HandleGameOver(false);
        }
    }

    public void ReportCollect(SphereColor color, bool fromFusion)
    {
        if (gameOver) return;

        if (currentMissionIndex >= missions.Length)
            return;

        var mission = missions[currentMissionIndex];

        if (color == mission.targetColor)
        {
            if (mission.missionType == MissionType.CollectOnly)
            {
                if (!fromFusion)
                    progress++;
            }
            else
            {
                progress++;
            }
        }
        else
        {
            if (fromFusion && mission.missionType == MissionType.CollectOnly)
                return;

            bool isRelevant = InventoryManager.Instance
                .IsColorRelevantRecursive(mission.targetColor, color);

            if (!isRelevant)
                LoseLife();
        }

        if (progress >= mission.amount)
            CompleteMission();

        UpdateUI();
    }




    void CompleteMission()
    {
        if (chaosActive)
        {
            DeactivateChaosMode();
        }

        currentMissionIndex++;
        progress = 0;
        currentLives = maxLives;
        UpdateLifeUI();

        IncreaseDifficultyAfterMission();
        ActivateRandomPotion();

        if (currentMissionIndex >= missions.Length)
        {
            HandleGameOver(true);
        }
    }

    private void DeactivateChaosMode()
    {
        chaosActive = false;
        totalPotionsUsed = 0;

        spawner.SetChaosActive(false);
        spawner.ResetMultipliers();

        if (chaosButton != null)
            chaosButton.SetActive(false);
    }


    void IncreaseDifficultyAfterMission()
    {
        float newMultiplier = 0.85f;
        spawner.SetDifficultyMultiplier(newMultiplier);
    }


    void ActivateRandomPotion()
    {
        List<Button> closedPotions = new List<Button>();
        foreach (var btn in potions)
        {
            if (!btn.interactable)
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
        if (!GameManagement.Instance.IsGameActive)
            return;

        if (!btn.interactable || potionCooldown) return;

        if (btn == potions[0])
        {
            ActivateBalancePotion();
        }
        if (btn == potions[1])
        {
            ActivateMagnetPotion();
        }
        if (btn == potions[2])
        {
            ActivateFilterPotion();
        }

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

        totalPotionsUsed++;

        if (totalPotionsUsed >= 3 && !chaosActive)
        {
            ActivateChaosMode();
        }

    }

    private void ActivateChaosMode()
    {
        chaosActive = true;

        spawner.SetChaosActive(true);
        spawner.SetChaosMultiplier(chaosSpawnMultiplier);

        if (chaosButton != null)
            chaosButton.SetActive(true);

        if (chaosSound != null)
            AudioSource.PlayClipAtPoint(
                chaosSound,
                Camera.main.transform.position
            );
    }
    public void OnChaosAnimationFinished()
    {
        if (chaosButton != null)
            chaosButton.SetActive(false);
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


    public void ActivateBalancePotion()
    {
        if (balancePotionActive) return;

        StartCoroutine(BalancePotionRoutine());
    }
    IEnumerator BalancePotionRoutine()
    {
        balancePotionActive = true;

        yield return new WaitForSeconds(potionCooldownTime);

        balancePotionActive = false;
    }


    public void ActivateMagnetPotion()
    {
        if (magnetPotionActive) return;

        StartCoroutine(MagnetPotionRoutine());
    }

    IEnumerator MagnetPotionRoutine()
    {
        magnetPotionActive = true;

        yield return new WaitForSeconds(potionCooldownTime);

        magnetPotionActive = false;
    }


    public void ActivateFilterPotion()
    {
        if (filterPotionActive) return;

        StartCoroutine(FilterPotionRoutine());
    }

    IEnumerator FilterPotionRoutine()
    {
        filterPotionActive = true;

        yield return new WaitForSeconds(potionCooldownTime);

        filterPotionActive = false;
    }

}
