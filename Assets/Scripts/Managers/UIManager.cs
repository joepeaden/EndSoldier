using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public Text waveText;
    public SimpleHealthBar healthBar;

    void Start()
    {
        if(instance == null)
            instance = this;

        SceneManager.sceneLoaded += FindButtons;
        FindButtons(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    // eventaully remove this method, check for null in each individual method
    // like in healthbar
     private void FindButtons(Scene scene, LoadSceneMode mode)
    {
        switch(scene.name)
        {
            case "Battle":
                
                // Reset Button 
                // Button debugResetButton = GameObject.Find("ResetButton").GetComponent<Button>();
                // debugResetButton.onClick.AddListener(FlowManager.instance.ResetGame);
                // FlowManager.instance.AssignResetButton(debugResetButton);
                // debugResetButton.gameObject.SetActive(false);

                // Wave Text (UI)
                waveText = GameObject.Find("WaveText").GetComponent<Text>();

                break;

            case "Menu":

                // Start Button
                Button startButton = GameObject.Find("StartButton").GetComponent<Button>();
                startButton.onClick.AddListener(FlowManager.instance.StartGame);

                break;

            case "Scores":

                // Score Text 
                Text scoreText = GameObject.Find("ScoreText").GetComponent<Text>();
                scoreText.text = "FINAL SCORE: " + Scoreboard.instance.GetPoints();

                // Reset Button 
                Button resetButton = GameObject.Find("ResetButton").GetComponent<Button>();
                resetButton.onClick.AddListener(FlowManager.instance.ResetGame);
                // FlowManager.instance.AssignResetButton(resetButton);

                break;
        }
    }

    public void UpdateWaveUI(int wave)
    {
        waveText.text = "Wave: " + wave; 
    } 

    public void UpdateHealthBar(int currentHP, int maxHP)
    {
        if(healthBar == null)
        {
            healthBar = GameObject.FindGameObjectWithTag("HealthBar").GetComponent<SimpleHealthBar>();
        }

        healthBar.UpdateBar(currentHP, maxHP); 
        
        float healthPercentage = (float)currentHP/(float)maxHP;

        if (healthPercentage >= .66)
            healthBar.UpdateColor(Color.green);
        else if (healthPercentage >= .33)
            healthBar.UpdateColor(Color.yellow);
        else
            healthBar.UpdateColor(Color.red);
    


    }
}
