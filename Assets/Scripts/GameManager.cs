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

        Player.OnPlayerBeginAim += delegate { SetReticleActive(true); };
        Player.OnPlayerEndAim += delegate { SetReticleActive(false); };
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

    public void SetReticleActive(bool isActive)
    {
        reticleGO.GetComponent<MeshRenderer>().enabled = isActive;
    }
}
