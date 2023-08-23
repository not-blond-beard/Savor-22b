using System.Net.WebSockets;
using GraphQlClient.Core;
using GraphQlClient.EventCallbacks;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

public class RecipeRenderer : MonoBehaviour
{
    [Header("API")]
    public GraphApi svrReference;

    [Header("UI Prefabs")]
    public GameObject recipePrefab;

    [Header("UI Containers")]
    public RectTransform recipeContent;

    private void DrawRecipeList(List<Recipe> recipeList)
    {
        foreach (Recipe recipe in recipeList)
        {
            GameObject recipeUI = Instantiate(recipePrefab, recipeContent);
            RecipeUI recipeUIComponent = recipeUI.GetComponent<RecipeUI>();
            recipeUIComponent.SetRecipe(recipe);
        }
    }

    public async void GetAllRecipes()
    {
        GraphApi.Query query = svrReference.GetQueryByName(QueryNames.GET_ALL_RECIPE, GraphApi.Query.Type.Query);
        UnityWebRequest request = await svrReference.Post(query);

        DisplayData(request.downloadHandler.text);

    }

    public void DisplayData(string data)
    {
        List<Recipe> recipes = Recipe.CreateFromJSON(data);

        DrawRecipeList(recipes);
    }

    void Start()
    {
        GetAllRecipes();
    }


}

