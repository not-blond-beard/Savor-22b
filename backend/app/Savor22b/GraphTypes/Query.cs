namespace Savor22b.GraphTypes;

using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Bencodex.Types;
using GraphQL;
using GraphQL.Types;
using Libplanet;
using Libplanet.Action;
using Libplanet.Assets;
using Libplanet.Blockchain;
using Libplanet.Crypto;
using Libplanet.Net;
using Libplanet.Tx;
using Savor22b.Action;
using Savor22b.Helpers;
using Savor22b.Model;

public class Query : ObjectGraphType
{
    [SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1118:ParameterMustNotSpanMultipleLines",
        Justification = "GraphQL docs require long lines of text.")]


    private readonly BlockChain _blockChain;
    private readonly Swarm _swarm;

    public Query(
        BlockChain blockChain,
        Swarm? swarm = null)
    {
        _blockChain = blockChain;
        _swarm = swarm;

        Field<StringGraphType>(
            "asset",
            description: "The specified address's balance in MNT.",
            arguments: new QueryArguments(
                new QueryArgument<NonNullGraphType<StringGraphType>>
                {
                    Name = "address",
                    Description = "The account holder's 40-hex address",
                }
            ),
            resolve: context =>
            {
                var accountAddress = new Address(context.GetArgument<string>("address"));
                FungibleAssetValue asset = blockChain.GetBalance(
                    accountAddress,
                    Currencies.KeyCurrency
                );

                return asset.ToString();
            }
        );

        // TODO: Move to Libplanet.Explorer or Node API.
        Field<StringGraphType>(
            "peerString",
            resolve: context =>
                swarm is null
                ? throw new InvalidOperationException("Network settings is not set.")
                : swarm.AsPeer.PeerString);

        Field<ListGraphType<RecipeGraphType.RecipeType>>(
            "recipe",
            resolve: context =>
                {
                    CsvParser<RecipeReference> recipeCsvParser = new CsvParser<RecipeReference>();
                    var recipeList = recipeCsvParser.ParseCsv(Paths.GetCSVDataPath("recipe.csv"));

                    CsvParser<RecipeStat> recipeStatCsvParser = new CsvParser<RecipeStat>();
                    var recipeStatList = recipeStatCsvParser.ParseCsv(Paths.GetCSVDataPath("recipe_stat.csv"));

                    var recipes = GetRecipeList(recipeList, recipeStatList);

                    return recipes;
                }
        );


        Field<NonNullGraphType<StringGraphType>>(
            "createAction_PlaceUserHouse",
            description: "Place User House",
            arguments: new QueryArguments(
                new QueryArgument<NonNullGraphType<StringGraphType>>
                {
                    Name = "publicKey",
                    Description = "The base64-encoded public key for Transaction.",
                },
                new QueryArgument<NonNullGraphType<IntGraphType>>
                {
                    Name = "villageId",
                    Description = "ID of the village you want to move to",
                },
                new QueryArgument<NonNullGraphType<IntGraphType>>
                {
                    Name = "x",
                    Description = "x coordinate of the house",
                },
                new QueryArgument<NonNullGraphType<IntGraphType>>
                {
                    Name = "y",
                    Description = "y coordinate of the house",
                }
            ),
            resolve: context =>
            {
                var publicKey = new PublicKey(ByteUtil.ParseHex(context.GetArgument<string>("publicKey")));

                var action = new PlaceUserHouseAction(
                    context.GetArgument<int>("villageId"),
                    context.GetArgument<int>("x"),
                    context.GetArgument<int>("y")
                );

                return getUnsignedTransactionHex(action, publicKey);
            }
        );

        Field<NonNullGraphType<StringGraphType>>(
            "createAction_GenerateIngredient",
            description: "Generate Ingredient",
            arguments: new QueryArguments(
                new QueryArgument<NonNullGraphType<StringGraphType>>
                {
                    Name = "publicKey",
                    Description = "The base64-encoded public key for Transaction.",
                },
                new QueryArgument<NonNullGraphType<GuidGraphType>>
                {
                    Name = "seedStateId",
                    Description = "Seed state Id (PK)",
                }
            ),
            resolve: context =>
            {
                var publicKey = new PublicKey(ByteUtil.ParseHex(context.GetArgument<string>("publicKey")));

                var action = new GenerateIngredientAction(
                    context.GetArgument<Guid>("seedStateId"),
                    Guid.NewGuid()
                );

                return getUnsignedTransactionHex(action, publicKey);
            }
        );

        Field<NonNullGraphType<StringGraphType>>(
            "createAction_GenerateFood",
            description: "Generate Food",
            arguments: new QueryArguments(
                new QueryArgument<NonNullGraphType<StringGraphType>>
                {
                    Name = "publicKey",
                    Description = "The base64-encoded public key for Transaction.",
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
                var publicKey = new PublicKey(ByteUtil.ParseHex(context.GetArgument<string>("publicKey")));

                var action = new GenerateFoodAction(
                    context.GetArgument<int>("recipeID"),
                    Guid.NewGuid(),
                    context.GetArgument<List<Guid>>("refrigeratorStateIDs")
                );

                return getUnsignedTransactionHex(action, publicKey);
            }
        );

        Field<NonNullGraphType<StringGraphType>>(
            "createAction_UseRandomSeedItem",
            description: "Use Random Seed Item",
            arguments: new QueryArguments(
                new QueryArgument<NonNullGraphType<StringGraphType>>
                {
                    Name = "publicKey",
                    Description = "The base64-encoded public key for Transaction.",
                },
                new QueryArgument<NonNullGraphType<GuidGraphType>>
                {
                    Name = "itemStateID",
                    Description = "item state id to use",
                }
            ),
            resolve: context =>
            {
                var publicKey = new PublicKey(ByteUtil.ParseHex(context.GetArgument<string>("publicKey")));

                var action = new UseRandomSeedItemAction(
                    Guid.NewGuid(),
                    context.GetArgument<Guid>("itemStateID")
                );

                return getUnsignedTransactionHex(action, publicKey);
            }
        );

        Field<NonNullGraphType<StringGraphType>>(
            "createAction_BuyCookingEquipment",
            description: "Buy cooking equipment",
            arguments: new QueryArguments(
                new QueryArgument<NonNullGraphType<StringGraphType>>
                {
                    Name = "publicKey",
                    Description = "The base64-encoded public key for Transaction.",
                },
                new QueryArgument<NonNullGraphType<IntGraphType>>
                {
                    Name = "desiredEquipmentID",
                    Description = "Desired Equipment ID",
                }
            ),
            resolve: context =>
            {
                var publicKey = new PublicKey(ByteUtil.ParseHex(context.GetArgument<string>("publicKey")));

                var action = new BuyCookingEquipmentAction(
                    Guid.NewGuid(),
                    context.GetArgument<int>("desiredEquipmentID")
                );

                return getUnsignedTransactionHex(action, publicKey);
            }
        );

        Field<NonNullGraphType<StringGraphType>>(
            "createAction_PlantingSeed",
            description: "Planting Seed",
            arguments: new QueryArguments(
                new QueryArgument<NonNullGraphType<StringGraphType>>
                {
                    Name = "publicKey",
                    Description = "The base64-encoded public key for Transaction.",
                },
                new QueryArgument<NonNullGraphType<GuidGraphType>>
                {
                    Name = "seedStateId",
                    Description = "Seed state Id (Guid)",
                },
                new QueryArgument<NonNullGraphType<IntGraphType>>
                {
                    Name = "fieldIndex",
                    Description = "Target field Index",
                }
            ),
            resolve: context =>
            {
                var publicKey = new PublicKey(ByteUtil.ParseHex(context.GetArgument<string>("publicKey")));

                var action = new PlantingSeedAction(
                    context.GetArgument<Guid>("seedStateId"),
                    context.GetArgument<int>("fieldIndex")
                );

                return getUnsignedTransactionHex(action, publicKey);
            }
        );


        Field<NonNullGraphType<StringGraphType>>(
            "createAction_RemovePlantedSeed",
            description: "Remove Planted Seed",
            arguments: new QueryArguments(
                new QueryArgument<NonNullGraphType<StringGraphType>>
                {
                    Name = "publicKey",
                    Description = "The base64-encoded public key for Transaction.",
                },
                new QueryArgument<NonNullGraphType<IntGraphType>>
                {
                    Name = "fieldIndex",
                    Description = "Target field Index",
                }
            ),
            resolve: context =>
            {
                var publicKey = new PublicKey(ByteUtil.ParseHex(context.GetArgument<string>("publicKey")));

                var action = new RemovePlantedSeedAction(
                    context.GetArgument<int>("fieldIndex")
                );

                return getUnsignedTransactionHex(action, publicKey);
            }
        );

        Field<NonNullGraphType<StringGraphType>>(
            "createAction_RemoveWeed",
            description: "Remove Weed",
            arguments: new QueryArguments(
                new QueryArgument<NonNullGraphType<StringGraphType>>
                {
                    Name = "publicKey",
                    Description = "The base64-encoded public key for Transaction.",
                },
                new QueryArgument<NonNullGraphType<IntGraphType>>
                {
                    Name = "fieldIndex",
                    Description = "Target field Index",
                }
            ),
            resolve: context =>
            {
                var publicKey = new PublicKey(ByteUtil.ParseHex(context.GetArgument<string>("publicKey")));

                var action = new RemoveWeedAction(
                    context.GetArgument<int>("fieldIndex")
                );

                return getUnsignedTransactionHex(action, publicKey);
            }
        );
    }

    private string getUnsignedTransactionHex(IAction action, PublicKey publicKey)
    {
        Address signer = publicKey.ToAddress();
        TxActionList txActionList = new(new[] { action });

        long nonce = _blockChain.GetNextTxNonce(signer);

        TxInvoice invoice = new TxInvoice(
            genesisHash: _blockChain.Genesis.Hash,
            actions: txActionList
        );
        UnsignedTx unsignedTransaction =
                new UnsignedTx(invoice, new TxSigningMetadata(publicKey, nonce));

        byte[] unsignedTransactionString = unsignedTransaction.SerializeUnsignedTx().ToArray();
        string unsignedTransactionHex = ByteUtil.Hex(unsignedTransactionString);

        return unsignedTransactionHex;
    }


    private List<Recipe> GetRecipeList(List<RecipeReference> recipeList, List<RecipeStat> recipeStatList)
    {
        var recipeStatDictionary = CreateRecipeStatDictionary(recipeStatList);

        var recipes = recipeList.GroupBy(recipe => recipe.ID)
            .Select(group => CreateRecipe(group.Key, group.ToList(), recipeStatDictionary))
            .ToList();

        return recipes;
    }

    private Dictionary<int, RecipeStat> CreateRecipeStatDictionary(List<RecipeStat> recipeStatList)
    {
        return recipeStatList.ToDictionary(rs => rs.ID);
    }

    private Recipe CreateRecipe(int recipeId, List<RecipeReference> recipeReferences, Dictionary<int, RecipeStat> recipeStatDictionary)
    {
        var ingredients = recipeReferences.Select(recipeReference =>
        {
            var id = recipeReference.IngredientID ?? recipeReference.ReferencedRecipeID!.Value;
            var name = recipeReference.IngredientID != null
                ? recipeReference.IngredientName!
                : recipeReference.ReferencedRecipeName!;
            var type = recipeReference.IngredientID != null ? "ingredient" : "food";
            return new RecipeIngredient(id, name, type);
        }).ToList();

        var recipeStat = recipeStatDictionary[recipeId];

        return new Recipe(
            recipeId,
            recipeStat.Name,
            ingredients,
            recipeStat.MinGrade,
            recipeStat.MaxGrade
        );
    }
}
