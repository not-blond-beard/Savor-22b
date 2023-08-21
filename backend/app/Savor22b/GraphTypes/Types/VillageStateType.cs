namespace Savor22b.GraphTypes.Types;

using GraphQL.Types;
using Savor22b.States;

public class VillageStateType : ObjectGraphType<VillageState>
{
    public VillageStateType()
    {
        Field<ListGraphType<HouseFieldStateType>>(
            name: "houseFieldStates",
            description: "The list of house field states in the village.",
            resolve: context =>
            {
                var states = context.Source.HouseFieldStates.Select(state => state.Value);
                return states;
            }
        );
        Field<NonNullGraphType<HouseStateType>>(
            name: "houseState",
            description: "The house state in the village.",
            resolve: context => context.Source.HouseState
        );
    }
}
