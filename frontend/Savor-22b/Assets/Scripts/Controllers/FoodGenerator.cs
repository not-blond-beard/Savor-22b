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
    public string[] refrigeratorIds;

    public GameObject recipeRendererObject;
    public GameObject inventoryObject;
    public RecipeRenderer recipeRenderer;
    public ItemInventory inventory;

    public async void CreateNewFood()
    {
        GetDatas();
        if (recipeId == 0)
        {
            Debug.Log("Please select valid recipe");
            return;
        }
        if (refrigeratorIds.Length == 0)
        {
            Debug.Log("Please select refrigerator");
        }

        GraphApi.Query query = svrReference.GetQueryByName(QueryNames.CREATE_FOOD, GraphApi.Query.Type.Mutation);

        // argument must set exact same name in schema
        query.SetArgs(new { privateKeyHex = privateKeyHex, recipeID = recipeId, refrigeratorStateIDs = refrigeratorIds });
        UnityWebRequest request = await svrReference.Post(query);

        recipeRenderer.ResetRecipeSelectors();
    }

    private void GetDatas()
    {
        // get private key, refrigerator ids
        privateKeyHex = inventory.privateKeyHex;
        //refrigeratorIds = inventory.GetStateIds().ToArray();
        refrigeratorIds = inventory.selectedStateIds.ToArray();

        //get recipe id
        recipeId = recipeRenderer.selectedRecipeId;
        Debug.Log("Number of Edibles selected: " + refrigeratorIds.Length + "\n" + "recipe Id: " + recipeId);
    }

    void Start()
    {
        //Setting inventory, recipe scripts
        inventory = inventoryObject.GetComponent<ItemInventory>();
        recipeRenderer = recipeRendererObject.GetComponent<RecipeRenderer>();
    }
}
