using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GraphQlClient.Core;
using GraphQlClient.EventCallbacks;
using UnityEngine.Networking;

using TMPro; // Textmeshpro


public class CreateSeed : MonoBehaviour
{
    public string pkHex = "eda6ef63ae945cd15572fcf7d6635a8b3f8d86e85b57a353b482bc82c7fd2ad4";

    public GameObject loading;

    public GraphApi SavorReference;

    public TMP_Text mutationDisplay;

    public async void createNewSeed()
    {
        //loading.SetActive(true);
        //Gets the needed query from the Api Reference
        GraphApi.Query createSeed = SavorReference.GetQueryByName("CreateNewSeed", GraphApi.Query.Type.Mutation);

        //Converts the JSON object to an argument string and sets the queries argument
        createSeed.SetArgs(new { privateKeyHex = "eda6ef63ae945cd15572fcf7d6635a8b3f8d86e85b57a353b482bc82c7fd2ad4" });

        //Performs Post request to server
        UnityWebRequest request = await SavorReference.Post(createSeed);

        //loading.SetActive(false);

        mutationDisplay.text = HttpHandler.FormatJson(request.downloadHandler.text);

    }

}
