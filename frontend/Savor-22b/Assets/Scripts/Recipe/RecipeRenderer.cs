using System.Net.WebSockets;
using GraphQlClient.Core;
using GraphQlClient.EventCallbacks;
using UnityEngine;
using UnityEngine.UI;
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

    public Button recipeButton;
    public Button combineButton;

    private bool isRecipeSelector = false;

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
        SetRecipeSelectorButton();
    }

    private void SetRecipeSelectorButton()
    {
        recipeButton.onClick.AddListener(ToggleRecipeSelector);

    }

    private void ToggleRecipeSelector()
    {
        if (!isRecipeSelector)
        {
            ActivateRecipeSelector();
        }
        else
        {
            DeactivateRecipeSelector();
        }
    }

    private void ActivateRecipeSelector()
    {
        GameObject[] recipeObjects = GameObject.FindGameObjectsWithTag("Recipe");
        foreach (GameObject recipeObject in recipeObjects)
        {
            Transform recipeToggleTransform = recipeObject.transform.Find("EdibleSelector");
            if (recipeToggleTransform != null)
            {
                GameObject recipeToggle = recipeToggleTransform.gameObject;
                recipeToggle.SetActive(true);
            }
        }
        isRecipeSelector = true;
    }

    private void DeactivateRecipeSelector()
    {
        GameObject[] recipeObjects = GameObject.FindGameObjectsWithTag("Recipe");
        foreach (GameObject recipeObject in recipeObjects)
        {
            Transform recipeToggleTransform = recipeObject.transform.Find("EdibleSelector");
            if (recipeToggleTransform != null)
            {
                GameObject recipeToggle = recipeToggleTransform.gameObject;
                recipeToggle.SetActive(false);
            }
        }
        isRecipeSelector = false;
    }


}

