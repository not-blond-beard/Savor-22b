namespace Savor22b.Action;

using Libplanet.Action;
using Savor22b.States;
using Savor22b.Model;
using System.Collections.Immutable;
using Bencodex.Types;
using Libplanet.Headless.Extensions;
using Libplanet.State;
using Savor22b.Action.Exceptions;
using Savor22b.Constants;
using Libplanet;

[ActionType(nameof(ConquestDungeonAction))]
public class ConquestDungeonAction : SVRAction
{
    public int DungeonId { get; private set; }

    public ConquestDungeonAction() { }

    public ConquestDungeonAction(int dungeonId)
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

    private void AssertDungeonExists(int dungeonId)
    {
        Dungeon? dungeon = CsvDataHelper.GetDungeonById(dungeonId);

        if (dungeon == null)
        {
            throw new InvalidDungeonException($"Invalid dungeon ID: {dungeonId}");
        }
    }

    public override IAccountStateDelta Execute(IActionContext ctx)
    {
        if (ctx.Rehearsal)
        {
            return ctx.PreviousStates;
        }

        AssertDungeonExists(DungeonId);

        IAccountStateDelta states = ctx.PreviousStates;

        RootState rootState = states.GetState(ctx.Signer) is Dictionary rootStateEncoded
            ? new RootState(rootStateEncoded)
            : new RootState();
        UserDungeonState userDungeonState = rootState.UserDungeonState;
        GlobalDungeonState globalDungeonState = states.GetState(Addresses.DungeonDataAddress)
            is Dictionary encoded
            ? new GlobalDungeonState(encoded)
            : new GlobalDungeonState();

        if (!userDungeonState.IsDungeonCleared(DungeonId, ctx.BlockIndex))
        {
            throw new DungeonNotClearedException($"Dungeon {DungeonId} is not cleared yet.");
        }

        if (!userDungeonState.CanUseDungeonConquestKey(DungeonId, ctx.BlockIndex))
        {
            throw new NotHaveRequiredException("You don't have enough dungeon conquest key");
        }

        Address? presentDungeonOwner = globalDungeonState.DungeonConquestAddress(DungeonId);

        if (presentDungeonOwner == null)
        {
            globalDungeonState = globalDungeonState.SetDungeonConquestAddress(
                DungeonId,
                ctx.Signer
            );
            userDungeonState = userDungeonState.AddDungeonConquestHistory(
                new DungeonConquestHistoryState(ctx.BlockIndex, DungeonId, 1)
            );
        }
        else if (presentDungeonOwner == ctx.Signer)
        {
            throw new AlreadyOwnedException("You already own this dungeon");
        }
        else
        {
            globalDungeonState = globalDungeonState.SetDungeonConquestAddress(
                DungeonId,
                ctx.Signer
            );
            userDungeonState = userDungeonState.AddDungeonConquestHistory(
                new DungeonConquestHistoryState(
                    ctx.BlockIndex,
                    DungeonId,
                    userDungeonState.CalculateDungeonConquest(ctx.Random),
                    presentDungeonOwner
                )
            );
        }

        rootState.SetUserDungeonState(userDungeonState);

        return states
            .SetState(Addresses.DungeonDataAddress, globalDungeonState.Serialize())
            .SetState(ctx.Signer, rootState.Serialize());
    }
}
