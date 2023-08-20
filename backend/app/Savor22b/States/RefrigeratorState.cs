namespace Savor22b.States;

using Bencodex.Types;
using Savor22b.Constants;
using Libplanet.Headless.Extensions;

public class RefrigeratorState : State
{
    public RefrigeratorState(Guid stateID, int? ingredientID, int? foodID, string grade, int hp, int def, int atk, int spd, long? availableBlockIndex)
    {
        StateID = stateID;
        IngredientID = ingredientID;
        FoodID = foodID;
        Grade = grade;
        HP = hp;
        DEF = def;
        ATK = atk;
        SPD = spd;
        AvailableBlockIndex = availableBlockIndex;
    }

    public static RefrigeratorState CreateIngredient(
        Guid stateID,
        int ingredientID,
        string grade,
        int hp,
        int def,
        int atk,
        int spd
    )
    {
        return new RefrigeratorState(
            stateID,
            ingredientID,
            null,
            grade,
            hp,
            def,
            atk,
            spd,
            null
        );
    }

    public static RefrigeratorState CreateFood(
        Guid stateID,
        int foodID,
        string grade,
        int hp,
        int def,
        int atk,
        int spd,
        long? availableBlockIndex)
    {
        return new RefrigeratorState(
            stateID,
            null,
            foodID,
            grade,
            hp,
            def,
            atk,
            spd,
            availableBlockIndex
        );
    }

    public RefrigeratorState(Dictionary encoded)
    {
        StateID = encoded[nameof(StateID)].ToGuid();
        Grade = encoded[nameof(Grade)].ToDotnetString();
        HP = encoded[nameof(HP)].ToInteger();
        DEF = encoded[nameof(DEF)].ToInteger();
        ATK = encoded[nameof(ATK)].ToInteger();
        SPD = encoded[nameof(SPD)].ToInteger();
        IngredientID = encoded[nameof(IngredientID)].ToNullableInteger();
        FoodID = encoded[nameof(FoodID)].ToNullableInteger();
        AvailableBlockIndex = encoded[nameof(AvailableBlockIndex)].ToNullableLong();
    }

    public Guid StateID { get; set; }

    public int? IngredientID { get; set; }

    public int? FoodID { get; set; }

    public string Grade { get; set; }

    public int HP { get; set; }

    public int DEF { get; set; }

    public int ATK { get; set; }

    public int SPD { get; set; }

    public long? AvailableBlockIndex { get; set; }

    public IValue Serialize()
    {
        var pairs = new List<KeyValuePair<IKey, IValue>>
        {
            new KeyValuePair<IKey, IValue>((Text)nameof(StateID), StateID.Serialize()),
            new KeyValuePair<IKey, IValue>((Text)nameof(Grade), Grade.Serialize()),
            new KeyValuePair<IKey, IValue>((Text)nameof(HP), HP.Serialize()),
            new KeyValuePair<IKey, IValue>((Text)nameof(DEF), DEF.Serialize()),
            new KeyValuePair<IKey, IValue>((Text)nameof(ATK), ATK.Serialize()),
            new KeyValuePair<IKey, IValue>((Text)nameof(SPD), SPD.Serialize()),
            new KeyValuePair<IKey, IValue>((Text)nameof(IngredientID), IngredientID.Serialize()),
            new KeyValuePair<IKey, IValue>((Text)nameof(FoodID), FoodID.Serialize()),
            new KeyValuePair<IKey, IValue>((Text)nameof(AvailableBlockIndex), AvailableBlockIndex.Serialize()),
        };

        return new Dictionary(pairs);
    }

    public bool IsAvailable(long currentBlockIndex)
    {
        if (AvailableBlockIndex is null)
        {
            return true;
        }

        return currentBlockIndex > AvailableBlockIndex;
    }

    public Edible GetEdibleType()
    {
        return FoodID is not null ? Edible.FOOD : Edible.INGREDIENT;
    }
}
