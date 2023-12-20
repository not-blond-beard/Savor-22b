namespace Savor22b.States;

using Bencodex.Types;
using Savor22b.Constants;
using Libplanet.Headless.Extensions;
using System.Collections.Immutable;

public class RefrigeratorState : State
{
    public RefrigeratorState(
        Guid stateID,
        int? ingredientID,
        int? foodID,
        string grade,
        int hp,
        int def,
        int atk,
        int spd,
        long? availableBlockIndex,
        ImmutableList<Guid> usedKitchenEquipmentStateIds,
        bool isSuperFood = false
    )
    {
        StateID = stateID;
        IngredientID = ingredientID;
        FoodID = foodID;
        Grade = grade;
        HP = hp;
        DEF = def;
        ATK = atk;
        SPD = spd;
        IsSuperFood = isSuperFood;
        AvailableBlockIndex = availableBlockIndex;
        UsedKitchenEquipmentStateIds = usedKitchenEquipmentStateIds;
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
            null,
            ImmutableList<Guid>.Empty
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
        long availableBlockIndex,
        ImmutableList<Guid> usedKitchenEquipmentStateIds
    )
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
            availableBlockIndex,
            usedKitchenEquipmentStateIds
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
        IsSuperFood = encoded[nameof(IsSuperFood)].ToBoolean();
        AvailableBlockIndex = encoded[nameof(AvailableBlockIndex)].ToNullableLong();
        UsedKitchenEquipmentStateIds = ((List)encoded[nameof(UsedKitchenEquipmentStateIds)])
            .Select(e => e.ToGuid())
            .ToImmutableList();
    }

    public Guid StateID { get; set; }

    public int? IngredientID { get; set; }

    public int? FoodID { get; set; }

    public string Grade { get; set; }

    public int HP { get; set; }

    public int DEF { get; set; }

    public int ATK { get; set; }

    public int SPD { get; set; }

    public bool IsSuperFood { get; set; }

    public long? AvailableBlockIndex { get; set; }

    public ImmutableList<Guid> UsedKitchenEquipmentStateIds { get; set; }

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
            new KeyValuePair<IKey, IValue>((Text)nameof(IsSuperFood), IsSuperFood.Serialize()),
            new KeyValuePair<IKey, IValue>(
                (Text)nameof(UsedKitchenEquipmentStateIds),
                new List(UsedKitchenEquipmentStateIds.Select(element => element.Serialize()))
            ),
            new KeyValuePair<IKey, IValue>(
                (Text)nameof(AvailableBlockIndex),
                AvailableBlockIndex.Serialize()
            ),
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

    public override bool Equals(object obj)
    {
        return obj is RefrigeratorState other
            && StateID == other.StateID
            && IngredientID == other.IngredientID
            && FoodID == other.FoodID
            && Grade == other.Grade
            && HP == other.HP
            && DEF == other.DEF
            && ATK == other.ATK
            && SPD == other.SPD
            && IsSuperFood == other.IsSuperFood;
    }

    public override int GetHashCode()
    {
        int hash = 17;

        hash = hash * 23 + StateID.GetHashCode();
        hash = hash * 23 + (IngredientID?.GetHashCode() ?? 0);
        hash = hash * 23 + (FoodID?.GetHashCode() ?? 0);
        hash = hash * 23 + (Grade?.GetHashCode() ?? 0);
        hash = hash * 23 + HP.GetHashCode();
        hash = hash * 23 + DEF.GetHashCode();
        hash = hash * 23 + ATK.GetHashCode();
        hash = hash * 23 + SPD.GetHashCode();
        hash = hash * 23 + IsSuperFood.GetHashCode();

        return hash;
    }
}
