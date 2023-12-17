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


public class ReadIngredient : MonoBehaviour
{
    public string pkHex = "eda6ef63ae945cd15572fcf7d6635a8b3f8d86e85b57a353b482bc82c7fd2ad4";

    public GameObject loading;

    public GraphApi SavorReference;

    public TMP_Text subscriptionDisplay;

    // Subscription Method
    public string jsontext;
    private IngredientState[] ingredientStatesArray;

    private ClientWebSocket cws;

    public int ingredientListCount = 0;

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

    public async void GetAllIngredients()
    {
        Subscribe();
    }

    public async void Subscribe()
    {
        loading.SetActive(true);

        GraphApi.Query readIngredient = SavorReference.GetQueryByName("ReadAllIngredient", GraphApi.Query.Type.Subscription);

        readIngredient.SetArgs(new { address = "0x53103C2D7875D2f5f02AeC3075155e268a6e3A94" });
        cws = await SavorReference.Subscribe(readIngredient, "default");

        Debug.Log("Subscribe Activated");
    }

    public void DisplayData(OnSubscriptionDataReceived subscriptionDataReceived)
    {
        Debug.Log("I was called");
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
    }

    public void ParseJson()
    {
        JObject json = JObject.Parse(jsontext);

        JArray ingredientStateList = json["payload"]["data"]["inventory"]["refrigeratorStateList"] as JArray;
        int count = ingredientStateList.Count;

        ingredientListCount = count;

        ingredientStatesArray = new IngredientState[count + 1];

        for (int i = 0; i < count; i++)
        {
            ingredientStatesArray[i] = new IngredientState();

            string state = json["payload"]["data"]["inventory"]["refrigeratorStateList"][i]["stateId"].ToString();
            string ingredient = json["payload"]["data"]["inventory"]["refrigeratorStateList"][i]["ingredientId"].ToString();
            string result = "ingredientId: " + ingredient + ", stateId: " + state;

            ingredientStatesArray[i].stateId = state;
            ingredientStatesArray[i].ingredientId = ingredient;

            Debug.Log(result);
            DisplayResults(result);
        }
    }

    public HashSet<string> displayedResults = new(); // 중복된 결과를 체크할 컨테이너

    public void DisplayResults(string result)
    {
        if (!displayedResults.Contains(result))
        {
            displayedResults.Add(result);

            string temp = subscriptionDisplay.text + "\n" + result;
            subscriptionDisplay.text = temp;
        }
    }



    // Start is called before the first frame update
    void Start()
    {
        subscriptionDisplay.text = "";
    }

    [System.Serializable]
    public class IngredientState
    {
        public string stateId;
        public string ingredientId;
    }
}

