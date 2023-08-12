using System.Collections;
using System.Collections.Generic;
using System.Net.WebSockets;
using UnityEngine;
using UnityEngine.UI;

using GraphQlClient.Core;
using GraphQlClient.EventCallbacks;
using UnityEngine.Networking;

// data parsing
using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

using System.Threading.Tasks;
using System.Threading;

using TMPro; // Textmeshpro


public class CreateIngredient : MonoBehaviour
{
    public string pkHex = "eda6ef63ae945cd15572fcf7d6635a8b3f8d86e85b57a353b482bc82c7fd2ad4";

    public GameObject loading;

    public GraphApi SavorReference;

    public TMP_Text mutationDisplay;


    // Subscription Method
    public string jsontext;
    private SeedState[] seedStatesArray;

    private ClientWebSocket cws;

    // Seed List Fields
    public TMP_Text seedList;
    private GameObject seedListObject;
    public int seedListCount = 0;

    // Debug Log display
    public TMP_Text debugLog;

    // Create New Ingredient
    public async Task CreateNewIngredient(string seedstateId)
    {
        loading.SetActive(true);

        //Gets the needed query from the Api Reference
        GraphApi.Query createIngredient = SavorReference.GetQueryByName("CreateNewIngredient", GraphApi.Query.Type.Mutation);

        //Converts the JSON object to an argument string and sets the queries argument"
        createIngredient.SetArgs(new { privateKeyHex = pkHex, seedStateId = seedstateId });

        //Performs Post request to server
        UnityWebRequest request = await SavorReference.Post(createIngredient);

        loading.SetActive(false);

        mutationDisplay.text = HttpHandler.FormatJson(request.downloadHandler.text);

    }



    //Reading Subscription Data
    private void OnEnable()
    {
        Debug.Log("I was enabled");
        OnSubscriptionDataReceived.RegisterListener(DisplayData);
    }

    private void OnDisable()
    {
        Debug.Log("I was disabled");
        OnSubscriptionDataReceived.UnregisterListener(DisplayData);
    }

    public async void GetAllSeeds()
    {
        Subscribe();
    }


    public async void Subscribe()
    {
        loading.SetActive(true);

        GraphApi.Query readSeed = SavorReference.GetQueryByName("ReadAllSeed", GraphApi.Query.Type.Subscription);

        readSeed.SetArgs(new { address = "0x53103C2D7875D2f5f02AeC3075155e268a6e3A94" });
        cws = await SavorReference.Subscribe(readSeed, "default");

        Debug.Log("Subscribe Activated");
        DisplayDebugLog("Subscribe Activated");
    }

    public void DisplayData(OnSubscriptionDataReceived subscriptionDataReceived)
    {
        Debug.Log("I was called");
        DisplayDebugLog("DisplayData was called");
        jsontext = HttpHandler.FormatJson(subscriptionDataReceived.data);

        Debug.Log(jsontext);

        if (jsontext != null)
        {
            ParseJson();
        }
    }

    public void CancelSubscribe()
    {
        if (cws != null)
        {
            SavorReference.CancelSubscription(cws);
        }
        Debug.Log("Subscribe Cancelled");
        DisplayDebugLog("Subscribe Cancelled");
        DisplayDebugLog("Read Button Activated");
    }


    // Data parsing and getting data from JSON string
    public void ParseJson()
    {
        string resultText = "";
        JObject json = JObject.Parse(jsontext);

        JArray seedStateList = json["payload"]["data"]["inventory"]["seedStateList"] as JArray; // 수정된 부분
        int count = seedStateList.Count; // 수정된 부분

        seedListCount = count;

        seedStatesArray = new SeedState[count + 1];

        for (int i = 0; i < count; i++)
        {
            seedStatesArray[i] = new SeedState();

            string state = json["payload"]["data"]["inventory"]["seedStateList"][i]["stateId"].ToString();
            string seed = json["payload"]["data"]["inventory"]["seedStateList"][i]["seedId"].ToString();
            string result = "State: " + state + "\nSeed: " + seed;

            seedStatesArray[i].stateId = state;
            seedStatesArray[i].seedId = seed;

            resultText = string.Concat(resultText, result, "\n");
            Debug.Log(resultText);
            DisplayDebugLog(resultText);
        }
        // Cancel Subscibe if you want
        //CancelSubscribe();

        // Create List from parsed data
        ReadSeedList();
    }

    // Read Seed List via Subscription
    public void ReadSeedList()
    {
        Task.Delay(1000);
        CreateList();
    }

    // Create Seed List from seedStatesArray and display it on UI
    public GameObject canvasObject;
    public Transform locObject;

    public void CreateList()
    {

        // Instantiate the Seed List Prefab
        seedListObject = Resources.Load("Prefabs/SelectionPanel") as GameObject;

        for (int i = 0; i < seedListCount; i++)
        {
            GameObject exsistingObj = GameObject.Find("Seed " + i);
            if (exsistingObj == null)
            {
                GameObject gameObj = MonoBehaviour.Instantiate(seedListObject, new Vector3(0, 0, 0), Quaternion.identity);

                // Adding SeedStateScript to each list and setting its values
                gameObj.AddComponent<SeedStateScript>();
                SeedStateScript seedStateScript = gameObj.GetComponent<SeedStateScript>();
                seedStateScript.stateId = seedStatesArray[i].stateId;
                seedStateScript.seedId = seedStatesArray[i].seedId;

                // Setting Location and Size of Each list
                gameObj.transform.position = locObject.position + new Vector3(0, -i * 200, 0);
                gameObj.transform.rotation = locObject.rotation;
                gameObj.transform.localScale = locObject.localScale;
                gameObj.transform.SetParent(canvasObject.transform, false);
                gameObj.transform.localScale = new Vector3(1, 1, 1);

                // Link button infos
                Button button = gameObj.GetComponentInChildren<Button>();
                if (button != null)
                {
                    seedStateScript.linkedButton = button;
                    button.onClick.AddListener(() => HandleButtonClick(seedStateScript));
                }
                // Setting Names of each list
                gameObj.name = "Seed " + i;
                DisplayDebugLog(gameObj.name + "Created");

                // Display SeedState ID and Seed ID on each list
                var textArea = gameObj.transform.Find("SelectionInfo");
                textArea.GetComponent<TextMeshProUGUI>().text = "Seed ID : " + seedStatesArray[i].seedId + "\n" + "State ID : " + seedStatesArray[i].stateId;

            }

        }
    }

    private async void HandleButtonClick(SeedStateScript script)
    {
        if (script != null)
        {
            Debug.Log("Button from prefab " + script.linkedButton.name + " clicked. Seed ID: " + script.seedId + ", State ID: " + script.stateId);
        }
        else
        {
            Debug.LogWarning("Script not associated with any button.");
        }
        string SSID = script.stateId;
        await CreateNewIngredient(SSID);
        Debug.Log("Create New Ingredient from" + SSID);
        DisplayDebugLog("Create New Ingredient from" + SSID);
    }

    [System.Serializable]
    public class SeedState
    {
        public string stateId;
        public string seedId;
    }

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>


    void Start()
    {
        debugLog.text = "";

        Subscribe();


    }
    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {

    }

    // Debug Log display
    public void DisplayDebugLog(string log)
    {
        string temp = debugLog.text + "\n" + log;
        debugLog.text = temp;
    }

}


