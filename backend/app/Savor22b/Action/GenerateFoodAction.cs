namespace Savor22b.Action;

using System.Collections.Immutable;
using Bencodex.Types;
using System;
using Libplanet.Action;
using Libplanet.State;
using Savor22b.Helpers;
using Savor22b.Model;
using Savor22b.States;
using Libplanet.Headless.Extensions;


[ActionType(nameof(GenerateFoodAction))]
public class GenerateFoodAction : SVRAction
{
    public Guid RefrigeratorStateID;
    public Guid SeedStateID;

    public GenerateFoodAction()
    {
    }


    protected override IImmutableDictionary<string, IValue> PlainValueInternal =>
        new Dictionary<string, IValue>(){
            [nameof(SeedStateID)] = SeedStateID.Serialize(),
            [nameof(RefrigeratorStateID)] = RefrigeratorStateID.Serialize()
        }.ToImmutableDictionary();

    protected override void LoadPlainValueInternal(
        IImmutableDictionary<string, IValue> plainValue)
    {
        SeedStateID = plainValue[nameof(SeedStateID)].ToGuid();
        RefrigeratorStateID = plainValue[nameof(RefrigeratorStateID)].ToGuid();
    }

    public override IAccountStateDelta Execute(IActionContext ctx)
    {
        if (ctx.Rehearsal)
        {
            return ctx.PreviousStates;
        }

        IAccountStateDelta states = ctx.PreviousStates;

        InventoryState inventoryState =
            states.GetState(ctx.Signer) is Bencodex.Types.Dictionary stateEncoded
                ? new InventoryState(stateEncoded)
                : new InventoryState();

        return states.SetState(ctx.Signer, inventoryState.Serialize());
    }
}
