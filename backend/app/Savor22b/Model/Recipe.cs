namespace Savor22b.Model;

public class Recipe
{
    public int ID { get; set; }

    public string Name { get; set; }

    public List<int> IngredientIDList { get; set; }

    public List<int> FoodIDList { get; set; }

    public List<int> RequiredKitchenEquipmentCategoryList { get; set; }

    public int ResultFoodID { get; set; }

    public int RequiredBlock { get; set; }
}
