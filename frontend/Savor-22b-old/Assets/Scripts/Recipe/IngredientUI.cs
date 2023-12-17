using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class IngredientUI : MonoBehaviour
{
    public TMP_Text id;
    public new TMP_Text name;
    public TMP_Text type;

    public void SetIngredient(IngredientType ingredient)
    {
        id.text = ingredient.id.ToString();
        name.text = ingredient.name;
        type.text = ingredient.type;
    }
}
