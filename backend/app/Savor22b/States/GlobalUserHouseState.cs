namespace Savor22b.States;

using Bencodex.Types;
using Libplanet;
using Libplanet.Headless.Extensions;

public class GlobalUserHouseState : State
{
    public Dictionary<string, Address> UserHouse { get; private set; }

    public GlobalUserHouseState()
    {
        UserHouse = new Dictionary<string, Address>();
    }

    public GlobalUserHouseState(Dictionary<string, Address> userHouse)
    {
        UserHouse = userHouse;
    }

    public GlobalUserHouseState(Bencodex.Types.Dictionary encoded)
    {
        if (encoded.TryGetValue((Text)nameof(UserHouse), out var userHouse))
        {
            UserHouse = ((Bencodex.Types.Dictionary)userHouse).ToDictionary(
                pair => ((Bencodex.Types.Text)pair.Key).Value,
                pair => pair.Value.ToAddress()
            );
        }
        else
        {
            UserHouse = new Dictionary<string, Address>();
        }
    }

    public IValue Serialize()
    {
        var pairs = new[]
        {
            new KeyValuePair<IKey, IValue>(
                (Text)nameof(UserHouse),
                new Dictionary(
                    UserHouse.Select(
                        e => new KeyValuePair<IKey, IValue>((Text)e.Key, e.Value.ToBencodex())
                    )
                )
            ),
        };

        return new Dictionary(pairs);
    }

    public void SetUserHouse(string address, Address userAddress)
    {
        UserHouse[address] = userAddress;
    }

    public string CreateKey(int villageId, int targetX, int targetY)
    {
        return $"{villageId},{targetX},{targetY}";
    }

    public bool CheckPlacedHouse(int villageID, int targetX, int targetY)
    {
        string key = CreateKey(villageID, targetX, targetY);
        return UserHouse.ContainsKey(key);
    }

    public bool CheckPlacedHouse(string key)
    {
        return UserHouse.ContainsKey(key);
    }
}
