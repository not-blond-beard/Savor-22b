namespace Savor22b.States;

using Bencodex.Types;
using Libplanet.Headless.Extensions;

public class RefrigeratorState : State
{
    public RefrigeratorState(Guid stateID, int? ingredientID, int? foodID, string grade, int hp, int def, int atk, int spd)
    {
        StateID = stateID;
        IngredientID = ingredientID;
        FoodID = foodID;
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
            foodID: null,
            grade: grade,
            hp: hp,
            def: def,
            atk: atk,
            spd: spd
        );
    }

    public static RefrigeratorState CreateFood(Guid stateID, int foodID, string grade, int hp, int def, int atk, int spd)
    {
        return new RefrigeratorState(
            stateID: stateID,
            ingredientID: null,
            foodID: foodID,
            grade: grade,
            hp: hp,
            def: def,
            atk: atk,
            spd: spd
        );
    }

    public RefrigeratorState(Dictionary encoded)
    {
        StateID = encoded[nameof(StateID)].ToGuid();
        Grade = (string)((Text)encoded[(Text)nameof(Grade)]).Value;
        HP = (int)((Integer)encoded[(Text)nameof(HP)]).Value;
        DEF = (int)((Integer)encoded[(Text)nameof(DEF)]).Value;
        ATK = (int)((Integer)encoded[(Text)nameof(ATK)]).Value;
        SPD = (int)((Integer)encoded[(Text)nameof(SPD)]).Value;

        if (encoded.ContainsKey((Text)nameof(IngredientID)))
        {
            IngredientID = (int)((Integer)encoded[(Text)nameof(IngredientID)]).Value;
        }

        if (encoded.ContainsKey((Text)nameof(FoodID)))
        {
            FoodID = (int)((Integer)encoded[(Text)nameof(FoodID)]).Value;
        }
    }

    public Guid StateID { get; set; }

    public int? IngredientID { get; set; }

    public int? FoodID { get; set; }

    public string Grade { get; set; }

    public int HP { get; set; }

    public int DEF { get; set; }

    public int ATK { get; set; }

    public int SPD { get; set; }

    public long? ActivatedBlockIndex { get; set; }

    public IValue Serialize()
    {
        var pairs = new List<KeyValuePair<IKey, IValue>>
        {
            new KeyValuePair<IKey, IValue>((Text)nameof(StateID), StateID.Serialize()),
            new KeyValuePair<IKey, IValue>((Text)nameof(Grade), (Text)Grade),
            new KeyValuePair<IKey, IValue>((Text)nameof(HP), (Integer)HP),
            new KeyValuePair<IKey, IValue>((Text)nameof(DEF), (Integer)DEF),
            new KeyValuePair<IKey, IValue>((Text)nameof(ATK), (Integer)ATK),
            new KeyValuePair<IKey, IValue>((Text)nameof(SPD), (Integer)SPD),
        };

        if (IngredientID.HasValue)
        {
            pairs.Add(new KeyValuePair<IKey, IValue>((Text)nameof(IngredientID), (Integer)IngredientID.Value));
        }

        if (FoodID.HasValue)
        {
            pairs.Add(new KeyValuePair<IKey, IValue>((Text)nameof(FoodID), (Integer)FoodID.Value));
        }

        return new Dictionary(pairs);
    }
}
