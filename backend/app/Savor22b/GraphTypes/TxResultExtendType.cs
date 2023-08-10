namespace Savor22b.GraphTypes;

using System.Collections.Generic;
using System.Linq;
using GraphQL.Types;
using Libplanet;
using Libplanet.Assets;
using Libplanet.Explorer.GraphTypes;
using static Libplanet.Explorer.GraphTypes.TxResultType;

public class TxResultExtendType : ObjectGraphType<TxResult>
{
    public TxResultExtendType()
    {

        Field<NonNullGraphType<TxStatusType>>(
            nameof(TxResult.TxStatus),
            description: "The transaction status.",
            resolve: context => context.Source.TxStatus
        );

        Field<LongGraphType>(
            nameof(TxResult.BlockIndex),
            description: "The block index which the target transaction executed.",
            resolve: context => context.Source.BlockIndex
        );

        Field<StringGraphType>(
            nameof(TxResult.BlockHash),
            description: "The block hash which the target transaction executed.",
            resolve: context => context.Source.BlockHash
        );

        Field<StringGraphType>(
            nameof(TxResult.ExceptionName),
            description: "The name of exception. (when only failed)",
            resolve: context => context.Source.ExceptionName
        );

        Field<ExceptionMetadataType>(
            nameof(TxResult.ExceptionMetadata),
            description: "The hexadecimal string of the exception metadata. (when only failed)",
            resolve: context => context.Source.ExceptionMetadata != null ? new ExceptionMetadata(context.Source.ExceptionMetadata) : null
        );

        Field<ListGraphType<NonNullGraphType<UpdatedStateType>>>(
            nameof(TxResult.UpdatedStates),
            resolve: context => context.Source.UpdatedStates?
                .Select(pair => new UpdatedState(pair.Key, pair.Value))
        );

        Field<ListGraphType<NonNullGraphType<FungibleAssetBalancesType>>>(
            nameof(TxResult.UpdatedFungibleAssets),
            resolve: context => context.Source.UpdatedFungibleAssets?
                .Select(pair => new FungibleAssetBalances(pair.Key, pair.Value.Values))
        );

        Field<ListGraphType<NonNullGraphType<FungibleAssetBalancesType>>>(
            nameof(TxResult.FungibleAssetsDelta),
            resolve: context => context.Source.FungibleAssetsDelta?
                .Select(pair => new FungibleAssetBalances(pair.Key, pair.Value.Values))
        );

        Field<ListGraphType<
            NonNullGraphType<ListGraphType<
                NonNullGraphType<StringGraphType>
            >>
        >>(
            nameof(TxResult.ActionsLogsList),
            resolve: context => context.Source.ActionsLogsList
        );
    }

    public record UpdatedState(Address Address, Bencodex.Types.IValue? State);

    public class UpdatedStateType : ObjectGraphType<UpdatedState>
    {
        public UpdatedStateType()
        {
            Field<NonNullGraphType<AddressType>>(
                nameof(UpdatedState.Address),
                resolve: context => context.Source.Address
            );
            Field<BencodexValueType>(
                nameof(UpdatedState.State),
                resolve: context => context.Source.State
            );
        }
    }
}
