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
                Name = "Inventory",
                Type = typeof(InventoryStateType),
                Description = "Inventory state",
                Arguments = new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>>
                    {
                        Name = "address",
                        Description = "The account holder's 40-hex address",
                    }
                ),
                Resolver = new FuncFieldResolver<InventoryState>(context =>
                {
                    var accountAddress = new Address(context.GetArgument<string>("address"));
                    return GetInventoryState(accountAddress);
                }),
                StreamResolver = new SourceStreamResolver<InventoryState>((context) =>
                {
                    var accountAddress = new Address(context.GetArgument<string>("address"));

                    return _subject
                        .DistinctUntilChanged()
                        .Select(_ => GetInventoryState(accountAddress));
                }),
            }
        );

        _blockRenderer.BlockSubject
                .ObserveOn(NewThreadScheduler.Default)
                .Subscribe(RenderBlock);
    }

    private InventoryState GetInventoryState(Address address)
    {
        var inventoryStateEncoded = _blockChain.GetState(address);

        InventoryState inventoryState =
            inventoryStateEncoded is Bencodex.Types.Dictionary bdict
                ? new InventoryState(bdict)
                : new InventoryState();

        return inventoryState;
    }

    private void RenderBlock((Block OldTip, Block NewTip) pair)
    {
        _subject.OnNext(pair.NewTip.Hash);
    }
}
