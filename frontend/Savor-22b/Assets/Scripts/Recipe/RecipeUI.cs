using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RecipeUI : MonoBehaviour
{
    public TMP_Text id;
    public new TMP_Text name;
    public TMP_Text minGrade;
    public TMP_Text maxGrade;

    public IngredientType[] ingredients;

    public Toggle toggleButton;

    private GameObject RecipePanel;
    private FoodGenerator foodGenerator;
    public int recipeId;


    public void SetRecipe(Recipe recipe)
    {
        id.text = recipe.id.ToString();
        name.text = recipe.name;
        minGrade.text = recipe.minGrade;
        maxGrade.text = recipe.maxGrade;
        ingredients = recipe.ingredients;

        recipeId = recipe.id;

        toggleButton.onValueChanged.AddListener(delegate { SendRecipeId(); });
    }

    private void SendRecipeId()
    {
        ResetSelectors();
        RecipePanel = GameObject.Find("RecipePanel");
        foodGenerator = RecipePanel.GetComponent<FoodGenerator>();

        foodGenerator.recipeId = recipeId;
    }

    // allow only one toggle button to be selected
    private void ResetSelectors()
    {
        GameObject[] recipeObjects = GameObject.FindGameObjectsWithTag("Recipe");
        foreach (GameObject recipeObject in recipeObjects)
        {
            Transform recipeToggleTransform = recipeObject.transform.Find("EdibleSelector");
            RecipeUI recipeUI = recipeObject.GetComponent<RecipeUI>();
            if (recipeToggleTransform != null && recipeUI.recipeId != recipeId)
            {
                GameObject recipeToggle = recipeToggleTransform.gameObject;
                Toggle toggle = recipeToggle.GetComponent<Toggle>();
                toggle.isOn = false;
            }
        }
    }
}
