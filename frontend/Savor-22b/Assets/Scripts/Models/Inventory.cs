using System;

[Serializable]
public class Inventory
{
    public Seed[] seedStateList;
    public Refrigerator[] refrigeratorStateList;

    public static Inventory CreateFromJSON(string jsonString)
    {
       Inventory inventory = ResponseParser.Parse<Inventory>(jsonString, "inventory");

         return inventory;
    }
}