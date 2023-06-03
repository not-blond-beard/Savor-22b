namespace Savor22b.Action;

using System.Collections.Immutable;
using Bencodex.Types;
using Libplanet;
using Libplanet.Action;
using Libplanet.State;
using Libplanet.Assets;
using Libplanet.Headless.Extensions;

/// <summary>
/// Basically, it's just a double of <see cref="Libplanet.Action.Sys.Tranfer"/>,
/// a system built-in action.  Although it is redundant, it's here for
/// an example of composing custom actions.
/// </summary>
[ActionType(nameof(TransferAsset))]
public class TransferAsset : SVRAction
{
    public TransferAsset()
    {
    }

    public TransferAsset(Address sender, Address recipient, FungibleAssetValue amount)
    {
        Sender = sender;
        Recipient = recipient;
        Amount = amount;
    }

    public Address Sender { get; private set; }

    public Address Recipient { get; private set; }

    public FungibleAssetValue Amount { get; private set; }

    protected override IImmutableDictionary<string, IValue> PlainValueInternal =>
        new Dictionary<string, IValue>()
        {
            [nameof(Sender)] = Sender.ToBencodex(),
            [nameof(Recipient)] = Recipient.ToBencodex(),
            [nameof(Amount)] = Amount.ToBencodex(),
        }.ToImmutableDictionary();

    protected override void LoadPlainValueInternal(
        IImmutableDictionary<string, IValue> plainValue)
    {
        Sender = plainValue[nameof(Sender)].ToAddress();
        Recipient = plainValue[nameof(Recipient)].ToAddress();
        Amount = plainValue[nameof(Amount)].ToFungibleAssetValue();
    }

    public override IAccountStateDelta Execute(IActionContext context)
    {
        IAccountStateDelta? state = context.PreviousStates;

        if (context.Rehearsal)
        {
            return state;
        }

        if (Sender != context.Signer)
        {
            throw new InvalidTransferSignerException(context.Signer, Sender, Recipient);
        }

        return state.TransferAsset(Sender, Recipient, Amount);
    }
}
