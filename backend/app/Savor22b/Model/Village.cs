namespace Savor22b.Model;

public class Village
{
    public static readonly int DistancePriceUnit = 1000;
    public static readonly int DistanceBlockUnit = 10;

    public int Id { get; set; }

    public string Name { get; set; }

    public int Width { get; set; }

    public int Height { get; set; }
    public int WorldX { get; set; }
    public int WorldY { get; set; }

    public bool AbleToPlaceHouse(int targetX, int targetY)
    {
        int halfWidth = (Width - 1) / 2;
        int halfHeight = (Height - 1) / 2;

        if (Math.Abs(targetX) > halfWidth)
        {
            return false;
        }

        if (Math.Abs(targetY) > halfHeight)
        {
            return false;
        }

        return true;
    }
}
