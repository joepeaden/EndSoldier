using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    private Text waveText;
    private Text pointsText; 
    private Text weaponText;
    private SimpleHealthBar healthBar;


    void Start()
    {
        if(instance == null)
            instance = this;
    }

    public void UpdatePointsUI(int pts)
    {
        if(pointsText == null)
        {
            pointsText = GameObject.FindGameObjectWithTag("PointsText").GetComponent<Text>();
        }

        pointsText.text = "Points: " + pts; 
    } 

    public void UpdateWaveUI(int wave)
    {
        if(waveText == null)
        {
            waveText = GameObject.FindGameObjectWithTag("WaveText").GetComponent<Text>();
        }

        waveText.text = "Wave: " + wave; 
    } 

    public void UpdateWeaponInfoUI(string weaponName, int ammo)
    {
        if(weaponText == null)
        {
            weaponText = GameObject.FindGameObjectWithTag("WeaponText").GetComponent<Text>();
        }

        weaponText.text = weaponName + ": " + ammo; 
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
