namespace Savor22b.GraphTypes.Types;

using GraphQL.Types;
using Savor22b.States;

public class UserStateType : ObjectGraphType<RootState>
{
    public UserStateType()
    {
        Field<NonNullGraphType<InventoryStateType>>(
            name: "inventoryState",
            description: "The inventory state of the user.",
            resolve: context => context.Source.InventoryState
        );
        Field<VillageStateType>(
            name: "villageState",
            description: "The village state of the user.",
            resolve: context => context.Source.VillageState
        );
        Field<NonNullGraphType<DungeonStateType>>(
            name: "dungeonState",
            description: "The dungeon state of the user.",
            resolve: context => context.Source.DungeonState
        );
    }
}
