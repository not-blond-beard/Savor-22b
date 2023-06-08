namespace Savor22b.Model;

public class RecipeReference
{
    public int ID { get; set; }
    public string Name { get; set; }
    public int? IngredientID { get; set; }
    public string? IngredientName { get; set; }
    public int? ReferencedRecipeID { get; set; }
    public string? ReferencedRecipeName { get; set; }
}
