using TMPro;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public TMP_Text redText;
    public TMP_Text blueText;
    public TMP_Text yellowText;
    public TMP_Text greenText;
    public TMP_Text orangeText;
    public TMP_Text purpleText;

    int redCount;
    int blueCount;
    int yellowCount;
    int greenCount;
    int orangeCount;
    int purpleCount;

    void Awake()
    {
        Instance = this;
    }

    public void AddSphere(SphereColor color)
    {
        switch (color)
        {
            case SphereColor.Red:
                redCount++;
                redText.text = redCount.ToString();
                break;

            case SphereColor.Blue:
                blueCount++;
                blueText.text = blueCount.ToString();
                break;

            case SphereColor.Yellow:
                yellowCount++;
                yellowText.text = yellowCount.ToString();
                break;

            case SphereColor.Green:
                greenCount++;
                greenText.text = greenCount.ToString();
                break;

            case SphereColor.Orange:
                orangeCount++;
                orangeText.text = orangeCount.ToString();
                break;

            case SphereColor.Purple:
                purpleCount++;
                purpleText.text = purpleCount.ToString();
                break;
        }
    }
}
