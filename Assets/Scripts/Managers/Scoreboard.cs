using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using UnityEngine.UI;

// for keeping track of player score 
// and saving/displaying scoreboard

public class Scoreboard : MonoBehaviour
{
    public static Scoreboard instance;
    
    private int points;
    // private Text finalScore;

    void Start()
    {
        if(instance == null)
            instance = this;
    }

    public int GetPoints()
    {
        return points;
    }

    public void AddPoints(int points)
    {
        this.points += points;
    }

    public void ResetPoints()
    {
        points = 0;
    }

}
