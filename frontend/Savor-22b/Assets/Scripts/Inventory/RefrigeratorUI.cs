using System;
using UnityEngine;
using UnityEngine.UI;

public class RefrigeratorUI : MonoBehaviour
{
    public Text stateId;
    public Text grade;
    public Text hp;
    public Text attack;
    public Text defense;
    public Text speed;

    public Text ingredientId;
    public Text recipeId;

    public Button FoodCreateButton;

    public Toggle toggleButton;

    private GameObject RecipePanel;
    private FoodGenerator foodGenerator;

    public void SetRefrigeratorData(Refrigerator refrigerator)
    {
        stateId.text = refrigerator.stateId.ToString();
        grade.text = refrigerator.grade;
        hp.text = refrigerator.hp.ToString();
        attack.text = refrigerator.attack.ToString();
        defense.text = refrigerator.defense.ToString();
        speed.text = refrigerator.speed.ToString();

        ingredientId.text = refrigerator.ingredientId.ToString();
        recipeId.text = refrigerator.recipeId.ToString();

        toggleButton.onValueChanged.AddListener(delegate { SendStateId(); });
    }

    private void SendStateId()
    {
        RecipePanel = GameObject.Find("RecipePanel");
        foodGenerator = RecipePanel.GetComponent<FoodGenerator>();

        if (foodGenerator.refrigeratorIds[1] != null)
        {
            toggleButton.isOn = false;
        }

        if (toggleButton.isOn)
        {
            for (int i = 0; i < foodGenerator.refrigeratorIds.Length; i++)
            {
                if (foodGenerator.refrigeratorIds[i] == null)
                {
                    foodGenerator.refrigeratorIds[i] = stateId.text;
                    break;
                }
            }
        }
        else
        {
            for (int i = 0; i < foodGenerator.refrigeratorIds.Length; i++)
            {
                if (foodGenerator.refrigeratorIds[i] == stateId.text)
                {
                    foodGenerator.refrigeratorIds[i] = null;
                    break;
                }
            }
        }
    }
}
