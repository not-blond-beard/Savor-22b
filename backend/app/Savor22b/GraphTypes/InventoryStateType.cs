using GraphQL.Types;
using Savor22b.States;

public class InventoryStateType : ObjectGraphType<InventoryState>
{
    public InventoryStateType()
    {
        Field<ListGraphType<SeedStateType>>(
            name: "seedStateList",
            description: "The list of seed states in the inventory.",
            resolve: context => context.Source.SeedStateList.ToList()
        );
    }
}

public class SeedStateType : ObjectGraphType<SeedState>
{
    public SeedStateType()
    {
        Field<IntGraphType>(
            name: "id",
            description: "The ID of the seed state.",
            resolve: context => context.Source.Id
        );

        Field<StringGraphType>(
            name: "name",
            description: "The name of the seed state.",
            resolve: context => context.Source.Name
        );
    }
}
