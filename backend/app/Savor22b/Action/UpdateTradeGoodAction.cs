namespace Savor22b.Action;

using System;
using System.Collections.Immutable;
using Bencodex.Types;
using Libplanet.Action;
using Libplanet.Assets;
using Libplanet.Headless.Extensions;
using Libplanet.State;
using Savor22b.States;
using Savor22b.States.Trade;
using Savor22b.Action.Exceptions;

[ActionType(nameof(UpdateTradeGoodAction))]
public class UpdateTradeGoodAction : SVRAction
{
    public UpdateTradeGoodAction() { }

    public UpdateTradeGoodAction(Guid productId, FungibleAssetValue price)
    {
        ProductId = productId;
        Price = price;
    }

    public FungibleAssetValue Price;

    public Guid ProductId;

    protected override IImmutableDictionary<string, IValue> PlainValueInternal =>
        new Dictionary<string, IValue>()
        {
            [nameof(ProductId)] = ProductId.Serialize(),
            [nameof(Price)] = Price.ToBencodex(),
        }.ToImmutableDictionary();

    protected override void LoadPlainValueInternal(IImmutableDictionary<string, IValue> plainValue)
    {
        ProductId = plainValue[nameof(ProductId)].ToGuid();
        Price = plainValue[nameof(Price)].ToFungibleAssetValue();
    }

    public override IAccountStateDelta Execute(IActionContext ctx)
    {
        if (ctx.Rehearsal)
        {
            return ctx.PreviousStates;
        }

        IAccountStateDelta states = ctx.PreviousStates;

        TradeInventoryState tradeInventoryState = states.GetState(TradeInventoryState.StateAddress) is Dictionary tradeInventoryStateEncoded
            ? new TradeInventoryState(tradeInventoryStateEncoded)
            : new TradeInventoryState();

        var good = tradeInventoryState.TradeGoods.First(g => g.Key == ProductId);

        if (good.Value.SellerAddress != ctx.Signer)
        {
            throw new PermissionDeniedException("You not have permission");
        }

        good.Value.UpdatePrice(Price);

        tradeInventoryState.TradeGoods.Remove(good.Key);
        tradeInventoryState.TradeGoods.Add(good.Key, good.Value);

        return states.SetState(TradeInventoryState.StateAddress, tradeInventoryState.Serialize());
    }
}
