namespace Savor22b.GraphTypes;

public class RecipeIngredient
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }

    public RecipeIngredient(int id, string name, string type)
    {
        Id = id;
        Name = name;
        Type = type;
    }
}
