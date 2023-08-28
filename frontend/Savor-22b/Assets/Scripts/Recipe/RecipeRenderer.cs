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
    private Dictionary<int, GameObject> recipeUIObjects = new Dictionary<int, GameObject>();

    public GameObject toggleGroupObject;
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

            //group the toggles
            recipeUIComponent.toggleButton.group = toggleGroup;

            //setting listner for FoodGenerator
            recipeUIComponent.toggleButton.onValueChanged.AddListener(delegate { handleSelectRecipeButton(recipe.id); });

            recipeUIObjects.Add(recipe.id, recipeUI);
        }
    }



    public void DisplayData(string data)
    {
        List<Recipe> recipes = Recipe.CreateFromJSON(data);

        DrawRecipeList(recipes);
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
        foreach (var kvp in recipeUIObjects)
        {
            GameObject recipeUI = kvp.Value;
            Transform recipeToggleTransform = recipeUI.transform.Find("EdibleSelector");
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
        foreach (var kvp in recipeUIObjects)
        {
            GameObject recipeUI = kvp.Value;
            Transform recipeToggleTransform = recipeUI.transform.Find("EdibleSelector");
            if (recipeToggleTransform != null)
            {
                GameObject recipeToggle = recipeToggleTransform.gameObject;
                recipeToggle.SetActive(false);
            }
        }
        isRecipeSelector = false;
    }

    // handler for FoodGenerator
    private void handleSelectRecipeButton(int recipeId)
    {
        selectedRecipeId = recipeId;
    }

    public void ResetRecipeSelectors()
    {
        foreach (var kvp in recipeUIObjects)
        {
            GameObject recipeUI = kvp.Value;
            Transform recipeToggleTransform = recipeUI.transform.Find("EdibleSelector");
            if (recipeToggleTransform != null)
            {
                GameObject recipeToggle = recipeToggleTransform.gameObject;
                Toggle toggle = recipeToggle.GetComponent<Toggle>();
                toggle.isOn = false;
            }
        }
    }
}

