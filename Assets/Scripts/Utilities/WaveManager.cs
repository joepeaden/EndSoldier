using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WaveManager : MonoBehaviour
{
    // ya know. I still haven't really thought through when a Singleton is the right move vs. a static class. Or other methods.
    // I'll need to give that some thought. Obviously there are a lot of design inconsistencies in this project. \_(o_o)_/ Deal with it.
    //                                                                                                                ^ that's my attempt at a shrug
    public static WaveManager Instance { get { return _instance; } }
    private static WaveManager _instance;

    public static UnityEvent OnPrepForNextWave = new UnityEvent();
    public static int totalEnemiesAlive;

    [SerializeField]
    private WaveData waveData;

    private void Start()
    {
        if (_instance != null)
        {
            Debug.LogWarning("More than one WaveManager instance! Deleting this one.");
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }

        // prep first wave
        StartCoroutine(BeginFirstWave());
    }

    private IEnumerator BeginFirstWave()
    {
        yield return new WaitForSeconds(waveData.firstWaveDelay);
        StartCoroutine(WaitForWaveDelay());
    }

    private IEnumerator WaitForWaveDelay()
    {
        OnPrepForNextWave.Invoke();
        yield return new WaitForSeconds(waveData.waveDelay);

        EnemySpawner.NextWave();

        // wait until something is spawned before going to next wave
        yield return new WaitUntil(() => totalEnemiesAlive > 0);
        StartCoroutine("CheckForNextWave");
    }

    private IEnumerator CheckForNextWave()
    {
        while (totalEnemiesAlive > 0)
        {
            yield return new WaitForSeconds(1f);
        }

        //if (currentWaveIndex > waves.Length)
        //{
        //    Debug.Log("You won!");
        //}
        //else
        //{
        StartCoroutine(WaitForWaveDelay());
        //}
    }

    public WaveData GetWaveData()
    {
        return waveData;
    }
}
