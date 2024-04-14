namespace Savor22b.States;

using Libplanet.Headless.Extensions;

using Bencodex.Types;
using Libplanet;

public class GlobalDungeonState : State
{
    public Dictionary<string, Address> DungeonStatus { get; private set; }

    public GlobalDungeonState()
    {
        DungeonStatus = new Dictionary<string, Address>();
    }

    public GlobalDungeonState(Dictionary<string, Address> dungeonStatus)
    {
        DungeonStatus = dungeonStatus;
    }

    public GlobalDungeonState(Bencodex.Types.Dictionary encoded)
    {
        if (encoded.TryGetValue((Text)nameof(DungeonStatus), out var dungeonStatus))
        {
            DungeonStatus = ((Bencodex.Types.Dictionary)dungeonStatus).ToDictionary(
                pair => ((Bencodex.Types.Text)pair.Key).Value,
                pair => pair.Value.ToAddress()
            );
        }
        else
        {
            DungeonStatus = new Dictionary<string, Address>();
        }
    }

    public IValue Serialize()
    {
        var pairs = new[]
        {
            new KeyValuePair<IKey, IValue>(
                (Text)nameof(DungeonStatus),
                new Dictionary(
                    DungeonStatus.Select(
                        e => new KeyValuePair<IKey, IValue>((Text)e.Key, e.Value.ToBencodex())
                    )
                )
            ),
        };

        return new Dictionary(pairs);
    }

    public Address? DungeonConquestAddress(int dungeonId)
    {
        return DungeonStatus.TryGetValue(dungeonId.ToString(), out Address address)
            ? address
            : null;
    }

    public GlobalDungeonState SetDungeonConquestAddress(int dungeonId, Address address)
    {
        DungeonStatus[dungeonId.ToString()] = address;
        return new GlobalDungeonState(DungeonStatus);
    }
}
