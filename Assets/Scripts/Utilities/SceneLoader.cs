using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get { return _instance; } }
    private static SceneLoader _instance;

    public enum SceneList
    {
        StartMenu,
        House,
        FailMenu,
        Apartment
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Debug.Log("More than one SceneLoader, deleting one.");
            Destroy(gameObject);
            return;
        }

        _instance = this;
    }

    public void LoadScene(SceneList sceneToLoad, bool additive)
    {
        SceneManager.LoadScene(sceneToLoad.ToString(), additive ? LoadSceneMode.Additive : LoadSceneMode.Single);
    }
}
