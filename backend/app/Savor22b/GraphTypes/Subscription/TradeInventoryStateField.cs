namespace Savor22b.GraphTypes.Query;

using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Bencodex.Types;
using GraphQL.Resolvers;
using GraphQL.Types;
using Libplanet.Blockchain;
using Savor22b.GraphTypes.Types;
using Savor22b.States.Trade;

public class TradeInventoryStateField : FieldType
{
    private readonly BlockChain _blockChain;
    private readonly Subject<Libplanet.Blocks.BlockHash> _subject;

    public TradeInventoryStateField(BlockChain blockChain, Subject<Libplanet.Blocks.BlockHash> subject)
        : base()
    {
        _blockChain = blockChain;
        _subject = subject;

        Name = "TradeInventoryState";
        Type = typeof(TradeInventoryStateType);
        Description = "무역상점";
        Arguments = new QueryArguments();
        Resolver = new FuncFieldResolver<TradeInventoryState>(context =>
        {
            return GetTradeInventoryState(_blockChain);
        });
        StreamResolver = new SourceStreamResolver<TradeInventoryState>(
            (context) =>
            {
                return _subject.DistinctUntilChanged().Select(_ => GetTradeInventoryState(_blockChain));
            }
        );
    }

    public static TradeInventoryState GetTradeInventoryState(BlockChain blockChain)
    {
        var tradeInventoryStateEncoded = blockChain.GetState(TradeInventoryState.StateAddress);

        TradeInventoryState tradeInventoryState = tradeInventoryStateEncoded is Dictionary bdict
            ? new TradeInventoryState(bdict)
            : new TradeInventoryState();

        return tradeInventoryState;
    }
}
