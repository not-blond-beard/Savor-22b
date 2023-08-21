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

        Field<LongGraphType>(
            name: "cookingStartedBlockIndex",
            description: "The block index when the cooking started.",
            resolve: context => context.Source.CookingStartedBlockIndex
        );

        Field<LongGraphType>(
            name: "cookingDurationBlock",
            description: "The block duration of the cooking.",
            resolve: context => context.Source.CookingDurationBlock
        );
    }
}
