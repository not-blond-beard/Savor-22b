using System;
using System.Collections.Generic;
using Newtonsoft.Json;

[Serializable]
public class Recipe
{
    public int id { get; set; }
    public string name { get; set; }
    public IngredientType[] ingredients { get; set; }
    public string minGrade { get; set; }
    public string maxGrade { get; set; }

    public static List<Recipe> CreateFromJSON(string jsonString)
    {
        List<Recipe> recipes = ResponseParser.Parse<List<Recipe>>(jsonString, "recipe");

        return recipes;
    }
}
