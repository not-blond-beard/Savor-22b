
namespace Savor22b.GraphTypes;

using GraphQL.Types;

public class TxApplied
{
    public bool TransactionApplied { get; set; }
    public TxApplied(bool transactionApplied)
    {
        TransactionApplied = transactionApplied;
    }
}

public class TxAppliedGraphType : ObjectGraphType<TxApplied>
{
    public TxAppliedGraphType()
    {
        Field<BooleanGraphType>(
            name: "applied",
            description: "Whether the transaction was applied or not.",
            resolve: context => context.Source.TransactionApplied
        );
    }
}
