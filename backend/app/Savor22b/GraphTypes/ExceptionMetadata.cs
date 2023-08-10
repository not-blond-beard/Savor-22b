namespace Savor22b.GraphTypes;

using Bencodex.Types;
using Libplanet.Headless.Extensions;

public class ExceptionMetadata
{
    public string errorType { get; set; }
    public string errorMessage { get; set; }
    public int? errorCode { get; set; }

    public ExceptionMetadata(string errorType, string errorMessage, int? errorCode)
    {
        this.errorType = errorType;
        this.errorMessage = errorMessage;
        this.errorCode = errorCode;
    }

    public ExceptionMetadata(Bencodex.Types.IValue encoded)
    {
        if (encoded is Bencodex.Types.Dictionary dict)
        {
            // Bencodex 데이터에서 필요한 값을 꺼내서 ExceptionMetadata 객체에 매핑합니다.
            if (dict.TryGetValue((Text)"errorType", out var errorTypeValue) && errorTypeValue is Text errorTypeText)
            {
                errorType = errorTypeText.Value;
            }

            if (dict.TryGetValue((Text)"errorMessage", out var errorMessageValue) && errorMessageValue is Text errorMessageText)
            {
                errorMessage = errorMessageText.Value;
            }

            if (dict.TryGetValue((Text)"errorCode", out var errorCodeValue) && errorCodeValue is Integer errorCodeInteger)
            {
                errorCode = (int)errorCodeInteger;
            }
        }
    }
}
