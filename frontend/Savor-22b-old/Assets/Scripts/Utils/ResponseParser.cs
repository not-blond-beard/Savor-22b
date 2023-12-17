using System;
using Newtonsoft.Json.Linq;

public static class ResponseParser
{
    public static JToken Parse(string jsonString)
    {
        JObject json = JObject.Parse(jsonString);

        if (json["payload"] is not JObject payload)
        {
            if (json["data"] is not JToken dataToken)
            {
                throw new Exception("Payload not found in JSON.");
            }
            else if (dataToken is not null)
            {
                return dataToken;
            }

            throw new Exception("Payload not found in JSON.");
        }

        if (payload["data"] is not JToken resultToken)
        {
            throw new Exception("Data not found in JSON.");
        }

        return resultToken;
    }

    public static T Parse<T>(string jsonString, string targetPath)
    {
        JToken jObject = Parse(jsonString);
        JToken token = jObject.SelectToken(targetPath);

        if (token == null)
        {
            throw new Exception("Target not found in JSON.");
        }

        T result = token.ToObject<T>();

        return result;
    }
}