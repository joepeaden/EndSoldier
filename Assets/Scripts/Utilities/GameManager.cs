using System.Collections;
using UnityEngine;

/// <summary>
/// Has references to the player.
/// </summary>
public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }
    public static bool isSlowMotion;
    // should be in SO probably
    public static float slowMotionSpeed = .5f;

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
        player.OnPlayerDeath.AddListener(GameOver);
    }

    public void OnDestroy()
    {
        player.OnPlayerDeath.RemoveListener(GameOver);
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

    private void GameOver()
    {
        SceneLoader.Instance.LoadScene(SceneLoader.SceneList.FailMenu, true);
    }
}
