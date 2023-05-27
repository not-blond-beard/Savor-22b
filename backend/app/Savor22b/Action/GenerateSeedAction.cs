namespace Savor22b.Action;

using System;
using Libplanet.Action;
using Libplanet.Store;
using Savor22b.States;
using Serilog;


[ActionType("generate_seed")]
public class GenerateSeedAction : BaseAction
{

    class ActionPlainValue : DataModel
    {

        public ActionPlainValue()
            : base()
        {
        }


        public ActionPlainValue(Bencodex.Types.Dictionary encoded)
            : base(encoded)
        {
        }
    }

    private ActionPlainValue _plainValue;

    public GenerateSeedAction()
    {
        _plainValue = new ActionPlainValue();
    }

    public override Bencodex.Types.IValue PlainValue => _plainValue.Encode();

    public override void LoadPlainValue(Bencodex.Types.IValue plainValue)
    {
        if (plainValue is Bencodex.Types.Dictionary bdict)
        {
            _plainValue = new ActionPlainValue(bdict);
        }
        else
        {
            throw new ArgumentException(
                $"Invalid {nameof(plainValue)} type: {plainValue.GetType()}");
        }
    }

    public override IAccountStateDelta Execute(IActionContext ctx)
    {
        IAccountStateDelta states = ctx.PreviousStates;

        InventoryState inventoryState =
            states.GetState(ctx.Signer) is Bencodex.Types.Dictionary stateEncoded
                ? new InventoryState(stateEncoded)
                : new InventoryState();
        // 랜덤으로 생성, csv 데이터에서 읽어와야함
        SeedState seedState = new SeedState(1, "test");
        inventoryState = inventoryState.AddSeed(seedState);

        var encodedValue = inventoryState.ToBencodex();
        var statesWithUpdated = states.SetState(ctx.Signer, encodedValue);

        return statesWithUpdated;
    }
}
