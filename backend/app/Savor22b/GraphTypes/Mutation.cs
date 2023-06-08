namespace Savor22b.GraphTypes;

using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using GraphQL;
using GraphQL.Types;
using Libplanet;
using Libplanet.Assets;
using Libplanet.Blockchain;
using Libplanet.Crypto;
using Libplanet.Explorer.GraphTypes;
using Libplanet.Net;
using Libplanet.Tx;
using Savor22b.Action;

public class Mutation : ObjectGraphType
{
    [SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1118:ParameterMustNotSpanMultipleLines",
        Justification = "GraphQL docs require long lines of text.")]
    public Mutation(
        BlockChain blockChain,
        Swarm? swarm = null
    )
    {
        Field<TransactionType>(
            "stage",
            description: "Stage transaction to current chain",
            arguments: new QueryArguments(
                new QueryArgument<NonNullGraphType<ByteStringType>>
                {
                    Name = "payloadHex",
                    Description = "The hexadecimal string of the serialized transaction to stage.",
                }
            ),
            resolve: context =>
            {
                string payloadHex = context.GetArgument<string>("payloadHex");
                byte[] payload = ByteUtil.ParseHex(payloadHex);
                var tx = Transaction.Deserialize(payload);
                blockChain.StageTransaction(tx);
                swarm?.BroadcastTxs(new[] { tx });
                return tx;
            }
        );

        // TODO: This mutation should be upstreamed to Libplanet.Explorer so that any native tokens
        // can work together with this mutation:
        Field<TransactionType>(
            "transferAsset",
            description: "Transfers the given amount of MNT from the account of the specified " +
                "privateKeyHex to the specified recipient.  The transaction is signed using " +
                "the privateKeyHex and added to the stage (and eventually included in one of " +
                "the next blocks).",
            arguments: new QueryArguments(
                new QueryArgument<NonNullGraphType<StringGraphType>>
                {
                    Name = "recipient",
                    Description = "The recipient's 40-hex address.",
                },
                new QueryArgument<NonNullGraphType<StringGraphType>>
                {
                    Name = "amount",
                    Description = "The amount to transfer in MNT.",
                },
                new QueryArgument<NonNullGraphType<StringGraphType>>
                {
                    Name = "privateKeyHex",
                    Description = "A hex-encoded private key of the sender.  A made " +
                        "transaction will be signed using this key.",
                }
            ),
            resolve: context =>
            {
                Address recipient = new Address(context.GetArgument<string>("recipient"));
                string amount = context.GetArgument<string>("amount");
                string privateKeyHex = context.GetArgument<string>("privateKeyHex");

                PrivateKey privateKey = PrivateKey.FromString(privateKeyHex);
                var action = new Transfer(
                    recipient,
                    FungibleAssetValue.Parse(
                        Currencies.KeyCurrency,
                        amount
                    )
                );
                var actionList = new List<SVRAction>();
                actionList.Add(action);

                var tx = blockChain.MakeTransaction(
                    privateKey,
                    actionList,
                    ImmutableHashSet<Address>.Empty
                        .Add(privateKey.ToAddress())
                        .Add(recipient));
                swarm?.BroadcastTxs(new[] { tx });
                return tx;
            }
        );

        // TODO: This mutation should be upstreamed to Libplanet.Explorer so that any native tokens
        // can work together with this mutation:
        Field<TransactionType>(
            "mintAsset",
            description: "Mints the given amount of MNT to the balance of the specified " +
                "recipient. The transaction is signed using the privateKeyHex and added to " +
                "the stage (and eventually included in one of the next blocks).",
            arguments: new QueryArguments(
                new QueryArgument<NonNullGraphType<StringGraphType>>
                {
                    Name = "recipient",
                    Description = "The recipient's 40-hex address.",
                },
                new QueryArgument<NonNullGraphType<StringGraphType>>
                {
                    Name = "amount",
                    Description = "The amount to mint in MNT.",
                },
                new QueryArgument<NonNullGraphType<StringGraphType>>
                {
                    Name = "privateKeyHex",
                    Description = "A hex-encoded private key of the minter.  A made " +
                        "transaction will be signed using this key.",
                }
            ),
            resolve: context =>
            {
                Address recipient = new Address(context.GetArgument<string>("recipient"));
                string amount = context.GetArgument<string>("amount");
                string privateKeyHex = context.GetArgument<string>("privateKeyHex");

                PrivateKey privateKey = PrivateKey.FromString(privateKeyHex);
                var action = new Mint(
                    recipient,
                    FungibleAssetValue.Parse(
                        Currencies.KeyCurrency,
                        amount
                    )
                );
                var actionList = new List<SVRAction>();
                actionList.Add(action);

                var tx = blockChain.MakeTransaction(
                    privateKey,
                    actionList,
                    ImmutableHashSet<Address>.Empty
                        .Add(privateKey.ToAddress())
                        .Add(recipient));
                swarm?.BroadcastTxs(new[] { tx });
                return tx;
            }
        );

        // TODO: This mutation should be upstreamed to Libplanet.Explorer so that any native tokens
        // can work together with this mutation:
        Field<TransactionType>(
            "generateSeed",
            description: "Generate Seed",
            arguments: new QueryArguments(
                new QueryArgument<NonNullGraphType<StringGraphType>>
                {
                    Name = "privateKeyHex",
                    Description = "A hex-encoded private key of the minter.  A made " +
                        "transaction will be signed using this key.",
                }
            ),
            resolve: context =>
            {
                string privateKeyHex = context.GetArgument<string>("privateKeyHex");

                PrivateKey privateKey = PrivateKey.FromString(privateKeyHex);

                var actionList = new List<SVRAction>();
                var action = new GenerateSeedAction(Guid.NewGuid());

                actionList.Add(action);

                var tx = blockChain.MakeTransaction(privateKey, actionList);

                swarm?.BroadcastTxs(new[] { tx });

                return tx;
            }
        );

        Field<TransactionType>(
            "generateIngredient",
            description: "Generate Ingredient",
            arguments: new QueryArguments(
                new QueryArgument<NonNullGraphType<StringGraphType>>
                {
                    Name = "privateKeyHex",
                    Description = "A hex-encoded private key of the minter.  A made " +
                        "transaction will be signed using this key.",
                },
                new QueryArgument<NonNullGraphType<GuidGraphType>>
                {
                    Name = "seedStateId",
                    Description = "Seed state Id (PK)",
                }
            ),
            resolve: context =>
            {
                string privateKeyHex = context.GetArgument<string>("privateKeyHex");

                PrivateKey privateKey = PrivateKey.FromString(privateKeyHex);

                var actionList = new List<SVRAction>();
                var action = new GenerateIngredientAction(
                    context.GetArgument<Guid>("seedStateId"),
                    Guid.NewGuid()
                );

                actionList.Add(action);

                var tx = blockChain.MakeTransaction(privateKey, actionList);

                swarm?.BroadcastTxs(new[] { tx });

                return tx;
            }
        );

        Field<TransactionType>(
            "generateFood",
            description: "Generate Food",
            arguments: new QueryArguments(
                new QueryArgument<NonNullGraphType<StringGraphType>>
                {
                    Name = "privateKeyHex",
                    Description = "A hex-encoded private key of the minter.  A made " +
                        "transaction will be signed using this key.",
                },
                new QueryArgument<NonNullGraphType<IntGraphType>>
                {
                    Name = "recipeID",
                    Description = "Recipe ID",
                },
                new QueryArgument<NonNullGraphType<ListGraphType<GuidGraphType>>>
                {
                    Name = "refrigeratorStateIDs",
                    Description = "refrigerator state ID list",
                }
            ),
            resolve: context =>
            {
                string privateKeyHex = context.GetArgument<string>("privateKeyHex");

                PrivateKey privateKey = PrivateKey.FromString(privateKeyHex);

                var action = new GenerateFoodAction(
                    context.GetArgument<int>("recipeID"),
                    Guid.NewGuid(),
                    context.GetArgument<List<Guid>>("refrigeratorStateIDs")
                );

                var tx = blockChain.MakeTransaction(
                    privateKey, new List<SVRAction> { action });

                swarm?.BroadcastTxs(new[] { tx });

                return tx;
            }
        );
    }
}
