using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Manages flow of game; e.g. main menu -> game -> scoreboard, etc.

public class FlowManager : MonoBehaviour
{
    // "singleton"
    public static FlowManager instance;

    // buttons
    [SerializeField]
    private Button resetButton;
    [SerializeField]
    private Button startButton;

    void Start()
    {
        if(instance == null)
            instance = this;
    }

    public void AssignResetButton(Button btn)
    {
        resetButton = btn;
    }

    public void StartGame()
    {
        SceneManager.LoadSceneAsync(1);
    }

    public void ResetGame()
	{
		// resetButton.gameObject.SetActive(false);
		SceneManager.LoadSceneAsync(1);
        //Scoreboard.instance.ResetPoints();
	}

    public void GameOver()
    {
        // load scoreboard scene
        SceneManager.LoadSceneAsync(2);
    }

    public void BeginResetSequence()
    {
        resetButton.gameObject.SetActive(true);
    }
}
