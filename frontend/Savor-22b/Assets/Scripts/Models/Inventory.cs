using UnityEngine;

[System.Serializable]
public class Inventory
{
    public Seed[] seedStateList;
    public Refrigerator[] refrigeratorStateList;

    public static Inventory CreateFromJSON(string jsonString)
    {
        return ResponseParser.Parse<Inventory>(jsonString, "inventory");
    }
}