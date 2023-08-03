using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GraphQlClient.Core;
using GraphQlClient.EventCallbacks;
using UnityEngine.Networking;

using TMPro; // Textmeshpro


public class CreateIngredient : MonoBehaviour
{
    public string pkHex = "eda6ef63ae945cd15572fcf7d6635a8b3f8d86e85b57a353b482bc82c7fd2ad4";
    public string seedSId = "f2076c04-bd3d-4ed3-8475-63fbe84fd124";

    public GameObject loading;

    public GraphApi SavorReference;

    public TMP_InputField inputString;
    public string newIngredientName;
    public TMP_Text mutationDisplay;

    public async void createNewIngredient()
    {
        loading.SetActive(true);
	//Gets the needed query from the Api Reference
        GraphApi.Query createIngredient = SavorReference.GetQueryByName("CreateNewIngredient", GraphApi.Query.Type.Mutation);
	
	//Converts the JSON object to an argument string and sets the queries argument"
        createIngredient.SetArgs(new{privateKeyHex = pkHex, seedStateId = seedSId});
        
	//Performs Post request to server
        UnityWebRequest request = await SavorReference.Post(createIngredient);

        loading.SetActive(false);

        mutationDisplay.text = HttpHandler.FormatJson(request.downloadHandler.text);
    
    }

    public void applyInput()
    {
        newIngredientName = inputString.GetComponent<TMP_InputField>().text;
    }


}
