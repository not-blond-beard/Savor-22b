using System;
using Newtonsoft.Json;

[Serializable]
public class Recipe
{
    public int id { get; set; }
    public string name { get; set; }
    public IngredientType[] ingredients { get; set; }
    public string minGrade { get; set; }
    public string maxGrade { get; set; }
}
