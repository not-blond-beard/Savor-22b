namespace Savor22b.GraphTypes.Types;

public class RecipeComponent
{
    public int Id { get; set; }
    public string Name { get; set; }

    public RecipeComponent(int id, string name)
    {
        Id = id;
        Name = name;
    }
}
