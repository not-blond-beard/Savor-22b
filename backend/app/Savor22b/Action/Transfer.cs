namespace Savor22b.Action;

using System;
using System.Collections.Immutable;
using Bencodex.Types;
using Libplanet;
using Libplanet.Action;
using Libplanet.State;
using Libplanet.Assets;
using Libplanet.Headless.Extensions;


[ActionType(nameof(Transfer))]
public class Transfer : SVRAction
{
    public Transfer(Address recipient, FungibleAssetValue amount)
    {
        Recipient = recipient;
        Amount = amount;
    }

    public Transfer()
    {
        // Used only for deserialization.  See also class Libplanet.Action.Sys.Registry.
    }

    /// <summary>
    /// The address of the recipient.
    /// </summary>
    public Address Recipient { get; private set; }

    /// <summary>
    /// The amount of the asset to be transferred.
    /// </summary>
    public FungibleAssetValue Amount { get; private set; }

    protected override IImmutableDictionary<string, IValue> PlainValueInternal =>
        new Dictionary<string, IValue>()
        {
            [nameof(Recipient)] = Recipient.ToBencodex(),
            [nameof(Amount)] = Amount.ToBencodex(),
        }.ToImmutableDictionary();

    protected override void LoadPlainValueInternal(
        IImmutableDictionary<string, IValue> plainValue)
    {
        Recipient = plainValue[nameof(Recipient)].ToAddress();
        Amount = plainValue[nameof(Amount)].ToFungibleAssetValue();
    }

    /// <inheritdoc cref="IAction.Execute(IActionContext)"/>
    public override IAccountStateDelta Execute(IActionContext context)
    {
        return context.PreviousStates.TransferAsset(
            context.Signer,
            Recipient,
            Amount,
            allowNegativeBalance: false
        );
    }

    /// <inheritdoc cref="IEquatable{T}.Equals(T)"/>
    public bool Equals(Transfer? other) =>
        other is { } o && Recipient.Equals(o.Recipient) && Amount.Equals(o.Amount);

    /// <inheritdoc cref="IEquatable{T}.Equals(T)"/>
    public bool Equals(IAction? other) => other is Transfer o && Equals(o);

    /// <inheritdoc cref="object.Equals(object?)"/>
    public override bool Equals(object? obj) => obj is Transfer o && Equals(o);

    /// <inheritdoc cref="object.GetHashCode()"/>
    public override int GetHashCode() => HashCode.Combine(Recipient, Amount);
}
