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

    public int recipeId;


    public void SetRecipe(Recipe recipe)
    {
        id.text = recipe.id.ToString();
        name.text = recipe.name;
        minGrade.text = recipe.minGrade;
        maxGrade.text = recipe.maxGrade;
        ingredients = recipe.ingredients;

        recipeId = recipe.id;
    }
}
