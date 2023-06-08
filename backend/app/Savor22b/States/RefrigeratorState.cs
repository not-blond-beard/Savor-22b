namespace Savor22b.States;

using Bencodex.Types;
using Libplanet.Headless.Extensions;

public class RefrigeratorState : State
{
    public Guid StateID { get; set; }
    public int? IngredientID { get; set; }
    public int? RecipeID { get; set; }
    public string Grade { get; set; }
    public int HP { get; set; }
    public int DEF { get; set; }
    public int ATK { get; set; }
    public int SPD { get; set; }

    public RefrigeratorState(Guid stateID, int? ingredientID, int? recipeID, string grade, int hp, int def, int atk, int spd)
    {
        StateID = stateID;
        IngredientID = ingredientID;
        RecipeID = recipeID;
        Grade = grade;
        HP = hp;
        DEF = def;
        ATK = atk;
        SPD = spd;
    }

    public static RefrigeratorState CreateIngredient(Guid stateID, int? ingredientID, string grade, int hp, int def, int atk, int spd)
    {
        return new RefrigeratorState(
            stateID: stateID,
            ingredientID: ingredientID,
            recipeID: null,
            grade: grade,
            hp: hp,
            def: def,
            atk: atk,
            spd: spd
        );
    }

    public static RefrigeratorState CreateFood(Guid stateID, int? recipeID, string grade, int hp, int def, int atk, int spd)
    {
        return new RefrigeratorState(
            stateID: stateID,
            ingredientID: null,
            recipeID: recipeID,
            grade: grade,
            hp: hp,
            def: def,
            atk: atk,
            spd: spd
        );
    }

    public RefrigeratorState(Bencodex.Types.Dictionary encoded)
    {
        StateID = encoded[nameof(StateID)].ToGuid();
        Grade = (string)((Text)encoded[(Text)nameof(Grade)]).Value;
        HP = (int)((Integer)encoded[(Text)nameof(HP)]).Value;
        DEF = (int)((Integer)encoded[(Text)nameof(DEF)]).Value;
        ATK = (int)((Integer)encoded[(Text)nameof(ATK)]).Value;
        SPD = (int)((Integer)encoded[(Text)nameof(SPD)]).Value;

        if (encoded.ContainsKey((Text)nameof(IngredientID)))
        {
            this.IngredientID = (int)((Integer)encoded[(Text)nameof(IngredientID)]).Value;
        }

        if (encoded.ContainsKey((Text)nameof(RecipeID)))
        {
            this.RecipeID = (int)((Integer)encoded[(Text)nameof(RecipeID)]).Value;
        }
    }

    public IValue Serialize()
    {
        var pairs = new List<KeyValuePair<IKey, IValue>>();

        pairs.Add(new KeyValuePair<IKey, IValue>((Text)nameof(StateID), StateID.Serialize()));
        pairs.Add(new KeyValuePair<IKey, IValue>((Text)nameof(Grade), (Text)Grade));
        pairs.Add(new KeyValuePair<IKey, IValue>((Text)nameof(HP), (Integer)HP));
        pairs.Add(new KeyValuePair<IKey, IValue>((Text)nameof(DEF), (Integer)DEF));
        pairs.Add(new KeyValuePair<IKey, IValue>((Text)nameof(ATK), (Integer)ATK));
        pairs.Add(new KeyValuePair<IKey, IValue>((Text)nameof(SPD), (Integer)SPD));

        if (IngredientID.HasValue)
        {
            pairs.Add(new KeyValuePair<IKey, IValue>((Text)nameof(IngredientID), (Integer)IngredientID.Value));
        }

        if (RecipeID.HasValue)
        {
            pairs.Add(new KeyValuePair<IKey, IValue>((Text)nameof(RecipeID), (Integer)RecipeID.Value));
        }

        return new Dictionary(pairs);
    }
}
