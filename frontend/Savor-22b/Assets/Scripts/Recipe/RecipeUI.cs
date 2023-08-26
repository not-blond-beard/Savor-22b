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
    public int selectedRecipeId;


    public void SetRecipe(Recipe recipe)
    {
        id.text = recipe.id.ToString();
        name.text = recipe.name;
        minGrade.text = recipe.minGrade;
        maxGrade.text = recipe.maxGrade;
        ingredients = recipe.ingredients;

        recipeId = recipe.id;

        toggleButton.onValueChanged.AddListener(delegate { StoreRecipeId(); });
    }

    private void StoreRecipeId()
    {
        if (toggleButton.isOn)
        {
            selectedRecipeId = recipeId;
        }
        else
        {
            selectedRecipeId = 0;
        }

    }

    public int GetRecipeId()
    {
        return selectedRecipeId;
    }
}
