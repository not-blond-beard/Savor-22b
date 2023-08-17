using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SeedStateScript : MonoBehaviour
{
    [SerializeField]
    public Button linkedButton;
    [SerializeField]
    public string stateId { get; set; }
    [SerializeField]
    public string seedId { get; set; }
}
