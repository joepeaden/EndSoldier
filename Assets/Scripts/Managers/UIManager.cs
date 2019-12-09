using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public Text waveText;

    void Start()
    {
        if(instance == null)
            instance = this;

        waveText = GameObject.Find("WaveText").GetComponent<Text>();;
    }

    public void UpdateWaveUI(int wave)
    {
        waveText.text = "Wave: " + wave; 
    } 
}
