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
    string samplejson = @"
        {
            ""type"": ""data"",
            ""id"": ""default"",
            ""payload"": {
                ""data"": {
                    ""inventory"": {
                        ""seedStateList"": [
                            {
                                ""stateId"": ""359a2ee0-8786-4093-b92a-a1d1f9eb735b"",
                                ""seedId"": 5
                            },
                            {
                                ""stateId"": ""c38273ea-4d54-4cbf-8559-7e60801f18ab"",
                                ""seedId"": 4
                            }
                        ]
                    }
                }
            }
        }";


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

        //SubscribeAwaiter();



        //CancelSubscribe();
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

    }

    // Data parsing and getting data from JSON string
    public void ParseJson()
    {
        string resultText = "";
        JObject json = JObject.Parse(jsontext);
        Debug.Log(json["payload"]["data"]["inventory"]["seedStateList"][0]["stateId"]);
        Debug.Log(json["payload"]["data"]["inventory"]["seedStateList"][0]["seedId"]);
        Debug.Log(json["payload"]["data"]["inventory"]["seedStateList"][1]["stateId"]);
        Debug.Log(json["payload"]["data"]["inventory"]["seedStateList"][1]["seedId"]);
        for (int i = 0; i < json.Count - 1; i++)
        {
            string state = json["payload"]["data"]["inventory"]["seedStateList"][i]["stateId"].ToString();
            string seed = json["payload"]["data"]["inventory"]["seedStateList"][i]["seedId"].ToString();
            string result = "State: " + state + "\nSeed: " + seed;
            resultText = string.Concat(resultText, result, "\n");
        }

        seedDisplay.text = resultText;
    }



    [System.Serializable]
    public class SeedState
    {
        public string stateId;
        public int seedId;

    }

    public async Task SubscribeAwaiter()
    {
        Subscribe();

        string text = jsontext;

        await JsonController(text);
    }

    public async Task JsonController(string text)
    {
        if (text != null)
        {
            ParseJson();

            await Task.Delay(1000);
        }
    }







}
