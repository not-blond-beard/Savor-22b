using System.Runtime.Serialization;
using Bencodex.Types;
using Libplanet.Action;

[Serializable]
public class ActionException : Exception, IExtractableException, ISerializable
{
    // 새로운 예외 클래스에서 사용할 메타데이터 프로퍼티
    private readonly string ErrorType;
    private readonly string ErrorMessage;
    private readonly int? ErrorCode;
    public IValue Metadata { get; }

    public ActionException(string message, string errorType, int? errorCode)
        : base(message)
    {
        ErrorType = errorType;
        ErrorMessage = message;
        ErrorCode = errorCode;

        var metaData = new Dictionary(
            new[]
            {
                new KeyValuePair<IKey, IValue>(
                    (Text) "errorType",
                    (Text) errorType
                ),
                new KeyValuePair<IKey, IValue>(
                    (Text) "errorMessage",
                    (Text) message
                )
            }
        );

        if (errorCode is int code)
        {
            Metadata = metaData.SetItem(
                (Text)"errorCode",
                (Integer)code
            );
        }

        Metadata = metaData;

    }
    protected ActionException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        if (info.GetString(nameof(ErrorType)) is { } errorType)
        {
            ErrorType = errorType;
        }

        if (info.GetString(nameof(ErrorMessage)) is { } errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        if (info.GetInt32(nameof(ErrorCode)) is int errorCode)
        {
            ErrorCode = errorCode;
        }

        if (info.GetValue(nameof(Metadata), typeof(IValue)) is IValue metadata)
        {
            Metadata = metadata;
        }
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);

        info.AddValue(nameof(ErrorType), ErrorType);
        info.AddValue(nameof(ErrorMessage), ErrorMessage);
        info.AddValue(nameof(ErrorCode), ErrorCode);

        if (Metadata is { } metadata)
        {
            info.AddValue(nameof(Metadata), metadata);
        }
    }
}
