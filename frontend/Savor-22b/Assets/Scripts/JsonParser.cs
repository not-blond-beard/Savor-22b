using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

using TMPro; // Textmeshpro

public class JsonParser : MonoBehaviour
{
    public class SeedState
    {
        public string stateId { get; set; }
        public int seedId { get; set; }
    }


    string samplejson = @"
        {
            ""type"": ""data"",
            ""id"": ""default"",
            ""payload"": {
                ""data"": {
                    ""inventory"": {
                        ""seedStateList"": [
                            {
                                ""stateId"": ""359a2ee0-8786-4093-b92a-a1d1f9eb735b"",
                                ""seedId"": 5
                            },
                            {
                                ""stateId"": ""c38273ea-4d54-4cbf-8559-7e60801f18ab"",
                                ""seedId"": 4
                            }
                        ]
                    }
                }
            }
        }";

    public TMP_Text SeedDisplay;

    //ParseJson() is used to parse the JSON string into a C# object
    public void ParseJson()
    {
        string resultText = "";
        JObject json = JObject.Parse(samplejson);
        Debug.Log(json["payload"]["data"]["inventory"]["seedStateList"][0]["stateId"]);
        Debug.Log(json["payload"]["data"]["inventory"]["seedStateList"][0]["seedId"]);
        Debug.Log(json["payload"]["data"]["inventory"]["seedStateList"][1]["stateId"]);
        Debug.Log(json["payload"]["data"]["inventory"]["seedStateList"][1]["seedId"]);
        for (int i = 0; i < json.Count - 1; i++)
        {
            string state = json["payload"]["data"]["inventory"]["seedStateList"][i]["stateId"].ToString();
            string seed = json["payload"]["data"]["inventory"]["seedStateList"][i]["seedId"].ToString();
            string result = "State: " + state + "\nSeed: " + seed;
            resultText = string.Concat(resultText, result, "\n");
        }


        SeedDisplay.text = resultText;
    }
}
