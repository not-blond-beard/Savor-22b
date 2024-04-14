namespace Savor22b.GraphTypes.Types;

using GraphQL.Types;
using Libplanet.Blockchain;
using Savor22b.States;
using Savor22b.States.Trade;

public class TradeInventoryStateType : ObjectGraphType<TradeInventoryState>
{
    public TradeInventoryStateType(BlockChain blockChain)
    {
        Field<NonNullGraphType<ListGraphType<TradeGoodStateType>>>(
            name: "tradeGoods",
            description: "무역상점의 상품들입니다.",
            resolve: context => context.Source.TradeGoods.Select(x => x.Value).ToList()
        );
    }
}
