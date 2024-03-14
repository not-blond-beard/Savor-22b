namespace Savor22b.GraphTypes.Types;

public class RecipeResponse
{
    public RecipeResponse(
        int id,
        string name,
        int requiredBlock,
        List<RecipeComponent> ingredientList,
        List<RecipeComponent> foodList
    )
    {
        Id = id;
        Name = name;
        RequiredBlock = requiredBlock;
        IngredientList = ingredientList;
        FoodList = foodList;
    }

    public int Id { get; set; }

    public string Name { get; set; }

    public int RequiredBlock { get; set; }

    public List<RecipeComponent> IngredientList { get; set; }

    public List<RecipeComponent> FoodList { get; set; }
}
