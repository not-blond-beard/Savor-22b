namespace Savor22b.GraphTypes.Types;

using GraphQL.Types;

public class ExceptionMetadataType : ObjectGraphType<ExceptionMetadata>
{
    public ExceptionMetadataType()
    {
        Field<NonNullGraphType<StringGraphType>>(
            name: "errorType",
            description: "The error type of the exception.",
            resolve: context => context.Source.errorType
        );

        Field<NonNullGraphType<StringGraphType>>(
            name: "errorMessage",
            description: "The error message of the exception.",
            resolve: context => context.Source.errorMessage
        );

        Field<IntGraphType>(
            name: "errorCode",
            description: "The error code of the exception.",
            resolve: context => context.Source.errorCode
        );
    }
}
