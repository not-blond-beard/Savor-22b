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

using TMPro; // Textmeshpro

public class ReadSeed : MonoBehaviour
{
    public GameObject loading;

    public GraphApi SavorReference;

    // Subscription Field
    public TMP_Text seedDisplay;
    public TMP_Text subscriptionDisplay;
    public string jsontext;

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
    }

    public async void Subscribe()
    {
        loading.SetActive(true);

        GraphApi.Query readSeed = SavorReference.GetQueryByName("ReadAllSeed", GraphApi.Query.Type.Subscription);

        readSeed.SetArgs(new { address = "0x53103C2D7875D2f5f02AeC3075155e268a6e3A94" });
        cws = await SavorReference.Subscribe(readSeed, "default");

        Debug.Log("Subscribe Activated");
    }

    public void DisplayData(OnSubscriptionDataReceived subscriptionDataReceived)
    {
        Debug.Log("I was called");
        subscriptionDisplay.text = HttpHandler.FormatJson(subscriptionDataReceived.data);
        jsontext = subscriptionDisplay.text;
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
    }

    // Data parsing and getting data from JSON string
    public void ParseJson()
    {
        string resultText = "";
        JObject json = JObject.Parse(jsontext);
        // Debug.Log(json["payload"]["data"]["inventory"]["seedStateList"][0]["stateId"]);
        // Debug.Log(json["payload"]["data"]["inventory"]["seedStateList"][0]["seedId"]);


        SeedState[] seedStatesArray = new SeedState[json.Count - 1];

        for (int i = 0; i < json.Count - 1; i++)
        {
            seedStatesArray[i] = new SeedState();

            string state = json["payload"]["data"]["inventory"]["seedStateList"][i]["stateId"].ToString();
            string seed = json["payload"]["data"]["inventory"]["seedStateList"][i]["seedId"].ToString();
            string result = "State: " + state + "\nSeed: " + seed;

            seedStatesArray[i].stateId = state;
            seedStatesArray[i].seedId = seed;

            resultText = string.Concat(resultText, result, "\n");


            Debug.Log("Stored : " + seedStatesArray[0].stateId);
            Debug.Log("Stored : " + seedStatesArray[0].seedId);
        }

        seedDisplay.text = resultText;
        CancelSubscribe();

    }

    [System.Serializable]
    public class SeedState
    {
        public string stateId;
        public string seedId;
    }
}
