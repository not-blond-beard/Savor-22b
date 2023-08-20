namespace Savor22b.GraphTypes.Types;

using GraphQL.Types;
using Savor22b.States;

public class VillageStateType : ObjectGraphType<VillageState>
{
    public VillageStateType()
    {
        Field<NonNullGraphType<ListGraphType<HouseFieldStateType>>>(
            name: "houseFieldStates",
            description: "The list of house field states in the village.",
            resolve: context => context.Source.HouseFieldStates.ToList()
        );
        Field<NonNullGraphType<HouseStateType>>(
            name: "houseState",
            description: "The house state in the village.",
            resolve: context => context.Source.HouseState
        );
    }
}
