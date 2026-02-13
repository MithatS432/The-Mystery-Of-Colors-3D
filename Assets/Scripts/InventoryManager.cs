using TMPro;
using UnityEngine;
using System.Linq;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [Header("UI")]
    public TMP_Text redText;
    public TMP_Text blueText;
    public TMP_Text yellowText;
    public TMP_Text greenText;
    public TMP_Text orangeText;
    public TMP_Text purpleText;

    private int redCount;
    private int blueCount;
    private int yellowCount;
    private int greenCount;
    private int orangeCount;
    private int purpleCount;

    [Header("Recipes")]
    public ColorRecipe[] recipes;

    void Awake()
    {
        Instance = this;

        // Priority sıralama
        recipes = recipes.OrderBy(r => r.priority).ToArray();

        UpdateAllUI();
    }

    // ========================
    // DIŞARIDAN GELEN TOP
    // ========================
    public void AddSphere(SphereColor color, bool reportToMission = true)
    {
        AddColor(color);

        if (reportToMission)
            MissionManager.Instance?.ReportCollect(color);

        TryResolveRecipes();
    }



    // ========================
    // RECIPE RESOLVE LOOP
    // ========================
    void TryResolveRecipes()
    {
        foreach (var recipe in recipes)
        {
            if (CanCraft(recipe))
            {
                Craft(recipe);
                return;
            }
        }
    }


    void Craft(ColorRecipe recipe)
    {
        RemoveColor(recipe.colorA);
        if (recipe.colorA == recipe.colorB)
            RemoveColor(recipe.colorA);
        else
            RemoveColor(recipe.colorB);

        if (SphereColorHelper.IsInventoryColor(recipe.resultColor))
        {
            AddColor(recipe.resultColor);
        }

    }





    // ========================
    // RECIPE UYGULANABİLİR Mİ
    // ========================
    bool CanCraft(ColorRecipe recipe)
    {
        if (recipe.colorA == recipe.colorB)
            return GetCount(recipe.colorA) >= 2;

        return GetCount(recipe.colorA) >= 1 &&
               GetCount(recipe.colorB) >= 1;
    }

    // ========================
    // ENVANTER EKLE
    // ========================
    void AddColor(SphereColor color)
    {
        switch (color)
        {
            case SphereColor.Red: redCount++; break;
            case SphereColor.Blue: blueCount++; break;
            case SphereColor.Yellow: yellowCount++; break;
            case SphereColor.Green: greenCount++; break;
            case SphereColor.Orange: orangeCount++; break;
            case SphereColor.Purple: purpleCount++; break;
        }

        UpdateUI(color);
    }



    // ========================
    // ENVANTER AZALT
    // ========================
    void RemoveColor(SphereColor color)
    {
        switch (color)
        {
            case SphereColor.Red: if (redCount > 0) redCount--; break;
            case SphereColor.Blue: if (blueCount > 0) blueCount--; break;
            case SphereColor.Yellow: if (yellowCount > 0) yellowCount--; break;
            case SphereColor.Green: if (greenCount > 0) greenCount--; break;
            case SphereColor.Orange: if (orangeCount > 0) orangeCount--; break;
            case SphereColor.Purple: if (purpleCount > 0) purpleCount--; break;
        }

        UpdateUI(color);
    }

    // ========================
    // HELPER
    // ========================
    int GetCount(SphereColor color)
    {
        switch (color)
        {
            case SphereColor.Red: return redCount;
            case SphereColor.Blue: return blueCount;
            case SphereColor.Yellow: return yellowCount;
            case SphereColor.Green: return greenCount;
            case SphereColor.Orange: return orangeCount;
            case SphereColor.Purple: return purpleCount;
        }

        return 0;
    }

    void UpdateUI(SphereColor color)
    {
        switch (color)
        {
            case SphereColor.Red: if (redText) redText.text = redCount.ToString(); break;
            case SphereColor.Blue: if (blueText) blueText.text = blueCount.ToString(); break;
            case SphereColor.Yellow: if (yellowText) yellowText.text = yellowCount.ToString(); break;
            case SphereColor.Green: if (greenText) greenText.text = greenCount.ToString(); break;
            case SphereColor.Orange: if (orangeText) orangeText.text = orangeCount.ToString(); break;
            case SphereColor.Purple: if (purpleText) purpleText.text = purpleCount.ToString(); break;
        }
    }

    void UpdateAllUI()
    {
        UpdateUI(SphereColor.Red);
        UpdateUI(SphereColor.Blue);
        UpdateUI(SphereColor.Yellow);
        UpdateUI(SphereColor.Green);
        UpdateUI(SphereColor.Orange);
        UpdateUI(SphereColor.Purple);
    }
}
