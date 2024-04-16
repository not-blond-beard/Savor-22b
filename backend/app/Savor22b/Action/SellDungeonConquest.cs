namespace Savor22b.Action;

using System.Collections.Immutable;
using Bencodex.Types;
using Libplanet.Action;
using Libplanet.Headless.Extensions;
using Libplanet.State;
using Savor22b.Action.Exceptions;
using Savor22b.States;
using Savor22b.Model;
using Libplanet.Assets;

[ActionType(nameof(SellDungeonConquest))]
public class SellDungeonConquest : SVRAction
{
    public int DungeonId { get; private set; }

    public SellDungeonConquest() { }

    public SellDungeonConquest(int dungeonId)
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

        GlobalDungeonState globalDungeonState = states.GetState(GlobalDungeonState.StateAddress) is Dictionary globalDungeonStateEncoded
            ? new GlobalDungeonState(globalDungeonStateEncoded)
            : new GlobalDungeonState();

        if (!globalDungeonState.IsDungeonConquestAddress(DungeonId, ctx.Signer))
        {
            throw new InvalidValueException($"Invalid dungeon ID: {DungeonId}");
        }

        Dungeon dungeon = GetDungeonInCsv(DungeonId);

        globalDungeonState = globalDungeonState.RemoveDungeonConquestAddress(DungeonId);

        states = states.MintAsset(ctx.Signer, FungibleAssetValue.Parse(Currencies.KeyCurrency, dungeon.ReturnDungeonRewardBBG));
        return states.SetState(GlobalDungeonState.StateAddress, globalDungeonState.Serialize());
    }
}
