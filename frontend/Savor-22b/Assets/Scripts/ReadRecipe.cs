using System.Collections;
using System.Collections.Generic;
using System.Net.WebSockets;
using UnityEngine;
using UnityEngine.UI;

using GraphQlClient.Core;
using GraphQlClient.EventCallbacks;
using UnityEngine.Networking;

// data parsing
using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

using System.Threading.Tasks;
using System.Threading;

using TMPro; // Textmeshpro


public class ReadRecipe : MonoBehaviour
{

    public GameObject loading;

    public GraphApi SavorReference;
    public TMP_Text displayText;
    public TMP_Text parsedText;
    public string jsontext;

    private Recipe[] storedRecipes;
    public int recipeCount;


    public string pkHex;
    public string pAddress;

    private void SetUserinfo()
    {
        pkHex = PlayerInfo.pkHex;
        pAddress = PlayerInfo.pAddress;
    }

    public async void GetAllRecipes()
    {
        GraphApi.Query query = SavorReference.GetQueryByName("GetAllRecipe", GraphApi.Query.Type.Query);

        UnityWebRequest request = await SavorReference.Post(query);
        displayText.text = HttpHandler.FormatJson(request.downloadHandler.text);
        jsontext = displayText.text;
        Debug.Log("Json Text: " + "\n" + jsontext);
        ParseJson();
        TheResults(storedRecipes);
    }

    public void ParseJson()
    {
        // Make JObject from json string
        JObject json = JObject.Parse(jsontext);

        // Recipe counter for the loop
        JArray recipeJArray = (JArray)json["data"]["recipe"];
        int recipeCount = recipeJArray.Count;

        // Create array of Recipe class
        Recipe[] recipeArray = new Recipe[recipeCount];

        for (int i = 0; i < recipeCount; i++)
        {
            // Set Recipe class for storing data
            recipeArray[i] = new Recipe();

            // Parsing Data from json
            recipeArray[i].id = (int)json["data"]["recipe"][i]["id"];
            recipeArray[i].name = (string)json["data"]["recipe"][i]["name"];
            recipeArray[i].minGrade = (string)json["data"]["recipe"][i]["minGrade"];
            recipeArray[i].maxGrade = (string)json["data"]["recipe"][i]["maxGrade"];

            // Parsing Ingredients - Aware of 2 ingredients

            // Ingredient Counter for the loop
            JArray ingredientJArray = (JArray)json["data"]["recipe"][i]["ingredients"];
            int ingredientCount = ingredientJArray.Count;

            // Create array of IngredientType class
            recipeArray[i].ingredients = new IngredientType[ingredientCount];
            for (int j = 0; j < ingredientCount; j++)
            {
                recipeArray[i].ingredients[j] = new IngredientType();
                recipeArray[i].ingredients[j].id = (int)json["data"]["recipe"][i]["ingredients"][j]["id"];
                recipeArray[i].ingredients[j].name = (string)json["data"]["recipe"][i]["ingredients"][j]["name"];
                recipeArray[i].ingredients[j].type = (string)json["data"]["recipe"][i]["ingredients"][j]["type"];
            }

            // Print DebugLog for checking
            Debug.Log("Recipe ID: " + recipeArray[i].id);
            Debug.Log("Recipe Name: " + recipeArray[i].name);
            Debug.Log("Recipe MinGrade: " + recipeArray[i].minGrade);
            Debug.Log("Recipe MaxGrade: " + recipeArray[i].maxGrade);
            for (int j = 0; j < ingredientCount; j++)
            {
                Debug.Log("Recipe Ingredient ID: " + recipeArray[i].ingredients[j].id);
                Debug.Log("Recipe Ingredient Name: " + recipeArray[i].ingredients[j].name);
                Debug.Log("Recipe Ingredient Type: " + recipeArray[i].ingredients[j].type);
            }

            storedRecipes = recipeArray;
        }
    }

    public HashSet<string> displayedResults = new(); // 중복된 결과를 체크할 컨테이너

    public void DisplayResults(string result)
    {
        if (!displayedResults.Contains(result))
        {
            displayedResults.Add(result);

            string temp = parsedText.text + "\n" + result;
            parsedText.text = temp;
        }
    }

    public void TheResults(Recipe[] recipe)
    {
        for (int i = 0; i < recipe.Length; i++)
        {
            string result = "Recipe ID: " + recipe[i].id + "\n" +
                            "Recipe Name: " + recipe[i].name + "\n" +
                            "Recipe MinGrade: " + recipe[i].minGrade + "\n" +
                            "Recipe MaxGrade: " + recipe[i].maxGrade + "\n";

            for (int j = 0; j < recipe[i].ingredients.Length; j++)
            {
                result += "Recipe Ingredient ID: " + recipe[i].ingredients[j].id + "\n" +
                          "Recipe Ingredient Name: " + recipe[i].ingredients[j].name + "\n" +
                          "Recipe Ingredient Type: " + recipe[i].ingredients[j].type + "\n";
            }

            DisplayResults(result);
        }
    }

    [System.Serializable]
    //Recipe contains id, name, minGrade, maxGrade, ingredients id, name, type
    public class Recipe
    {
        public int id;
        public string name;
        public string minGrade;
        public string maxGrade;
        public IngredientType[] ingredients;
    }

    [System.Serializable]
    public class IngredientType
    {
        public int id;
        public string name;
        public string type;
    }

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        parsedText.text = "";
    }

}

