namespace Savor22b.GraphTypes.Types;

using GraphQL.Types;
using Savor22b.States;

public class DungeonConquestHistoryStateType : ObjectGraphType<DungeonConquestHistoryState>
{
    public DungeonConquestHistoryStateType()
    {
        Field<NonNullGraphType<LongGraphType>>(
            name: "BlockIndex",
            description: "던전 점령을 신청한 블록의 인덱스입니다.",
            resolve: context => context.Source.BlockIndex
        );

        Field<NonNullGraphType<BooleanGraphType>>(
            name: "DungeonConquestStatus",
            description: "던전의 점령 신청이 성공했는지 여부입니다.",
            resolve: context => context.Source.DungeonConquestStatus
        );
    }
}
