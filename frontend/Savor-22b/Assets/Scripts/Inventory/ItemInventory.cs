using System.Net.WebSockets;
using System.Collections.Generic;
using GraphQlClient.Core;
using GraphQlClient.EventCallbacks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class ItemInventory : MonoBehaviour
{
    public static string SocketId = nameof(ItemInventory);

    public GameObject Loading;

    [Header("API")]
    public GraphApi svrReference;
    public string address;
    public string privateKeyHex;

    [Header("UI Prefabs")]
    public GameObject seedPrefab;
    public GameObject ingredientPrefab;
    public GameObject foodPrefab;

    [Header("UI Containers")]
    public RectTransform seedContent;
    public RectTransform ingredientContent;
    public RectTransform foodContent;

    public Button recipeButton;
    public Button combineButton;

    private bool isRecipeSelector = false;

    List<string> selectedStateIds;


    private ClientWebSocket clientWebSocket;
    private Event<OnSubscriptionDataReceived>.EventListener socketListener;

    private void Start()
    {
        Subscribe();

        SetRecipeSelectorButton();
    }
    private void OnEnable()
    {
        socketListener = SocketDataReceiver.Receiver(SocketId, DisplayData);
        OnSubscriptionDataReceived.RegisterListener(socketListener);
    }

    private void OnDisable()
    {
        OnSubscriptionDataReceived.UnregisterListener(socketListener);
    }

    private void resetUIElements()
    {
        foreach (Transform child in seedContent)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in ingredientContent)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in foodContent)
        {
            Destroy(child.gameObject);
        }

        if (Loading.activeSelf)
        {
            Loading.SetActive(false);
        }
    }

    private void DrawSeedList(Seed[] seedStateList)
    {

        foreach (Seed seed in seedStateList)
        {
            GameObject seedUI = Instantiate(seedPrefab, seedContent);
            SeedUI seedUIScript = seedUI.GetComponent<SeedUI>();

            seedUIScript.SetSeedData(seed);
        }
    }

    private void DrawIngredientList(Refrigerator[] refrigeratorStateList)
    {
        foreach (Refrigerator refrigerator in refrigeratorStateList)
        {
            if (refrigerator.ingredientId.HasValue)
            {
                GameObject ingredientUI = Instantiate(ingredientPrefab, ingredientContent);
                RefrigeratorUI ingredientUIScript = ingredientUI.GetComponent<RefrigeratorUI>();


                ingredientUIScript.SetRefrigeratorData(refrigerator);

                GameObject toggle = ingredientUI.transform.Find("EdibleSelector").gameObject;
                if (isRecipeSelector)
                {
                    toggle.SetActive(true);
                }

            }
        }
    }

    private void DrawFoodList(Refrigerator[] foodStateList)
    {
        foreach (Refrigerator refrigerator in foodStateList)
        {
            if (refrigerator.recipeId.HasValue)
            {
                GameObject foodUI = Instantiate(foodPrefab, foodContent);
                RefrigeratorUI foodUIScript = foodUI.GetComponent<RefrigeratorUI>();

                foodUIScript.SetRefrigeratorData(refrigerator);

                GameObject toggle = foodUI.transform.Find("EdibleSelector").gameObject;
                if (isRecipeSelector)
                {
                    toggle.SetActive(true);
                }
            }
        }
    }

    public void DisplayData(OnSubscriptionDataReceived subscriptionDataReceived)
    {
        Inventory inventory = Inventory.CreateFromJSON(subscriptionDataReceived.data);

        resetUIElements();

        DrawSeedList(inventory.seedStateList);
        DrawIngredientList(inventory.refrigeratorStateList);
        DrawFoodList(inventory.refrigeratorStateList);
    }

    public async void Subscribe()
    {
        Loading.SetActive(true);

        try
        {
            GraphApi.Query query = svrReference.GetQueryByName("GetInventoryState", GraphApi.Query.Type.Subscription);
            query.SetArgs(new { address });

            clientWebSocket = await svrReference.Subscribe(query, SocketId);
        }
        catch (System.Exception e)
        {
            Debug.Log(e);

            Loading.SetActive(false);
        }
    }

    public void CancelSubscribe()
    {
        svrReference.CancelSubscription(clientWebSocket);
    }

    // Create New Seed
    public async void CreateNewSeed()
    {
        GraphApi.Query query = svrReference.GetQueryByName("CreateNewSeed", GraphApi.Query.Type.Mutation);
        query.SetArgs(new { privateKeyHex });
        UnityWebRequest request = await svrReference.Post(query);
    }


    // Recipe edible selector
    private void SetRecipeSelectorButton()
    {
        recipeButton.onClick.AddListener(ToggleRecipeSelector);
        combineButton.onClick.AddListener(ResetSelectors);
    }

    private void ToggleRecipeSelector()
    {
        if (!isRecipeSelector)
        {
            //CancelSubscribe();
            ActivateRecipeSelector();
        }
        else
        {
            //Subscribe();
            DeactivateRecipeSelector();
        }
    }



    private void ActivateRecipeSelector()
    {
        GameObject[] edibleObjects = GameObject.FindGameObjectsWithTag("Edible");
        foreach (GameObject edibleObject in edibleObjects)
        {
            Transform edibleToggleTransform = edibleObject.transform.Find("EdibleSelector");
            if (edibleToggleTransform != null)
            {
                GameObject edibleToggle = edibleToggleTransform.gameObject;
                edibleToggle.SetActive(true);
            }
        }
        isRecipeSelector = true;
    }

    private void DeactivateRecipeSelector()
    {
        GameObject[] edibleObjects = GameObject.FindGameObjectsWithTag("Edible");
        foreach (GameObject edibleObject in edibleObjects)
        {
            Transform edibleToggleTransform = edibleObject.transform.Find("EdibleSelector");
            if (edibleToggleTransform != null)
            {
                GameObject edibleToggle = edibleToggleTransform.gameObject;
                edibleToggle.SetActive(false);
            }
        }
        isRecipeSelector = false;
    }

    private void ResetSelectors()
    {
        GameObject[] edibleObjects = GameObject.FindGameObjectsWithTag("Edible");
        foreach (GameObject edibleObject in edibleObjects)
        {
            Transform edibleToggleTransform = edibleObject.transform.Find("EdibleSelector");
            if (edibleToggleTransform != null)
            {
                GameObject edibleToggle = edibleToggleTransform.gameObject;
                Toggle toggle = edibleToggle.GetComponent<Toggle>();
                toggle.isOn = false;
            }
        }
        DeactivateRecipeSelector();
    }

    public List<string> GetStateIds()
    {
        selectedStateIds = new List<string>();
        GameObject[] edibleObjects = GameObject.FindGameObjectsWithTag("Edible");
        foreach (GameObject edibleObject in edibleObjects)
        {
            RefrigeratorUI selectedObject = edibleObject.GetComponent<RefrigeratorUI>();
            if (selectedObject.GetStateId() != null)
            {
                selectedStateIds.Add(selectedObject.GetStateId());
            }

        }

        return selectedStateIds;
    }

}

