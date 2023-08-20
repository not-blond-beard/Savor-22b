namespace Savor22b.GraphTypes.Types;

public class RecipeResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<RecipeComponent> IngredientList { get; set; }
    public List<RecipeComponent> FoodList { get; set; }

    public RecipeResponse(
        int id,
        string name,
        List<RecipeComponent> ingredientList,
        List<RecipeComponent> foodList
    )
    {
        Id = id;
        Name = name;
        IngredientList = ingredientList;
        FoodList = foodList;
    }
}
