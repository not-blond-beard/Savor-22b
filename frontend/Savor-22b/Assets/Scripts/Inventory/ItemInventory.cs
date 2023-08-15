using System.Net.WebSockets;
using GraphQlClient.Core;
using GraphQlClient.EventCallbacks;
using UnityEngine;


public class ItemInventory : MonoBehaviour
{
    public static string SocketId = nameof(ItemInventory);

    public GameObject Loading;

    [Header("API")]
    public GraphApi svrReference;
    public string address;

    [Header("UI Prefabs")]
    public GameObject seedPrefab;
    public GameObject ingredientPrefab;
    public GameObject foodPrefab;

    [Header("UI Containers")]
    public RectTransform seedContent;
    public RectTransform ingredientContent;
    public RectTransform foodContent;

    private ClientWebSocket clientWebSocket;
    private Event<OnSubscriptionDataReceived>.EventListener socketListener;

    private void OnEnable(){
        socketListener = SocketDataReceiver.Receiver(SocketId, DisplayData);
        OnSubscriptionDataReceived.RegisterListener(socketListener);
    }

    private void Start(){
        Subscribe();
    }

    private void OnDisable(){
        OnSubscriptionDataReceived.UnregisterListener(socketListener);
    }

    private void resetUIElements(){
        foreach (Transform child in seedContent){
            Destroy(child.gameObject);
        }

        foreach (Transform child in ingredientContent){
            Destroy(child.gameObject);
        }

        foreach (Transform child in foodContent){
            Destroy(child.gameObject);
        }

        if (Loading.activeSelf) {
            Loading.SetActive(false);
        }
    }

    private void DrawSeedList(Seed[] seedStateList){

        foreach (Seed seed in seedStateList){
            GameObject seedUI = Instantiate(seedPrefab, seedContent);
            SeedUI seedUIScript = seedUI.GetComponent<SeedUI>();

            seedUIScript.SetSeedData(seed);
        }
    }

    private void DrawIngredientList(Refrigerator[] refrigeratorStateList){

    }

    private void DrawFoodList(Refrigerator[] foodStateList){

    }

    public void DisplayData(OnSubscriptionDataReceived subscriptionDataReceived){
        Inventory inventory = Inventory.CreateFromJSON(subscriptionDataReceived.data);

        resetUIElements();

        DrawSeedList(inventory.seedStateList);
        // DrawIngredientList(Do stuff);
        // DrawFoodList(Do stuff);
    }

    public async void Subscribe(){
        Loading.SetActive(true);

        try {
            GraphApi.Query query = svrReference.GetQueryByName("GetInventoryState", GraphApi.Query.Type.Subscription);
            query.SetArgs(new { address });

            clientWebSocket = await svrReference.Subscribe(query, SocketId);
        }
        catch (System.Exception e){
            Debug.Log(e);

            Loading.SetActive(false);
        }
    }

    public void CancelSubscribe(){
        svrReference.CancelSubscription(clientWebSocket);
    }
}