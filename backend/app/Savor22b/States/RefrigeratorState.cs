namespace Savor22b.States;

using Bencodex.Types;

public class RefrigeratorState : State
{
    public int Id { get; set; }
    public int? IngredientId { get; set; }
    public int? RecipeId { get; set; }
    public string Grade { get; set; }
    public int HP { get; set; }
    public int DEF { get; set; }
    public int ATK { get; set; }
    public int SPD { get; set; }

    public RefrigeratorState(int id, int? ingredientId, int? recipeId, string grade, int hp, int def, int atk, int spd)
    {
        this.Id = id;
        this.IngredientId = ingredientId;
        this.RecipeId = recipeId;
        this.Grade = grade;
        this.HP = hp;
        this.DEF = def;
        this.ATK = atk;
        this.SPD = spd;
    }

    public RefrigeratorState(Bencodex.Types.Dictionary encoded)
    {
        this.Id = (int)((Integer)encoded[(Text)"id"]).Value;
        this.Grade = (string)((Text)encoded[(Text)"grade"]).Value;
        this.HP = (int)((Integer)encoded[(Text)"hp"]).Value;
        this.DEF = (int)((Integer)encoded[(Text)"def"]).Value;
        this.ATK = (int)((Integer)encoded[(Text)"atk"]).Value;
        this.SPD = (int)((Integer)encoded[(Text)"spd"]).Value;

        if (encoded.ContainsKey((Text)"ingredientId"))
        {
            this.IngredientId = (int)((Integer)encoded[(Text)"ingredientId"]).Value;
        }

        if (encoded.ContainsKey((Text)"recipeId"))
        {
            this.RecipeId = (int)((Integer)encoded[(Text)"recipeId"]).Value;
        }
    }

    public IValue Serialize()
    {
        var pairs = new List<KeyValuePair<IKey, IValue>>();

        pairs.Add(new KeyValuePair<IKey, IValue>((Text)"id", (Integer)this.Id));
        pairs.Add(new KeyValuePair<IKey, IValue>((Text)"grade", (Text)this.Grade));
        pairs.Add(new KeyValuePair<IKey, IValue>((Text)"hp", (Integer)this.HP));
        pairs.Add(new KeyValuePair<IKey, IValue>((Text)"def", (Integer)this.DEF));
        pairs.Add(new KeyValuePair<IKey, IValue>((Text)"atk", (Integer)this.ATK));
        pairs.Add(new KeyValuePair<IKey, IValue>((Text)"spd", (Integer)this.SPD));

        if (this.IngredientId.HasValue)
        {
            pairs.Add(new KeyValuePair<IKey, IValue>((Text)"ingredientId", (Integer)this.IngredientId.Value));
        }

        if (this.RecipeId.HasValue)
        {
            pairs.Add(new KeyValuePair<IKey, IValue>((Text)"recipeId", (Integer)this.RecipeId.Value));
        }

        return new Dictionary(pairs);
    }
}
