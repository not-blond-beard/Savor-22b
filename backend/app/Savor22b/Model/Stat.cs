namespace Savor22b.Model;

public class Stat
{
    public int ID { get; set; }
    public int IngredientID { get; set; }
    public int RecipeID { get; set; }
    public string Grade { get; set; }
    public int MinHP { get; set; }
    public int MaxHP { get; set; }
    public int MinDef { get; set; }
    public int MaxDef { get; set; }
}
