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

    string jsonText = @"
    {
        ""data"": {
        ""recipe"": [
            {
            ""id"": 1,
            ""name"": ""김밥 레시피"",
            ""minGrade"": ""D"",
            ""maxGrade"": ""B"",
            ""ingredients"": [
                {
                ""id"": 1,
                ""name"": ""쌀"",
                ""type"": ""ingredient""
                },
                {
                ""id"": 2,
                ""name"": ""김"",
                ""type"": ""ingredient""
                }
            ]
            },
            {
            ""id"": 2,
            ""name"": ""야채 김밥 레시피"",
            ""minGrade"": ""B"",
            ""maxGrade"": ""A"",
            ""ingredients"": [
                {
                ""id"": 4,
                ""name"": ""당근"",
                ""type"": ""ingredient""
                },
                {
                ""id"": 1,
                ""name"": ""김밥 레시피"",
                ""type"": ""food""
                }
            ]
            },
            {
            ""id"": 3,
            ""name"": ""불고기 김밥 레시피"",
            ""minGrade"": ""A"",
            ""maxGrade"": ""S"",
            ""ingredients"": [
                {
                ""id"": 2,
                ""name"": ""야채 김밥 레시피"",
                ""type"": ""food""
                },
                {
                ""id"": 5,
                ""name"": ""소"",
                ""type"": ""ingredient""
                }
            ]
            },
            {
            ""id"": 4,
            ""name"": ""트러플 불고기 김밥 레시피"",
            ""minGrade"": ""S"",
            ""maxGrade"": ""SS"",
            ""ingredients"": [
                {
                ""id"": 3,
                ""name"": ""케비어"",
                ""type"": ""ingredient""
                },
                {
                ""id"": 3,
                ""name"": ""불고기 김밥 레시피"",
                ""type"": ""food""
                }
            ]
            }
        ]
        }
    }";

    int sampleid;
    string samplestring;
    public void ParseJsonny()
    {
        Debug.Log(jsonText);
        JObject json = JObject.Parse(jsonText);
        JArray recipeArray = (JArray)json["data"]["recipe"];
        int recipeCount = recipeArray.Count;
        Debug.Log("Recipe Count: " + recipeCount);

        for (int i = 0; i < recipeCount; i++)
        {
            Debug.Log("Recipe ID: " + json["data"]["recipe"][i]["id"]);
            Debug.Log("Recipe Name: " + json["data"]["recipe"][i]["name"]);
            Debug.Log("Recipe MinGrade: " + json["data"]["recipe"][i]["minGrade"]);
            Debug.Log("Recipe MaxGrade: " + json["data"]["recipe"][i]["maxGrade"]);

            sampleid = (int)json["data"]["recipe"][i]["id"];
            Debug.Log("SAMPLE ID: " + sampleid);
            samplestring = json["data"]["recipe"][i]["name"].ToString();
            Debug.Log("SAMPLE STRING: " + samplestring);

            JArray ingredientsArray = (JArray)json["data"]["recipe"][i]["ingredients"];
            int ingredientsCount = ingredientsArray.Count;
            for (int j = 0; j < ingredientsCount; j++)
            {
                Debug.Log("Recipe Ingredient ID: " + json["data"]["recipe"][i]["ingredients"][j]["id"]);
                Debug.Log("Recipe Ingredient Name: " + json["data"]["recipe"][i]["ingredients"][j]["name"]);
                Debug.Log("Recipe Ingredient Type: " + json["data"]["recipe"][i]["ingredients"][j]["type"]);
            }
        }

    }

    private void Start()
    {
        ParseJsonny();
    }
}
