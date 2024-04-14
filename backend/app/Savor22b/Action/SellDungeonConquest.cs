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
using Libplanet.Assets;

[ActionType(nameof(SellDungeonConquest))]
public class SellDungeonConquest : SVRAction
{
    public Guid DungeonConquestHistoryStateId { get; private set; }

    public SellDungeonConquest() { }

    public SellDungeonConquest(Guid dungeonConquestHistoryStateId)
    {
        DungeonConquestHistoryStateId = dungeonConquestHistoryStateId;
    }

    protected override IImmutableDictionary<string, IValue> PlainValueInternal =>
        new Dictionary<string, IValue>()
        {
            [nameof(DungeonConquestHistoryStateId)] = DungeonConquestHistoryStateId.Serialize(),
        }.ToImmutableDictionary();

    protected override void LoadPlainValueInternal(IImmutableDictionary<string, IValue> plainValue)
    {
        DungeonConquestHistoryStateId = plainValue[nameof(DungeonConquestHistoryStateId)].ToGuid();
    }

    private Dungeon GetDungeonInCsv(int dungeonId)
    {
        Dungeon? dungeon = CsvDataHelper.GetDungeonById(dungeonId);

        if (dungeon == null)
        {
            throw new InvalidDungeonException($"Invalid dungeon ID: {dungeonId}");
        }

        return dungeon;
    }

    public override IAccountStateDelta Execute(IActionContext ctx)
    {
        if (ctx.Rehearsal)
        {
            return ctx.PreviousStates;
        }

        IAccountStateDelta states = ctx.PreviousStates;

        RootState rootState = states.GetState(ctx.Signer) is Dictionary rootStateEncoded
            ? new RootState(rootStateEncoded)
            : new RootState();
        UserDungeonState userDungeonState = rootState.UserDungeonState;

        var dungeonConquestHistory = userDungeonState.DungeonConquestHistories.FirstOrDefault(d => d.StateId == DungeonConquestHistoryStateId);

        if (dungeonConquestHistory == null)
        {
            throw new InvalidValueException($"Invalid state ID: {DungeonConquestHistoryStateId}");
        }

        Dungeon dungeon = GetDungeonInCsv(dungeonConquestHistory.DungeonId);

        states = states.MintAsset(ctx.Signer, FungibleAssetValue.Parse(Currencies.KeyCurrency, dungeon.ReturnDungeonRewardBBG));

        userDungeonState = userDungeonState.RemoveDungeonConquestHistory(dungeonConquestHistory);

        rootState.SetUserDungeonState(userDungeonState);

        return states.SetState(ctx.Signer, rootState.Serialize());
    }
}
