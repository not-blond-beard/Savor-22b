using System.Net.WebSockets;
using GraphQlClient.Core;
using GraphQlClient.EventCallbacks;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;


public class IngredientManager : MonoBehaviour
{
    [Header("UI Prefabs")]
    public GameObject ingredientPrefab;

    [Header("UI Containers")]
    public RectTransform ingredientContent;

    [Header("Parent")]
    public GameObject parent;

    private void GetIngredientFromParent()
    {
        //get component script from parent
        RecipeUI parentInfo = parent.GetComponent<RecipeUI>();
        DrawIngredientList(parentInfo.ingredients);
    }

    private void DrawIngredientList(IngredientType[] ingredients)
    {
        foreach (IngredientType ingredient in ingredients)
        {
            GameObject ingredientUI = Instantiate(ingredientPrefab, ingredientContent);
            IngredientUI ingredientUIComponent = ingredientUI.GetComponent<IngredientUI>();
            ingredientUIComponent.SetIngredient(ingredient);
        }
    }

    private void Start()
    {
        GetIngredientFromParent();
    }


}