namespace Savor22b.Model;

public class Ingredient
{
    public int Id { get; set; }
    public string Name { get; set; }
    // TODO should be Grade enum
    public string MinGrade { get; set; }
    // TODO should be Grade enum
    public string MaxGrade { get; set; }
    public int SeedId { get; set; }
}
