using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events; 

public class Scoreboard : MonoBehaviour
{
    public static UnityEvent<int> OnScoreUpdated = new UnityEvent<int>();

    public static int totalScore;

    private void Start()
    {
        Enemy.OnEnemyKilled.AddListener(AddPoints);
    }

    private void OnDestroy()
    {
        Enemy.OnEnemyKilled.RemoveListener(AddPoints);
    }

    private void AddPoints(int points)
    {
        totalScore += points;
        OnScoreUpdated.Invoke(totalScore);
    }
}
