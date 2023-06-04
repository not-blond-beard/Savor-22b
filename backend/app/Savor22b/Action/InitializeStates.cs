namespace Savor22b.Action;

using System.Collections.Immutable;
using Bencodex.Types;
using Libplanet;
using Libplanet.Action;
using Libplanet.Assets;
using Libplanet.Headless.Extensions;
using Libplanet.State;

[ActionType(nameof(InitializeStates))]
public class InitializeStates : SVRAction
{
    private Dictionary<Address, FungibleAssetValue> _assets;

    public InitializeStates()
    {
        _assets = new Dictionary<Address, FungibleAssetValue>();
    }

    public InitializeStates(Dictionary<Address, FungibleAssetValue> assets)
    {
        _assets = assets;
    }

    public override IAccountStateDelta Execute(IActionContext context)
    {
        IAccountStateDelta? states = context.PreviousStates;

        if (context.BlockIndex != 0)
        {
            return states;
        }

        foreach ((Address address, FungibleAssetValue value) in _assets)
        {
            states = states.MintAsset(address, value);
        }

        return states;
    }

    protected override IImmutableDictionary<string, IValue> PlainValueInternal =>
        new Dictionary<string, IValue>()
        {
            [nameof(_assets)] = new Bencodex.Types.Dictionary(
                _assets.Select(kv => new KeyValuePair<IKey, IValue>(
                    (Binary)kv.Key.ToBencodex(),
                    kv.Value.ToBencodex()
                ))),
        }.ToImmutableDictionary();

    protected override void LoadPlainValueInternal(
        IImmutableDictionary<string, IValue> plainValue)
    {
        _assets = new Dictionary<Address, FungibleAssetValue>(
            ((Dictionary)plainValue[nameof(_assets)]).Select(kv =>
                new KeyValuePair<Address, FungibleAssetValue>(
                    kv.Key.ToAddress(),
                    kv.Value.ToFungibleAssetValue()
                )
            )
        );
    }
}
