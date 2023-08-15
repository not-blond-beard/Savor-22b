using System;
using UnityEngine;
using UnityEngine.UI;

public class SeedUI : MonoBehaviour
{
    public Text seedStateId;
    public Text seedId;

    public Button IngredientCreateButton;

    public void SetSeedData(Seed seed)
    {
        seedStateId.text = seed.stateId.ToString();
        seedId.text = seed.seedId.ToString();

        SetIngredientCreateButton(seed.stateId);
    }

    public void CreateIngredient(Guid seedStateId)
    {
        Debug.Log("Create ingredient");
    }

    public void SetIngredientCreateButton(Guid seedStateId)
    {
        IngredientCreateButton.onClick.AddListener(() => CreateIngredient(seedStateId));
    }
}
