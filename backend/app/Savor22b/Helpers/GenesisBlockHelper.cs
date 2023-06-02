namespace Savor22b.Helpers;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Numerics;
using Bencodex;
using Bencodex.Types;
using ImmutableTrie;
using Libplanet;
using Libplanet.Action;
using Libplanet.Action.Sys;
using Libplanet.Assets;
using Libplanet.Blockchain;
using Libplanet.Blocks;
using Libplanet.Consensus;
using Libplanet.Crypto;
using Libplanet.Store;
using Libplanet.Store.Trie;
using Libplanet.Tx;
using Savor22b;
using Savor22b.Action;

public static class GenesisBlockHelper
{
    public static Block ProposeGenesisBlock(
        PrivateKey[] validatorKeys,
        PrivateKey privateKey,
        DateTimeOffset? timestamp = null
    )
    {
        var validatorSet = new ValidatorSet(
            validatorKeys
                .Select(k => new Validator(k.PublicKey, BigInteger.One))
                .ToList());
        var txs = CreateTxs(privateKey, validatorSet);

        var blockAction = BlockPolicySource.GetPolicy().BlockAction;
        var actionEvaluator = new ActionEvaluator(
            _ => blockAction,
            new BlockChainStates(new MemoryStore(), new TrieStateStore(new MemoryKeyValueStore())),
            new SVRActionLoader(),
            null);
        return
            BlockChain.ProposeGenesisBlock(
                actionEvaluator,
                transactions: txs,
                privateKey: privateKey,
                blockAction: blockAction,
                timestamp: timestamp);
    }

    public static byte[] ExportBlock(Block block)
    {
        Bencodex.Types.Dictionary dict = block.MarshalBlock();
        byte[] encoded = new Codec().Encode(dict);
        return encoded;
    }

    private static ImmutableList<Transaction> CreateTxs(PrivateKey privateKey,  ValidatorSet validatorSet)
    {
        var actions = CreateInitialStatesAction(privateKey, validatorSet);
        ImmutableList<Transaction> txs = Array.Empty<Transaction>()
            .Append(Transaction.Create(
                0,
                privateKey,
                null,
                actions))
            .ToImmutableList();
        return txs;
    }

    private static List<IAction> CreateInitialStatesAction(PrivateKey privateKey, ValidatorSet validatorSet)
    {
        Dictionary<Address, FungibleAssetValue> assets = new Dictionary<Address, FungibleAssetValue>();
        assets.Add(
            privateKey.PublicKey.ToAddress(),
            FungibleAssetValue.Parse(
                Currencies.KeyCurrency,
                "10000"
            ));

        List<IAction> actions = new List<IAction>
        {
            new InitializeStates(assets),
            new Initialize(validatorSet, ImmutableTrieDictionary<Address, IValue>.Empty),
        };
        return actions;
    }
}
