namespace Savor22b.States;


using Bencodex.Types;
using Libplanet.Headless.Extensions;

public class HouseState : State
{
    public int VillageID { get; private set; }
    public int PositionX { get; private set; }
    public int PositionY { get; private set; }
    public HouseInnerState InnerState { get; private set; }

    public HouseState(int villageID, int positionX, int positionY, HouseInnerState innerState)
    {
        VillageID = villageID;
        PositionX = positionX;
        PositionY = positionY;
        InnerState = innerState;
    }

    public HouseState(Bencodex.Types.Dictionary encoded)
    {
        VillageID = encoded[nameof(VillageID)].ToInteger();
        PositionX = encoded[nameof(PositionX)].ToInteger();
        PositionY = encoded[nameof(PositionY)].ToInteger();
        InnerState = new HouseInnerState((Bencodex.Types.Dictionary)encoded[nameof(InnerState)]);
    }

    public IValue Serialize()
    {

        var pairs = new[]
        {
            new KeyValuePair<IKey, IValue>((Text)nameof(VillageID), VillageID.Serialize()),
            new KeyValuePair<IKey, IValue>((Text)nameof(PositionX), PositionX.Serialize()),
            new KeyValuePair<IKey, IValue>((Text)nameof(PositionY), PositionY.Serialize()),
            new KeyValuePair<IKey, IValue>((Text)nameof(InnerState), InnerState.Serialize()),

        };

        return new Dictionary(pairs);
    }
}
