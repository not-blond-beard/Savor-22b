using System;

[Serializable]
public class Recipes
{
    public Recipe[] recipeList;

    public static Recipes CreateFromJSON(string jsonString)
    {
        Recipes recipes = ResponseParser.Parse<Recipes>(jsonString, "recipe");

        return recipes;
    }
}
