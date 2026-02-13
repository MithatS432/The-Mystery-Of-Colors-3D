using UnityEngine;

[System.Serializable]
public class ColorRecipe
{
    public SphereColor colorA;
    public SphereColor colorB;
    public SphereColor resultColor;

    public int priority;


    public bool Matches(SphereColor a, SphereColor b)
    {
        return (a == colorA && b == colorB) ||
               (a == colorB && b == colorA);
    }
}
