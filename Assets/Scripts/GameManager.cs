using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Author: Joseph Peaden

/// <summary>
/// Has references to the player.
/// </summary>
public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    [SerializeField] private GameObject playerGO;
    private Player player;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Debug.Log("More than one Camera Manager, deleting one.");
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

        player = playerGO.GetComponent<Player>();
    }

    public Player GetPlayerScript()
    {
        return player;
    }

    public GameObject GetPlayerGO()
    {
        return playerGO;
    }
}
