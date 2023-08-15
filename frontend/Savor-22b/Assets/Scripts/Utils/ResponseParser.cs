using Newtonsoft.Json.Linq;

public static class ResponseParser{
    static public JToken Parse(string response){
        string responseText = response;
        JObject json = JObject.Parse(responseText);

        if (
            json.GetValue("payload") is null 
            || json["payload"]?["data"] is null
        ){
            throw new System.Exception("Invalid response");
        }
        JToken data = json["payload"]["data"];

        return data;
    }

    static public T Parse<T>(string response, string dataKey){
        JToken token = Parse(response);
        T data = token.SelectToken(dataKey).ToObject<T>();

        return data;
    }
}