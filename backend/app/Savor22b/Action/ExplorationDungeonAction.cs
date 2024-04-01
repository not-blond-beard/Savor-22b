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

[ActionType(nameof(ExplorationDungeonAction))]
public class ExplorationDungeonAction : SVRAction
{
    public int DungeonId { get; private set; }

    public ExplorationDungeonAction() { }

    public ExplorationDungeonAction(int dungeonId)
    {
        DungeonId = dungeonId;
    }

    protected override IImmutableDictionary<string, IValue> PlainValueInternal =>
        new Dictionary<string, IValue>()
        {
            [nameof(DungeonId)] = DungeonId.Serialize(),
        }.ToImmutableDictionary();

    protected override void LoadPlainValueInternal(IImmutableDictionary<string, IValue> plainValue)
    {
        DungeonId = plainValue[nameof(DungeonId)].ToInteger();
    }

    private Dungeon ValidateDungeon(int dungeonId)
    {
        Dungeon? dungeon = CsvDataHelper.GetDungeonById(dungeonId);

        if (dungeon == null)
        {
            throw new InvalidDungeonException($"Invalid dungeon ID: {dungeonId}");
        }

        return dungeon;
    }

    private void ValidateUseDungeonKey(UserDungeonState userDungeonState, long blockIndex)
    {
        if (!userDungeonState.CanUseDungeonKey(blockIndex))
        {
            throw new NotHaveRequiredException("You don't have enough dungeon key");
        }
    }

    public override IAccountStateDelta Execute(IActionContext ctx)
    {
        if (ctx.Rehearsal)
        {
            return ctx.PreviousStates;
        }

        Dungeon dungeon = ValidateDungeon(DungeonId);

        IAccountStateDelta states = ctx.PreviousStates;

        RootState rootState = states.GetState(ctx.Signer) is Dictionary rootStateEncoded
            ? new RootState(rootStateEncoded)
            : new RootState();

        UserDungeonState userDungeonState = rootState.UserDungeonState;

        ValidateUseDungeonKey(userDungeonState, ctx.BlockIndex);

        DungeonHistoryState dungeonHistory = new DungeonHistoryState(
            ctx.BlockIndex,
            DungeonId,
            userDungeonState.CalculateDungeonClear(ctx.Random),
            dungeon.RewardSeedIdList
        );

        userDungeonState = userDungeonState.AddDungeonHistory(dungeonHistory);
        rootState.SetUserDungeonState(userDungeonState);

        return states.SetState(ctx.Signer, rootState.Serialize());
    }
}
