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
    public int selectedRecipeId;
    public ToggleGroup toggleGroup;

    void Start()
    {
        GetAllRecipes();
        SetRecipeSelectorButton();
    }

    public async void GetAllRecipes()
    {
        GraphApi.Query query = svrReference.GetQueryByName(QueryNames.GET_ALL_RECIPE, GraphApi.Query.Type.Query);
        UnityWebRequest request = await svrReference.Post(query);

        DisplayData(request.downloadHandler.text);

    }

    private void DrawRecipeList(List<Recipe> recipeList)
    {
        foreach (Recipe recipe in recipeList)
        {
            GameObject recipeUI = Instantiate(recipePrefab, recipeContent);
            RecipeUI recipeUIComponent = recipeUI.GetComponent<RecipeUI>();
            recipeUIComponent.SetRecipe(recipe);
        }
    }


    public void DisplayData(string data)
    {
        List<Recipe> recipes = Recipe.CreateFromJSON(data);

        DrawRecipeList(recipes);
        SetToggleGroup();
    }

    private void SetRecipeSelectorButton()
    {
        recipeButton.onClick.AddListener(ToggleRecipeSelector);
        combineButton.onClick.AddListener(DeactivateRecipeSelector);

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

    public int GetRecipeId()
    {
        selectedRecipeId = new int();
        GameObject[] recipeObjects = GameObject.FindGameObjectsWithTag("Recipe");
        foreach (GameObject recipeObject in recipeObjects)
        {
            RecipeUI selectedObject = recipeObject.GetComponent<RecipeUI>();
            if (selectedObject.GetRecipeId() != 0)
                selectedRecipeId = selectedObject.GetRecipeId();
            break;
        }
        return selectedRecipeId;
    }

    public void SetToggleGroup()
    {
        GameObject[] recipeObjects = GameObject.FindGameObjectsWithTag("Recipe");
        foreach (GameObject recipeObject in recipeObjects)
        {
            Toggle toggle = recipeObject.GetComponent<Toggle>();
            toggleGroup.RegisterToggle(toggle);
        }
    }

}

