namespace Savor22b.GraphTypes;

public class Recipe
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<RecipeIngredient> Ingredients { get; set; }
    public string MinGrade { get; set; }
    public string MaxGrade { get; set; }

    public Recipe(int id, string name, List<RecipeIngredient> ingredients, string minGrade, string maxGrade)
    {
        Id = id;
        Name = name;
        Ingredients = ingredients;
        MinGrade = minGrade;
        MaxGrade = maxGrade;
    }
}
