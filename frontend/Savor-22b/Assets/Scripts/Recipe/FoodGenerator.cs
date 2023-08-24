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
    public string[] refrigeratorIds = new string[2];

    void Start()
    {
        refrigeratorIds[0] = null;
        refrigeratorIds[1] = null;
    }

}
