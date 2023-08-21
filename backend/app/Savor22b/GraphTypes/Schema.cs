namespace Savor22b.GraphTypes;

public class Schema : GraphQL.Types.Schema
{
    public Schema(IServiceProvider services)
        : base(services)
    {
        Query = services.GetRequiredService<Query.Query>();
        Mutation = services.GetRequiredService<Mutation.Mutation>();
        Subscription = services.GetRequiredService<Subscription.Subscription>();
    }
}
