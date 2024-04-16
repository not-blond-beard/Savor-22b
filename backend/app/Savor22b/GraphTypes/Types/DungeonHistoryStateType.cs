namespace Savor22b.GraphTypes.Types;

using GraphQL.Types;
using Savor22b.States;

public class DungeonHistoryStateType : ObjectGraphType<DungeonHistoryState>
{
    public DungeonHistoryStateType()
    {
        Field<NonNullGraphType<LongGraphType>>(
            name: "BlockIndex",
            description: "던전 클리어를 신청한 블록 인덱스입니다.",
            resolve: context => context.Source.BlockIndex
        );

        Field<NonNullGraphType<BooleanGraphType>>(
            name: "DungeonClearStatus",
            description: "던전 클리어 결과입니다.",
            resolve: context => context.Source.DungeonClearStatus
        );
    }
}
