
namespace Savor22b.States;

using Bencodex.Types;


public class VillageState : State
{
    public static readonly int HouseFieldCount = 10;
    public Dictionary<int, HouseFieldState?> HouseFieldStates { get; private set; }
    public HouseState HouseState { get; private set; }

    public Dictionary<int, HouseFieldState?> InitializeHouseFieldStates()
    {
        Dictionary<int, HouseFieldState?> houseFieldStates = new();
        for (int i = 0; i < HouseFieldCount; i++)
        {
            houseFieldStates.Add(i, null);
        }

        return houseFieldStates;
    }

    private Dictionary<int, HouseFieldState?> InitializeHouseFieldStatesFromData(Dictionary<int, HouseFieldState> existingHouseFieldStates)
    {
        Dictionary<int, HouseFieldState?> houseFieldStates = new();
        for (int i = 0; i < HouseFieldCount; i++)
        {
            if (existingHouseFieldStates.ContainsKey(i))
            {
                houseFieldStates.Add(i, existingHouseFieldStates[i]);
            }
            else
            {
                houseFieldStates.Add(i, null);
            }
        }

        return houseFieldStates;
    }
    public VillageState(HouseState houseState)
    {
        HouseState = houseState;
        HouseFieldStates = InitializeHouseFieldStates();
    }

    public VillageState(HouseState houseState, Dictionary<int, HouseFieldState?> houseFieldStates)
    {
        HouseState = houseState;
        HouseFieldStates = houseFieldStates;
    }

    public VillageState(Dictionary encoded)
    {
        HouseState = new HouseState((Dictionary)encoded[nameof(HouseState)]);

        Dictionary<int, HouseFieldState> existingHouseFieldStates = ((Dictionary)encoded[nameof(HouseFieldStates)]).ToDictionary(
                pair => int.Parse(((Text)pair.Key).Value),
                pair => new HouseFieldState((Dictionary)pair.Value)
            );
        HouseFieldStates = InitializeHouseFieldStatesFromData(existingHouseFieldStates);
    }

    public IValue Serialize()
    {
        var pairs = new List<KeyValuePair<IKey, IValue>>(){
            new KeyValuePair<IKey, IValue>((Text)nameof(HouseState), HouseState.Serialize())
        };

        var houseFieldStatePairs = new List<KeyValuePair<IKey, IValue>>();
        foreach (KeyValuePair<int, HouseFieldState?> pair in HouseFieldStates)
        {
            if (pair.Value is not null)
            {
                houseFieldStatePairs.Add(new KeyValuePair<IKey, IValue>((Text)pair.Key.ToString(), pair.Value.Serialize()));
            }
        }

        pairs.Add(new KeyValuePair<IKey, IValue>((Text)nameof(HouseFieldStates), new Dictionary(houseFieldStatePairs)));

        return new Dictionary(pairs);
    }

    public void SetHouseState(HouseState houseState)
    {
        HouseState = houseState;
    }

    public void UpdateHouseFieldState(int fieldIndex, HouseFieldState? houseFieldState)
    {
        HouseFieldStates[fieldIndex] = houseFieldState;
    }

    public void RemoveHouseFieldState(int fieldIndex)
    {
        HouseFieldStates[fieldIndex] = null;
    }
}
