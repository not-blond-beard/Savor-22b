
namespace Savor22b.States;

using Bencodex.Types;


public class VillageState : State
{
    public HouseFieldState[] HouseFieldStateList { get; private set; }
    public HouseState HouseState { get; private set; }

    public VillageState(HouseState houseState)
    {
        HouseFieldStateList = new HouseFieldState[10].Select(houseFieldState => new HouseFieldState()).ToArray();
        HouseState = houseState;
    }

    public VillageState(HouseFieldState[] houseFieldStateList, HouseState houseState)
    {
        HouseFieldStateList = houseFieldStateList;
        HouseState = houseState;
    }

    public VillageState(Bencodex.Types.Dictionary encoded)
    {
        HouseFieldStateList = ((List)encoded[nameof(HouseFieldStateList)]).Select(houseFieldState => new HouseFieldState((Bencodex.Types.Dictionary)houseFieldState)).ToArray();
        HouseState = new HouseState((Bencodex.Types.Dictionary)encoded[nameof(HouseState)]);
    }

    public IValue Serialize()
    {
        var pairs = new[]
        {
            new KeyValuePair<IKey, IValue>((Text)nameof(HouseFieldStateList), new List(HouseFieldStateList.Select(houseFieldState => houseFieldState.Serialize()))),
            new KeyValuePair<IKey, IValue>((Text)nameof(HouseState), HouseState.Serialize()),
        };

        return new Dictionary(pairs);
    }

    public void SetHouseState(HouseState houseState)
    {
        HouseState = houseState;
    }

    public void SetHouseFieldStateList(HouseFieldState[] houseFieldStateList)
    {
        HouseFieldStateList = houseFieldStateList;
    }
}
