namespace Savor22b.Action;

using System;
using System.Collections.Immutable;
using Bencodex.Types;
using Libplanet.Action;
using Libplanet.Headless.Extensions;
using Libplanet.State;
using Savor22b.Action.Util;
using Savor22b.Action.Exceptions;
using Savor22b.States;
using Savor22b.Model;
using Libplanet.Blockchain;
using Savor22b.Constants;

[ActionType(nameof(PeriodicDungeonRewardAction))]
public class PeriodicDungeonRewardAction : SVRAction
{
    public PeriodicDungeonRewardAction() { }

    public PeriodicDungeonRewardAction(int dungeonId)
    {
        DungeonId = dungeonId;
    }

    public int DungeonId { get; private set; }

    protected override IImmutableDictionary<string, IValue> PlainValueInternal =>
        new Dictionary<string, IValue>()
        {
            [nameof(DungeonId)] = DungeonId.Serialize(),
        }.ToImmutableDictionary();

    protected override void LoadPlainValueInternal(IImmutableDictionary<string, IValue> plainValue)
    {
        DungeonId = plainValue[nameof(DungeonId)].ToInteger();
    }

    private List<SeedState> GenerateSeedStates(IRandom random, ImmutableList<int> seedIds)
    {
        List<SeedState> seedStates = new List<SeedState>();

        foreach (int seedId in seedIds)
        {
            seedStates.Add(new SeedState(random.GenerateRandomGuid(), seedId));
        }

        return seedStates;
    }

    public override IAccountStateDelta Execute(IActionContext ctx)
    {
        if (ctx.Rehearsal)
        {
            return ctx.PreviousStates;
        }

        Validation.EnsureDungeonExist(DungeonId);

        IAccountStateDelta states = ctx.PreviousStates;

        GlobalDungeonState globalDungeonState = states.GetState(Addresses.DungeonDataAddress)
            is Dictionary encoded
            ? new GlobalDungeonState(encoded)
            : new GlobalDungeonState();
        RootState rootState = states.GetState(ctx.Signer) is Dictionary rootStateEncoded
            ? new RootState(rootStateEncoded)
            : new RootState();
        UserDungeonState userDungeonState = rootState.UserDungeonState;
        InventoryState inventoryState = rootState.InventoryState;

        if (!globalDungeonState.IsDungeonConquestAddress(DungeonId, ctx.Signer))
        {
            throw new PermissionDeniedException(
                $"The dungeon {DungeonId} has not been conquered by the signer."
            );
        }

        DungeonConquestHistoryState? history = userDungeonState.CurrentConquestDungeonHistory(
            DungeonId
        );

        if (history is null)
        {
            throw new PermissionDeniedException(
                $"The dungeon {DungeonId} has not been conquered by the signer."
            );
        }

        int dungeonPeriodicRewardCount = userDungeonState.PresentDungeonPeriodicRewardCount(
            DungeonId,
            history.BlockIndex,
            ctx.BlockIndex
        );

        if (dungeonPeriodicRewardCount <= 0)
        {
            throw new ResourceIsNotReadyException(
                $"You need to wait blocks to get the periodic reward."
            );
        }

        Dungeon dungeon = CsvDataHelper.GetDungeonById(DungeonId)!;

        for (int i = 0; i < dungeonPeriodicRewardCount; i++)
        {
            DungeonConquestPeriodicRewardHistoryState periodicRewardHistory =
                new DungeonConquestPeriodicRewardHistoryState(
                    ctx.BlockIndex,
                    DungeonId,
                    dungeon.RewardSeedIdList
                );

            userDungeonState = userDungeonState.AddDungeonPeriodicRewardHistory(
                periodicRewardHistory
            );
            inventoryState = inventoryState.AddSeeds(
                GenerateSeedStates(ctx.Random, dungeon.RewardSeedIdList)
            );
        }

        rootState.SetUserDungeonState(userDungeonState);
        rootState.SetInventoryState(inventoryState);

        return states.SetState(ctx.Signer, rootState.Serialize());
    }
}
