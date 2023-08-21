namespace Savor22b.GraphTypes.Types;

using GraphQL.Types;

public class KitchenEquipmentStateDetailType : ObjectGraphType<KitchenEquipmentStateDetail>
{
    public KitchenEquipmentStateDetailType()
    {
        Field<NonNullGraphType<GuidGraphType>>(
            name: "stateId",
            description: "The ID of the kitchen equipment state.",
            resolve: context => context.Source.StateID
        );

        Field<NonNullGraphType<IntGraphType>>(
            name: "equipmentId",
            description: "The ID of the kitchen equipment.",
            resolve: context => context.Source.KitchenEquipmentID
        );

        Field<NonNullGraphType<StringGraphType>>(
            name: "equipmentName",
            description: "The Name of the kitchen equipment.",
            resolve: context => context.Source.EquipmentName
        );

        Field<NonNullGraphType<DecimalGraphType>>(
            name: "blockTimeReductionPercent",
            description: "The block time reduction percent of the kitchen equipment.",
            resolve: context => context.Source.BlockTimeReductionPercent
        );

        Field<NonNullGraphType<IntGraphType>>(
            name: "equipmentCategoryId",
            description: "The ID of the kitchen equipment category.",
            resolve: context => context.Source.KitchenEquipmentCategoryID
        );

        Field<NonNullGraphType<StringGraphType>>(
            name: "equipmentCategoryName",
            description: "The Name of the kitchen equipment category.",
            resolve: context => context.Source.EquipmentCategoryName
        );

        Field<NonNullGraphType<StringGraphType>>(
            name: "equipmentCategoryType",
            description: "The Name of the kitchen equipment category.",
            resolve: context => context.Source.EquipmentCategoryType
        );

        Field<NonNullGraphType<BooleanGraphType>>(
            name: "isCooking",
            description: "This equipment cooking status.",
            resolve: context => context.Source.IsCooking
        );

        Field<LongGraphType>(
            name: "cookingEndBlockIndex",
            description: "The end block index indicates when this equipment is cooking.",
            resolve: context => context.Source.CookingEndBlockIndex
        );

        Field<RefrigeratorStateType>(
            name: "cookingFood",
            description: "This equipment's cooking food.",
            resolve: context => context.Source.CookingFood
        );
    }
}
