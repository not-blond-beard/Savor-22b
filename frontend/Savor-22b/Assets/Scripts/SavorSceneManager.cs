using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SavorSceneManager : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void LoadCreateSeed()
    {
        LoadScene("CreateSeed");
    }

    public void LoadCreateIngredient()
    {
        LoadScene("CreateIngredient");
    }

}
