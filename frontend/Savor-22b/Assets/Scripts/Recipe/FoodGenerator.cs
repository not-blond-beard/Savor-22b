using GraphQlClient.Core;
using GraphQlClient.EventCallbacks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class FoodGenerator : MonoBehaviour
{
    [Header("API")]
    public GraphApi svrReference;
    public string privateKeyHex;
    public int recipeId;
    public string[] refrigeratorIds = new string[2];

    public Button combineButton;

    public async void CreateNewFood()
    {
        GraphApi.Query query = svrReference.GetQueryByName(QueryNames.CREATE_FOOD, GraphApi.Query.Type.Mutation);

        // argument must set exact same name in schema
        query.SetArgs(new { privateKeyHex = privateKeyHex, recipeID = recipeId, refrigeratorStateIDs = refrigeratorIds });
        UnityWebRequest request = await svrReference.Post(query);
    }

    private void SetCombineButton()
    {
        combineButton.onClick.AddListener(CreateNewFood);
        InitalizeVariables();
    }

    private void InitalizeVariables()
    {
        // Initalize Variables
        refrigeratorIds[0] = null;
        refrigeratorIds[1] = null;
        recipeId = 0;
    }


    void Start()
    {
        InitalizeVariables();
        SetCombineButton();
    }

}
