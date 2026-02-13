using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public TMP_Text scoreText;

    private int score;

    void Awake()
    {
        Instance = this;
        UpdateUI();
    }

    public void AddScore(int amount)
    {
        score += amount;

        if (score < 0)
            score = 0;

        UpdateUI();
    }


    void UpdateUI()
    {
        scoreText.text = score.ToString();
    }
}
