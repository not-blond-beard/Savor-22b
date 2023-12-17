using System;
using UnityEngine;
using UnityEngine.UI;
using GraphQlClient.Core;
using GraphQlClient.EventCallbacks;
using UnityEngine.Networking;

public class SeedUI : MonoBehaviour
{
    [Header("API")]
    public GraphApi svrReference;
    public string privateKeyHex;

    public Text seedStateId;
    public Text seedId;

    public Button IngredientCreateButton;

    public void SetSeedData(Seed seed)
    {
        seedStateId.text = seed.stateId.ToString();
        seedId.text = seed.seedId.ToString();

        SetIngredientCreateButton(seed.stateId);
    }

    public async void CreateIngredient(Guid seedStateId)
    {
        GraphApi.Query query = svrReference.GetQueryByName(QueryNames.CREATE_INGREDIENT, GraphApi.Query.Type.Mutation);
        query.SetArgs(new { privateKeyHex, seedStateId });
        UnityWebRequest request = await svrReference.Post(query);
    }

    public void SetIngredientCreateButton(Guid seedStateId)
    {
        IngredientCreateButton.onClick.AddListener(() => CreateIngredient(seedStateId));
    }
}
