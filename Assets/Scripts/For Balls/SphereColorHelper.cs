public static class SphereColorHelper
{
    public static bool IsInventoryColor(SphereColor color)
    {
        return color == SphereColor.Red ||
               color == SphereColor.Blue ||
               color == SphereColor.Yellow ||
               color == SphereColor.Green ||
               color == SphereColor.Orange ||
               color == SphereColor.Purple;
    }
}
