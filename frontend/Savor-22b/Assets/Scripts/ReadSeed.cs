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

using TMPro; // Textmeshpro

public class ReadSeed : MonoBehaviour
{
    public GameObject loading;

    public GraphApi SavorReference;

    // Subscription Field
    public TMP_Text seedDisplay;
    public TMP_Text subscriptionDisplay;
    
    private ClientWebSocket cws;

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

        //Prettify2();

        //CancelSubscribe();
    }

    public async void Subscribe()
    {
        loading.SetActive(true);

        GraphApi.Query readSeed = SavorReference.GetQueryByName("ReadAllSeed", GraphApi.Query.Type.Subscription);

        readSeed.SetArgs(new{address = "0x53103C2D7875D2f5f02AeC3075155e268a6e3A94"});
        cws = await SavorReference.Subscribe(readSeed, "default");

    }

    public void DisplayData(OnSubscriptionDataReceived subscriptionDataReceived)
    {
        Debug.Log("I was called");
        subscriptionDisplay.text = HttpHandler.FormatJson(subscriptionDataReceived.data);
    }

    public void CancelSubscribe()
    {
        if (cws != null)
            {
                SavorReference.CancelSubscription(cws);
            }

    }

    // Data parsing and getting data from JSON string















    public void Prettify2()
    {
        TextAsset data = new TextAsset(subscriptionDisplay.text);

        JObject obj = JObject.Parse(data.text);

        JToken dataToken = obj["data"];

        JToken readAllSeedToken = dataToken["ReadAllSeed"];

        JToken seedToken = readAllSeedToken["seed"];

        JToken stateToken = seedToken["state"];

        string stateId = stateToken["id"].ToString();
        int seedId = (int)seedToken["id"];

        seedDisplay.text = "Seed ID: " + seedId + "\n" + "State ID: " + stateId;
    }

    public void Prettify1()
    {
        TextAsset data = new TextAsset(subscriptionDisplay.text);



        SeedState seedState = JsonUtility.FromJson<SeedState>(data.text);

        int seedId = seedState.seedId;
        string stateId = seedState.stateId;
   
        seedDisplay.text = "Seed ID: " + seedId + "\n" + "State ID: " + stateId;
    }

    [System.Serializable]
    public class SeedState
    {
        public string stateId;
        public int seedId;

    }

   






}
