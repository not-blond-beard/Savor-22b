namespace Savor22b.GraphTypes;

using System;
using System.Reactive.Linq;
using System.Diagnostics.CodeAnalysis;
using GraphQL.Resolvers;
using GraphQL.Types;
using Libplanet;
using Libplanet.Blockchain;
using Libplanet.Net;
using Savor22b.States;
using GraphQL;
using System.Reactive.Concurrency;
using Libplanet.Blocks;
using System.Reactive.Subjects;
using Libplanet.Tx;

public class Subscription : ObjectGraphType
{
    [SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1118:ParameterMustNotSpanMultipleLines",
        Justification = "GraphQL docs require long lines of text.")]

    private readonly BlockChain _blockChain;
    private readonly BlockRenderer _blockRenderer;
    private readonly Subject<Libplanet.Blocks.BlockHash> _subject;
    public Subscription(
        BlockChain blockChain,
        BlockRenderer blockRenderer,
        Swarm? swarm = null
        )
    {
        _blockChain = blockChain;
        _blockRenderer = blockRenderer;
        _subject = new Subject<Libplanet.Blocks.BlockHash>();

        AddField(
            new FieldType()
            {
                Name = "UserState",
                Type = typeof(UserStateType),
                Description = "User State",
                Arguments = new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>>
                    {
                        Name = "address",
                        Description = "The account holder's 40-hex address",
                    }
                ),
                Resolver = new FuncFieldResolver<RootState>(context =>
                {
                    var accountAddress = new Address(context.GetArgument<string>("address"));
                    return GetRootState(accountAddress);
                }),
                StreamResolver = new SourceStreamResolver<RootState>((context) =>
                {
                    var accountAddress = new Address(context.GetArgument<string>("address"));

                    return _subject
                        .DistinctUntilChanged()
                        .Select(_ => GetRootState(accountAddress));
                }),
            }
        );

        AddField(
            new FieldType()
            {
                Name = "TransactionApplied",
                Type = typeof(TxAppliedGraphType),
                Description = "Transaction completed",
                Arguments = new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>>
                    {
                        Name = "txId",
                        Description = "Transaction id",
                    }
                ),
                Resolver = new FuncFieldResolver<TxApplied>(context =>
                {
                    var strId = context.GetArgument<string>("txId");
                    var txId = new TxId(ByteUtil.ParseHex(strId));
                    bool transactionExists = _blockChain.GetTransaction(txId) != null;

                    return new TxApplied(_blockChain.GetTransaction(txId) != null);
                }),
                StreamResolver = new SourceStreamResolver<TxApplied>((context) =>
                {
                    var strId = context.GetArgument<string>("txId");
                    var txId = new TxId(ByteUtil.ParseHex(strId));

                    return _subject
                        .DistinctUntilChanged()
                        .Select(_ => new TxApplied(_blockChain.GetTransaction(txId) != null));
                }),
            }
        );

        _blockRenderer.BlockSubject
                .ObserveOn(NewThreadScheduler.Default)
                .Subscribe(RenderBlock);
    }

    private RootState GetRootState(Address address)
    {
        var rootStateEncoded = _blockChain.GetState(address);

        RootState rootState =
            rootStateEncoded is Bencodex.Types.Dictionary bdict
                ? new RootState(bdict)
                : new RootState();

        return rootState;
    }

    private void RenderBlock((Block OldTip, Block NewTip) pair)
    {
        _subject.OnNext(pair.NewTip.Hash);
    }
}
