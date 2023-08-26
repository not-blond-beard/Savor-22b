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

    public async void CreateNewFood()
    {
        GetDatas();
        GraphApi.Query query = svrReference.GetQueryByName(QueryNames.CREATE_FOOD, GraphApi.Query.Type.Mutation);

        // argument must set exact same name in schema
        query.SetArgs(new { privateKeyHex = privateKeyHex, recipeID = recipeId, refrigeratorStateIDs = refrigeratorIds });
        UnityWebRequest request = await svrReference.Post(query);

        ResetRecipeSelectors();
    }


    private void ResetRecipeSelectors()
    {
        GameObject[] recipeObjects = GameObject.FindGameObjectsWithTag("Recipe");
        foreach (GameObject recipeObject in recipeObjects)
        {
            Transform recipeToggleTransform = recipeObject.transform.Find("EdibleSelector");
            if (recipeToggleTransform != null)
            {
                GameObject recipeToggle = recipeToggleTransform.gameObject;
                Toggle toggle = recipeToggle.GetComponent<Toggle>();
                toggle.isOn = false;
            }
        }
    }

    private void GetDatas()
    {
        // get private key, refrigerator ids
        ItemInventory inventory = GameObject.Find("InventoryStatePanel").GetComponent<ItemInventory>();
        privateKeyHex = inventory.privateKeyHex;
        refrigeratorIds = inventory.GetStateIds().ToArray();

        //get recipe id
        RecipeRenderer recipeRenderer = GameObject.Find("RecipePanel").GetComponent<RecipeRenderer>();
        recipeId = recipeRenderer.GetRecipeId();
    }
}
