using System;
using Newtonsoft.Json.Linq;

public static class ResponseParser{
    public static JToken Parse(string jsonString){
        JObject json = JObject.Parse(jsonString);

        if (json["payload"] is not JObject payload)
        {
            throw new Exception("Payload not found in JSON.");
        }

        if (payload["data"] is not JToken dataToken)
        {
            throw new Exception("Data not found in JSON.");
        }

        return dataToken;
    }

    public static T Parse<T>(string jsonString, string targetPath){
        JToken token = Parse(jsonString);

        if (token[targetPath] is not JObject target)
        {
            throw new Exception("Target not found in JSON.");
        }

        return target.ToObject<T>();
    }
}