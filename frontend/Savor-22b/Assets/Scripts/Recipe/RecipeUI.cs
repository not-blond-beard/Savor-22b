using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RecipeUI : MonoBehaviour
{
    public TMP_Text id;
    public new TMP_Text name;
    //public TMP_Text ingredients;
    public TMP_Text minGrade;
    public TMP_Text maxGrade;

    public IngredientType[] ingredients;

    public void SetRecipe(Recipe recipe)
    {
        id.text = recipe.id.ToString();
        name.text = recipe.name;
        //ingredients.text = recipe.ingredients.ToString();
        minGrade.text = recipe.minGrade;
        maxGrade.text = recipe.maxGrade;
        ingredients = recipe.ingredients;
    }
}
