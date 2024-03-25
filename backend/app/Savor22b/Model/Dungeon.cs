using System.Collections.Immutable;
using Bencodex.Types;
using Libplanet;
using Libplanet.Blockchain;
using Savor22b.Constants;
using Savor22b.States;

namespace Savor22b.Model;

public class Dungeon
{
    public string Name { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public int ID { get; set; }
    public int VillageId { get; set; }
    public ImmutableList<int> RewardSeedIdList { get; set; }

    private static GlobalDungeonState GetGlobalDungeonState(BlockChain blockChain)
    {
        GlobalDungeonState globalDungeonState = blockChain.GetState(Addresses.DungeonDataAddress)
            is Dictionary stateEncoded
            ? new GlobalDungeonState(stateEncoded)
            : new GlobalDungeonState();

        return globalDungeonState;
    }

    public bool IsConquest(BlockChain blockChain)
    {
        GlobalDungeonState globalDungeonState = GetGlobalDungeonState(blockChain);

        return globalDungeonState.DungeonStatus.ContainsKey(ID.ToString());
    }

    public Address? CurrentConquestUserAddress(BlockChain blockChain)
    {
        GlobalDungeonState globalDungeonState = GetGlobalDungeonState(blockChain);

        return globalDungeonState.DungeonStatus.TryGetValue(ID.ToString(), out Address address)
            ? address
            : default;
    }
}
