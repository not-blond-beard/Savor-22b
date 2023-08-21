using System.Net.WebSockets;
using GraphQlClient.Core;
using GraphQlClient.EventCallbacks;
using UnityEngine;
using UnityEngine.Networking;

public class RecipeManager : MonoBehaviour
{
    [Header("API")]
    public GraphApi svrReference;

    [Header("UI Prefabs")]
    public GameObject recipePrefab;

    [Header("UI Containers")]
    public RectTransform recipeContent;

    public string originalJson;

    private void DrawRecipeList(Recipe[] recipeList)
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
        GraphApi.Query query = svrReference.GetQueryByName("GetAllRecipe", GraphApi.Query.Type.Query);
        UnityWebRequest request = await svrReference.Post(query);
        originalJson = request.downloadHandler.text;
        DisplayData(originalJson);

    }

    public void DisplayData(string data)
    {
        Recipes recipes = Recipes.CreateFromJSON(data);

        DrawRecipeList(recipes.recipeList);
    }

    void Start()
    {
        GetAllRecipes();
    }


}

