namespace Savor22b.GraphTypes.Subscription;

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
using Libplanet.Store;
using System.Reactive.Subjects;
using Libplanet.Tx;
using Libplanet.Explorer.GraphTypes;
using Savor22b.GraphTypes.Types;
using Savor22b.GraphTypes.Query;

public class Subscription : ObjectGraphType
{
    [SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1118:ParameterMustNotSpanMultipleLines",
        Justification = "GraphQL docs require long lines of text."
    )]
    private readonly BlockChain _blockChain;
    private readonly BlockRenderer _blockRenderer;
    private readonly IStore _store;
    private readonly Subject<Libplanet.Blocks.BlockHash> _subject;

    public Subscription(
        BlockChain blockChain,
        BlockRenderer blockRenderer,
        IStore store,
        Swarm? swarm = null
    )
    {
        _blockChain = blockChain;
        _blockRenderer = blockRenderer;
        _store = store;
        _subject = new Subject<Libplanet.Blocks.BlockHash>();

        AddField(new UserStateField(_blockChain, _subject));
        AddField(new TradeInventoryStateField(_blockChain, _subject));
        AddField(new VillageField(_blockChain, _subject));

        AddField(
            new FieldType()
            {
                Name = "TransactionResult",
                Type = typeof(TxResultExtendType),
                Description = "Transaction Result",
                Arguments = new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>>
                    {
                        Name = "txId",
                        Description = "Transaction id",
                    }
                ),
                Resolver = new FuncFieldResolver<TxResult>(context =>
                {
                    if (!(_blockChain is BlockChain blockChain))
                    {
                        throw new ExecutionError("blockChain was not set yet!");
                    }

                    if (!(_store is IStore store))
                    {
                        throw new ExecutionError("store was not set yet!");
                    }

                    var strId = context.GetArgument<string>("txId");
                    var txId = new TxId(ByteUtil.ParseHex(strId));

                    if (!(store.GetFirstTxIdBlockHashIndex(txId) is { } txExecutedBlockHash))
                    {
                        if (_blockChain.GetStagedTransactionIds().Contains(txId))
                        {
                            return new TxResult(
                                TxStatus.STAGING,
                                null,
                                null,
                                null,
                                null,
                                null,
                                null,
                                null,
                                null
                            );
                        }
                        else
                        {
                            return new TxResult(
                                TxStatus.INVALID,
                                null,
                                null,
                                null,
                                null,
                                null,
                                null,
                                null,
                                null
                            );
                        }
                    }

                    try
                    {
                        TxExecution execution = blockChain.GetTxExecution(
                            txExecutedBlockHash,
                            txId
                        );
                        Block txExecutedBlock = blockChain[txExecutedBlockHash];

                        return execution switch
                        {
                            TxSuccess txSuccess
                                => new TxResult(
                                    TxStatus.SUCCESS,
                                    txExecutedBlock.Index,
                                    txExecutedBlock.Hash.ToString(),
                                    null,
                                    null,
                                    txSuccess.UpdatedStates,
                                    txSuccess.FungibleAssetsDelta,
                                    txSuccess.UpdatedFungibleAssets,
                                    txSuccess.ActionsLogsList
                                ),
                            TxFailure txFailure
                                => new TxResult(
                                    TxStatus.FAILURE,
                                    txExecutedBlock.Index,
                                    txExecutedBlock.Hash.ToString(),
                                    txFailure.ExceptionName,
                                    txFailure.ExceptionMetadata,
                                    null,
                                    null,
                                    null,
                                    null
                                ),
                            _
                                => throw new NotImplementedException(
                                    $"{nameof(execution)} is not expected concrete class."
                                )
                        };
                    }
                    catch (Exception)
                    {
                        return new TxResult(
                            TxStatus.INVALID,
                            null,
                            null,
                            null,
                            null,
                            null,
                            null,
                            null,
                            null
                        );
                    }
                }),
                StreamResolver = new SourceStreamResolver<Subject<BlockHash>>(
                    (context) => _subject.DistinctUntilChanged().Select(_ => _subject)
                )
            }
        );

        _blockRenderer.BlockSubject.ObserveOn(NewThreadScheduler.Default).Subscribe(RenderBlock);
    }

    private void RenderBlock((Block OldTip, Block NewTip) pair)
    {
        _subject.OnNext(pair.NewTip.Hash);
    }
}
