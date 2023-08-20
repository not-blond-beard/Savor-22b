namespace Savor22b.GraphTypes.Query;

using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
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
using Savor22b.GraphTypes.Types;
using Savor22b.Model;

public class Query : ObjectGraphType
{
    [SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1118:ParameterMustNotSpanMultipleLines",
        Justification = "GraphQL docs require long lines of text."
    )]
    private readonly BlockChain _blockChain;
    private readonly Swarm _swarm;

    public Query(BlockChain blockChain, Swarm? swarm = null)
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
                    : swarm.AsPeer.PeerString
        );

        Field<ListGraphType<RecipeGraphType.RecipeResponseType>>(
            "recipe",
            resolve: context =>
            {
                var recipes = combineRecipeData();

                return recipes;
            }
        );

        Field<NonNullGraphType<StringGraphType>>(
            "createAction_PlaceUserHouse",
            description: "Moves or installs a user's house to a specific location. If the user already has a house installed, it is considered a relocation, incurring costs and time",
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
                var publicKey = new PublicKey(
                    ByteUtil.ParseHex(context.GetArgument<string>("publicKey"))
                );

                var action = new PlaceUserHouseAction(
                    context.GetArgument<int>("villageId"),
                    context.GetArgument<int>("x"),
                    context.GetArgument<int>("y")
                );

                return new GetUnsignedTransactionHex(
                    action,
                    publicKey,
                    _blockChain,
                    _swarm
                ).UnsignedTransactionHex;
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
                    Name = "refrigeratorStateIdsToUse",
                    Description = "refrigerator state ID list",
                },
                new QueryArgument<NonNullGraphType<ListGraphType<GuidGraphType>>>
                {
                    Name = "kitchenEquipmentStateIdsToUse",
                    Description = "kitchen equipment state ID list",
                },
                new QueryArgument<NonNullGraphType<ListGraphType<IntGraphType>>>
                {
                    Name = "applianceSpaceNumbersToUse",
                    Description = "appliance space number list",
                }
            ),
            resolve: context =>
            {
                var publicKey = new PublicKey(
                    ByteUtil.ParseHex(context.GetArgument<string>("publicKey"))
                );

                var action = new CreateFoodAction(
                    context.GetArgument<int>("recipeID"),
                    Guid.NewGuid(),
                    context.GetArgument<List<Guid>>("refrigeratorStateIDs"),
                    context.GetArgument<List<Guid>>("kitchenEquipmentStateIdsToUse"),
                    context.GetArgument<List<int>>("applianceSpaceNumbersToUse")
                );

                return new GetUnsignedTransactionHex(
                    action,
                    publicKey,
                    _blockChain,
                    _swarm
                ).UnsignedTransactionHex;
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
                var publicKey = new PublicKey(
                    ByteUtil.ParseHex(context.GetArgument<string>("publicKey"))
                );

                var action = new UseRandomSeedItemAction(
                    Guid.NewGuid(),
                    context.GetArgument<Guid>("itemStateID")
                );

                return new GetUnsignedTransactionHex(
                    action,
                    publicKey,
                    _blockChain,
                    _swarm
                ).UnsignedTransactionHex;
            }
        );

        Field<NonNullGraphType<StringGraphType>>(
            "createAction_BuyKitchenEquipment",
            description: "Buy kitchen equipment",
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
                var publicKey = new PublicKey(
                    ByteUtil.ParseHex(context.GetArgument<string>("publicKey"))
                );

                var action = new BuyKitchenEquipmentAction(
                    Guid.NewGuid(),
                    context.GetArgument<int>("desiredEquipmentID")
                );

                return new GetUnsignedTransactionHex(
                    action,
                    publicKey,
                    _blockChain,
                    _swarm
                ).UnsignedTransactionHex;
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
                var publicKey = new PublicKey(
                    ByteUtil.ParseHex(context.GetArgument<string>("publicKey"))
                );

                var action = new PlantingSeedAction(
                    context.GetArgument<Guid>("seedStateId"),
                    context.GetArgument<int>("fieldIndex")
                );

                return new GetUnsignedTransactionHex(
                    action,
                    publicKey,
                    _blockChain,
                    _swarm
                ).UnsignedTransactionHex;
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
                var publicKey = new PublicKey(
                    ByteUtil.ParseHex(context.GetArgument<string>("publicKey"))
                );

                var action = new RemovePlantedSeedAction(context.GetArgument<int>("fieldIndex"));

                return new GetUnsignedTransactionHex(
                    action,
                    publicKey,
                    _blockChain,
                    _swarm
                ).UnsignedTransactionHex;
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
                var publicKey = new PublicKey(
                    ByteUtil.ParseHex(context.GetArgument<string>("publicKey"))
                );

                var action = new RemoveWeedAction(context.GetArgument<int>("fieldIndex"));

                return new GetUnsignedTransactionHex(
                    action,
                    publicKey,
                    _blockChain,
                    _swarm
                ).UnsignedTransactionHex;
            }
        );

        Field<NonNullGraphType<StringGraphType>>(
            "createAction_HarvestingSeed",
            description: "Harvesting Seed",
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
                var publicKey = new PublicKey(
                    ByteUtil.ParseHex(context.GetArgument<string>("publicKey"))
                );

                var action = new HarvestingSeedAction(
                    context.GetArgument<int>("fieldIndex"),
                    Guid.NewGuid()
                );

                return new GetUnsignedTransactionHex(
                    action,
                    publicKey,
                    _blockChain,
                    _swarm
                ).UnsignedTransactionHex;
            }
        );

        Field<NonNullGraphType<StringGraphType>>(
            "createAction_CreateFood",
            description: "Create Food",
            arguments: new QueryArguments(
                new QueryArgument<NonNullGraphType<StringGraphType>>
                {
                    Name = "publicKey",
                    Description = "The base64-encoded public key for Transaction.",
                },
                new QueryArgument<NonNullGraphType<ListGraphType<GuidGraphType>>>
                {
                    Name = "refrigeratorStateIdsToUse",
                    Description = "refrigerator state ID list for use",
                },
                new QueryArgument<NonNullGraphType<ListGraphType<GuidGraphType>>>
                {
                    Name = "kitchenEquipmentStateIdsToUse",
                    Description = "kitchen equipment state ID list for use",
                },
                new QueryArgument<NonNullGraphType<ListGraphType<IntGraphType>>>
                {
                    Name = "applianceSpaceNumbersToUse",
                    Description = "appliance space number list for use",
                }
            ),
            resolve: context =>
            {
                var publicKey = new PublicKey(
                    ByteUtil.ParseHex(context.GetArgument<string>("publicKey"))
                );

                var action = new CreateFoodAction(
                    context.GetArgument<int>("recipeID"),
                    Guid.NewGuid(),
                    context.GetArgument<List<Guid>>("refrigeratorStateIdsToUse"),
                    context.GetArgument<List<Guid>>("kitchenEquipmentStateIdsToUse"),
                    context.GetArgument<List<int>>("applianceSpaceNumbersToUse")
                );

                return new GetUnsignedTransactionHex(
                    action,
                    publicKey,
                    _blockChain,
                    _swarm
                ).UnsignedTransactionHex;
            }
        );

        Field<NonNullGraphType<StringGraphType>>(
            "createAction_BuyShopItem",
            description: "Buy Shop Item",
            arguments: new QueryArguments(
                new QueryArgument<NonNullGraphType<StringGraphType>>
                {
                    Name = "publicKey",
                    Description = "The base64-encoded public key for Transaction.",
                },
                new QueryArgument<NonNullGraphType<IntGraphType>>
                {
                    Name = "desiredShopItemID",
                    Description = "Desired Shop Item ID",
                }
            ),
            resolve: context =>
            {
                var publicKey = new PublicKey(
                    ByteUtil.ParseHex(context.GetArgument<string>("publicKey"))
                );

                var action = new BuyShopItemAction(
                    Guid.NewGuid(),
                    context.GetArgument<int>("desiredShopItemID")
                );

                return new GetUnsignedTransactionHex(
                    action,
                    publicKey,
                    _blockChain,
                    _swarm
                ).UnsignedTransactionHex;
            }
        );

        AddField(new CalculateRelocationCostQuery());
        AddField(new VillagesQuery(blockChain));
        AddField(new ShopQuery());
    }

    private List<RecipeResponse> combineRecipeData()
    {
        var foodDict = CsvDataHelper.GetFoodCSVData().ToDictionary(x => x.ID);
        var ingredientDict = CsvDataHelper.GetIngredientCSVData().ToDictionary(x => x.ID);

        var recipes = new List<RecipeResponse>();
        foreach (var recipe in CsvDataHelper.GetRecipeCSVData())
        {
            var recipeIngredientComponents = recipe.IngredientIDList
                .Select(
                    ingredientID =>
                        new RecipeComponent(ingredientID, ingredientDict[ingredientID].Name)
                )
                .ToList();
            var recipeFoodComponents = recipe.FoodIDList
                .Select(foodID => new RecipeComponent(foodID, foodDict[foodID].Name))
                .ToList();

            recipes.Add(
                new RecipeResponse(
                    recipe.ID,
                    recipe.Name,
                    recipeIngredientComponents,
                    recipeFoodComponents
                )
            );
        }

        return recipes;
    }
}
