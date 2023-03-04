using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Author: Joseph Peaden

/// <summary>
/// Has references to the player.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static UnityEvent OnPrepForNextWave = new UnityEvent();

    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }
    public static bool isSlowMotion;
    // should be in SO probably
    public static float slowMotionSpeed = .5f;
    // this too
    public float waveDelay;
    public float firstWaveDelay;

    public WaveData[] waves;
    public static int currentWaveIndex;
    public static int totalEnemiesAlive;

    [SerializeField] private GameObject playerGO;
    private Player player;

    [SerializeField] private GameObject reticleGO;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Debug.Log("More than one Game Manager, deleting one.");
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

        if (!playerGO)
        {
            Debug.LogWarning("Player not assigned, finding by tag.");
            playerGO = GameObject.FindGameObjectWithTag("Player");

            if (!playerGO)
            {
                Debug.LogWarning("Player not found by tag.");
            }
        }

        player = playerGO.GetComponent<Player>();
    }

    private void Start()
    {
        // prep first wave
        StartCoroutine(WaitForWaveDelay());
    }

    private IEnumerator WaitForWaveDelay()
    {
        OnPrepForNextWave.Invoke();
        yield return new WaitForSeconds(waveDelay);

        EnemySpawner.NextWave(waves[currentWaveIndex]);

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

        currentWaveIndex++;
        if (currentWaveIndex > waves.Length)
        {
            Debug.Log("You won!");
        }
        else
        {
            StartCoroutine(WaitForWaveDelay());
        }
    }

    public Player GetPlayerScript()
    {
        return player;
    }

    public GameObject GetPlayerGO()
    {
        return playerGO;
    }

    public GameObject GetReticleGO()
    {
        return reticleGO;
    }

    public void StartSlowMotion(float secondsToWait)
    {
        StartCoroutine(StartSlowMotionRoutine(secondsToWait));
    }

    private IEnumerator StartSlowMotionRoutine(float secondsToWait)
    {
        yield return new WaitForSeconds(secondsToWait);

        isSlowMotion = true;
        Time.timeScale = slowMotionSpeed;

        yield return new WaitForSeconds(2.75f);

        isSlowMotion = false;
        Time.timeScale = 1f;
    }
}
