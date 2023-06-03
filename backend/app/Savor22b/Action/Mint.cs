namespace Savor22b.Action;

using System;
using System.Collections.Immutable;
using Bencodex.Types;
using Libplanet;
using Libplanet.Action;
using Libplanet.Assets;
using Libplanet.State;
using Libplanet.Headless.Extensions;


[ActionType(0)]
public class Mint : SVRAction
{
    /// <summary>
    /// Creates a new instance of <see cref="Mint"/> action.
    /// </summary>
    /// <param name="recipient">The address of the recipient to receive the minted tokens.
    /// </param>
    /// <param name="amount">The amount of the asset to be minted.</param>
    public Mint(Address recipient, FungibleAssetValue amount)
    {
        Recipient = recipient;
        Amount = amount;
    }

    public Mint()
    {
        // Used only for deserialization.  See also class Libplanet.Action.Sys.Registry.
    }

    /// <summary>
    /// The address of the recipient to receive the minted tokens.
    /// </summary>
    public Address Recipient { get; private set; }

    /// <summary>
    /// The amount of the asset to be minted.
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
        return context.PreviousStates.MintAsset(Recipient, Amount);
    }

    /// <inheritdoc cref="IEquatable{T}.Equals(T)"/>
    public bool Equals(Mint? other) =>
        other is { } o && Recipient.Equals(o.Recipient) && Amount.Equals(o.Amount);

    /// <inheritdoc cref="IEquatable{T}.Equals(T)"/>
    public bool Equals(IAction? other) => other is Mint o && Equals(o);

    /// <inheritdoc cref="object.Equals(object?)"/>
    public override bool Equals(object? obj) => obj is Mint o && Equals(o);

    /// <inheritdoc cref="object.GetHashCode()"/>
    public override int GetHashCode() => HashCode.Combine(Recipient, Amount);
}
