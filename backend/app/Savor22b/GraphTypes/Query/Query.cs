namespace Savor22b.GraphTypes.Query;

using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using GraphQL;
using GraphQL.Types;
using Libplanet;
using Libplanet.Assets;
using Libplanet.Blockchain;
using Libplanet.Crypto;
using Libplanet.Net;
using Savor22b.Action;
using Savor22b.GraphTypes.Types;
using Savor22b.GraphTypes.Subscription;
using Savor22b.States;
using System.Reactive.Subjects;

public class Query : ObjectGraphType
{
    [SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1118:ParameterMustNotSpanMultipleLines",
        Justification = "GraphQL docs require long lines of text."
    )]
    private readonly BlockChain _blockChain;
    private readonly Swarm _swarm;

    public Query(
        BlockChain blockChain,
        Swarm? swarm = null,
        Subject<Libplanet.Blocks.BlockHash>? subject = null
    )
    {
        _blockChain = blockChain;
        _swarm = swarm;

        Field<UserStateType>(
            "userState",
            description: "The specified address's user state",
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
                var rootStateEncoded = blockChain.GetState(accountAddress);
                RootState rootState = rootStateEncoded is Bencodex.Types.Dictionary bdict
                    ? new RootState(bdict)
                    : new RootState();
                return rootState;
            }
        );

        Field<TradeInventoryStateType>(
            "tradeInventoryState",
            description: "무역상점 정보 조회",
            arguments: new QueryArguments(),
            resolve: context =>
            {
                return TradeInventoryStateField.GetTradeInventoryState(blockChain);
            }
        );

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
            "createAction_CreateFood",
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
                    context.GetArgument<List<Guid>>("kitchenEquipmentStateIdsToUse")
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
                new QueryArgument<NonNullGraphType<IntGraphType>>
                {
                    Name = "fieldIndex",
                    Description = "Target field Index",
                },
                new QueryArgument<NonNullGraphType<GuidGraphType>>
                {
                    Name = "itemStateIdToUse",
                    Description = "Item state id to use (Guid)",
                }
            ),
            resolve: context =>
            {
                var publicKey = new PublicKey(
                    ByteUtil.ParseHex(context.GetArgument<string>("publicKey"))
                );

                var action = new PlantingSeedAction(
                    Guid.NewGuid(),
                    context.GetArgument<int>("fieldIndex"),
                    context.GetArgument<Guid>("itemStateIdToUse")
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

        Field<NonNullGraphType<StringGraphType>>(
            "createAction_InstallKitchenEquipmentAction",
            description: "Install Kitchen Equipment Action",
            arguments: new QueryArguments(
                new QueryArgument<NonNullGraphType<StringGraphType>>
                {
                    Name = "publicKey",
                    Description = "The base64-encoded public key for Transaction.",
                },
                new QueryArgument<GuidGraphType>
                {
                    Name = "kitchenEquipmentStateID",
                    Description = "Kitchen Equipment State Id for install",
                },
                new QueryArgument<IntGraphType>
                {
                    Name = "spaceNumber",
                    Description = "Target space number to install kitchen equipment",
                }
            ),
            resolve: context =>
            {
                var publicKey = new PublicKey(
                    ByteUtil.ParseHex(context.GetArgument<string>("publicKey"))
                );

                var action = new InstallKitchenEquipmentAction(
                    context.GetArgument<Guid>("kitchenEquipmentStateID"),
                    context.GetArgument<int>("spaceNumber")
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
            "createAction_CancelFoodAction",
            description: "Cancel Food",
            arguments: new QueryArguments(
                new QueryArgument<NonNullGraphType<StringGraphType>>
                {
                    Name = "publicKey",
                    Description = "The base64-encoded public key for Transaction.",
                },
                new QueryArgument<NonNullGraphType<GuidGraphType>>
                {
                    Name = "foodStateId",
                    Description = "Food state Id (Guid)",
                }
            ),
            resolve: context =>
            {
                var publicKey = new PublicKey(
                    ByteUtil.ParseHex(context.GetArgument<string>("publicKey"))
                );

                var action = new CancelFoodAction(context.GetArgument<Guid>("foodStateId"));

                return new GetUnsignedTransactionHex(
                    action,
                    publicKey,
                    _blockChain,
                    _swarm
                ).UnsignedTransactionHex;
            }
        );

        Field<NonNullGraphType<StringGraphType>>(
            "createAction_UseLifeStoneAction",
            description: "Use LifeStone",
            arguments: new QueryArguments(
                new QueryArgument<NonNullGraphType<StringGraphType>>
                {
                    Name = "publicKey",
                    Description = "The base64-encoded public key for Transaction.",
                },
                new QueryArgument<NonNullGraphType<GuidGraphType>>
                {
                    Name = "foodStateId",
                    Description = "Food state Id (Guid)",
                }
            ),
            resolve: context =>
            {
                var publicKey = new PublicKey(
                    ByteUtil.ParseHex(context.GetArgument<string>("publicKey"))
                );

                var action = new UseLifeStoneAction(context.GetArgument<Guid>("foodStateId"));

                return new GetUnsignedTransactionHex(
                    action,
                    publicKey,
                    _blockChain,
                    _swarm
                ).UnsignedTransactionHex;
            }
        );

        Field<NonNullGraphType<StringGraphType>>(
            "createAction_LevelUpAction",
            description: "Super Food LevelUp",
            arguments: new QueryArguments(
                new QueryArgument<NonNullGraphType<StringGraphType>>
                {
                    Name = "publicKey",
                    Description = "The base64-encoded public key for Transaction.",
                },
                new QueryArgument<NonNullGraphType<GuidGraphType>>
                {
                    Name = "foodStateId",
                    Description = "Food state Id (Guid)",
                }
            ),
            resolve: context =>
            {
                var publicKey = new PublicKey(
                    ByteUtil.ParseHex(context.GetArgument<string>("publicKey"))
                );

                var action = new LevelUpAction(context.GetArgument<Guid>("foodStateId"));

                return new GetUnsignedTransactionHex(
                    action,
                    publicKey,
                    _blockChain,
                    _swarm
                ).UnsignedTransactionHex;
            }
        );

        Field<NonNullGraphType<StringGraphType>>(
            "createAction_CancelRegisteredTradeGoodAction",
            description: "무역상점 상품 등록 취소",
            arguments: new QueryArguments(
                new QueryArgument<NonNullGraphType<StringGraphType>>
                {
                    Name = "publicKey",
                    Description = "The base64-encoded public key for Transaction.",
                },
                new QueryArgument<NonNullGraphType<GuidGraphType>>
                {
                    Name = "productId",
                    Description = "상품 고유 Id",
                }
            ),
            resolve: context =>
            {
                var publicKey = new PublicKey(
                    ByteUtil.ParseHex(context.GetArgument<string>("publicKey"))
                );

                var action = new CancelRegisteredTradeGoodAction(
                    context.GetArgument<Guid>("productId")
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
            "createAction_UpdateTradeGoodAction",
            description: "무역상점 상품 가격 수정",
            arguments: new QueryArguments(
                new QueryArgument<NonNullGraphType<StringGraphType>>
                {
                    Name = "publicKey",
                    Description = "The base64-encoded public key for Transaction.",
                },
                new QueryArgument<NonNullGraphType<GuidGraphType>>
                {
                    Name = "productId",
                    Description = "상품 고유 Id",
                },
                new QueryArgument<NonNullGraphType<IntGraphType>>
                {
                    Name = "price",
                    Description = "가격",
                }
            ),
            resolve: context =>
            {
                var publicKey = new PublicKey(
                    ByteUtil.ParseHex(context.GetArgument<string>("publicKey"))
                );
                var price = FungibleAssetValue.Parse(
                    Currencies.KeyCurrency,
                    context.GetArgument<int>("price").ToString()
                );

                var action = new UpdateTradeGoodAction(
                    context.GetArgument<Guid>("productId"),
                    price
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
            "createAction_BuyTradeGoodAction",
            description: "무역상점 구매",
            arguments: new QueryArguments(
                new QueryArgument<NonNullGraphType<StringGraphType>>
                {
                    Name = "publicKey",
                    Description = "The base64-encoded public key for Transaction.",
                },
                new QueryArgument<NonNullGraphType<GuidGraphType>>
                {
                    Name = "productId",
                    Description = "상품 고유 Id",
                }
            ),
            resolve: context =>
            {
                var publicKey = new PublicKey(
                    ByteUtil.ParseHex(context.GetArgument<string>("publicKey"))
                );

                var action = new BuyTradeGoodAction(context.GetArgument<Guid>("productId"));

                return new GetUnsignedTransactionHex(
                    action,
                    publicKey,
                    _blockChain,
                    _swarm
                ).UnsignedTransactionHex;
            }
        );

        Field<NonNullGraphType<StringGraphType>>(
            "createAction_RegisterTradeGoodAction",
            description: "Register Trade Good to Trade Store",
            arguments: new QueryArguments(
                new QueryArgument<NonNullGraphType<StringGraphType>>
                {
                    Name = "publicKey",
                    Description = "The base64-encoded public key for Transaction.",
                },
                new QueryArgument<NonNullGraphType<IntGraphType>>
                {
                    Name = "price",
                    Description = "Price for good",
                },
                new QueryArgument<GuidGraphType>
                {
                    Name = "foodStateId",
                    Description = "Food state Id (Guid)",
                },
                new QueryArgument<ListGraphType<GuidGraphType>>
                {
                    Name = "itemStateIds",
                    Description = "Item state Ids (Guid)",
                }
            ),
            resolve: context =>
            {
                var publicKey = new PublicKey(
                    ByteUtil.ParseHex(context.GetArgument<string>("publicKey"))
                );
                var foodStateId = context.GetArgument<Guid?>("foodStateId");
                var itemStateIds = context.GetArgument<List<Guid>?>("itemStateIds");
                var price = FungibleAssetValue.Parse(
                    Currencies.KeyCurrency,
                    context.GetArgument<int>("price").ToString()
                );

                if (foodStateId is not null)
                {
                    return new GetUnsignedTransactionHex(
                        new RegisterTradeGoodAction(price, foodStateId.Value),
                        publicKey,
                        _blockChain,
                        _swarm
                    ).UnsignedTransactionHex;
                }
                else if (itemStateIds is not null)
                {
                    return new GetUnsignedTransactionHex(
                        new RegisterTradeGoodAction(price, itemStateIds.ToImmutableList()),
                        publicKey,
                        _blockChain,
                        _swarm
                    ).UnsignedTransactionHex;
                }
                else
                {
                    throw new ArgumentException("foodStateId or itemStateIds required");
                }
            }
        );

        AddField(new DungeonExplorationQuery(blockChain, swarm));
        AddField(new CalculateRelocationCostQuery());
        AddField(new DungeonReturnRewardQuery());
        AddField(new ShopQuery());
        AddField(new VillageField(blockChain, subject));
        AddField(new ShowMeTheMoney(blockChain, swarm));
        AddField(new ConquestDungeonActionQuery(blockChain, swarm));
        AddField(new RemoveInstalledKitchenEquipmentActionQuery(blockChain, swarm));
    }

    private List<RecipeResponse> combineRecipeData()
    {
        var foodDict = CsvDataHelper.GetFoodCSVData().ToDictionary(x => x.ID);
        var ingredientDict = CsvDataHelper.GetIngredientCSVData().ToDictionary(x => x.ID);
        var kitchenEquipmentCategoryDict = CsvDataHelper
            .GetKitchenEquipmentCategoryCSVData()
            .ToDictionary(x => x.ID);

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
            var requiredKitchenEquipmentCategoryComponents =
                recipe.RequiredKitchenEquipmentCategoryList
                    .Select(
                        equipment =>
                            new RecipeComponent(
                                equipment,
                                kitchenEquipmentCategoryDict[equipment].Name
                            )
                    )
                    .ToList();
            var resultFoodComponent = new RecipeComponent(
                recipe.ResultFoodID,
                foodDict[recipe.ResultFoodID].Name
            );

            recipes.Add(
                new RecipeResponse(
                    recipe.ID,
                    recipe.Name,
                    recipe.RequiredBlock,
                    resultFoodComponent,
                    requiredKitchenEquipmentCategoryComponents,
                    recipeIngredientComponents,
                    recipeFoodComponents
                )
            );
        }

        return recipes;
    }
}
