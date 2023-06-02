namespace Savor22b.Action;

using System;
using System.Reflection;
using Bencodex.Types;
using Libplanet;
using Libplanet.Action;
using Libplanet.Assets;
using Libplanet.State;


[ActionType(0)]
public class Mint : SVRBaseAction
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

    public IValue TypeId =>
        this.GetType().GetCustomAttribute<ActionTypeAttribute>()!.TypeIdentifier;

    /// <inheritdoc cref="IAction.PlainValue"/>
    public override IValue PlainValue => Bencodex.Types.Dictionary.Empty
        .Add("type_id", TypeId)
        .Add("values", Bencodex.Types.Dictionary.Empty
            .Add("recipient", Recipient.ByteArray)
            .Add("currency", Amount.Currency.Serialize())
            .Add("amount", new Bencodex.Types.Integer(Amount.RawValue)));

    /// <inheritdoc cref="IAction.LoadPlainValue(IValue)"/>
    public override void LoadPlainValue(IValue plainValue)
    {
        if (!(plainValue is Dictionary dict))
        {
            throw new ArgumentException(
                $"Given {nameof(plainValue)} must be a {nameof(Dictionary)}: " +
                $"{plainValue.GetType()}",
                nameof(plainValue));
        }

        if (!dict.TryGetValue((Text)"type_id", out IValue typeId))
        {
            throw new ArgumentException(
                $"Given {nameof(plainValue)} is missing type id: {plainValue}",
                nameof(plainValue));
        }

        if (!typeId.Equals(TypeId))
        {
            throw new ArgumentException(
                $"Given {nameof(plainValue)} has invalid type id: {plainValue}",
                nameof(plainValue));
        }

        if (!dict.TryGetValue((Text)"values", out IValue values))
        {
            throw new ArgumentException(
                $"Given {nameof(plainValue)} is missing values: {plainValue}",
                nameof(plainValue));
        }

        if (!(values is Dictionary valuesDict))
        {
            throw new ArgumentException(
                $"Given {nameof(plainValue)} has invalid values: {plainValue}",
                nameof(plainValue));
        }

        Recipient = new Address(valuesDict.GetValue<IValue>("recipient"));
        Amount = FungibleAssetValue.FromRawValue(
            new Currency(valuesDict["currency"]),
            valuesDict.GetValue<Bencodex.Types.Integer>("amount"));
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
