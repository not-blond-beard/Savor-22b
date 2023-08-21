namespace Savor22b.Model;

using System.Collections.Immutable;

public class Recipe
{
    public int ID { get; set; }

    public string Name { get; set; }

    public ImmutableList<int> IngredientIDList { get; set; }

    public ImmutableList<int> FoodIDList { get; set; }

    public ImmutableList<int> RequiredKitchenEquipmentCategoryList { get; set; }

    public int ResultFoodID { get; set; }

    public int RequiredBlock { get; set; }
}
