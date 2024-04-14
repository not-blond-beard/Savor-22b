namespace Savor22b.GraphTypes.Types;

using GraphQL.Types;
using Libplanet;
using Libplanet.Blockchain;
using Libplanet.Explorer.GraphTypes;
using Savor22b.Model;
using Savor22b.States.Trade;

public class TradeGoodStateType : ObjectGraphType<TradeGood>
{
    public TradeGoodStateType(BlockChain blockChain)
    {
        Field<NonNullGraphType<StringGraphType>>(
            "sellerAddress",
            description: "상품 판매자 주소입니다.",
            resolve: context => context.Source.SellerAddress.ToString()
        );

        Field<NonNullGraphType<GuidGraphType>>(
            "productStateId",
            description: "상품 고유 Id입니다.",
            resolve: context => context.Source.ProductStateId
        );

        Field<NonNullGraphType<BigIntGraphType>>(
            "price",
            description: "상품 가격입니다.",
            resolve: context => context.Source.Price.MajorUnit
        );

        Field<NonNullGraphType<StringGraphType>>(
            "type",
            description: "상품 타입입니다. (Food or Items)",
            resolve: context => context.Source.Type
        );

        Field<RefrigeratorStateType>(
            "food",
            resolve: context => context.Source is FoodGoodState foodGood ? foodGood.Food : null
        );

        Field<ListGraphType<ItemStateDetailType>>(
            "items",
            resolve: context => context.Source is ItemsGoodState itemsGood ? itemsGood.Items : null
        );
    }
}
