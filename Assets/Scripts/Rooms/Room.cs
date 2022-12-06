using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Handles toggling cut away walls when player enters or exits a room.
/// </summary>
public class Room : MonoBehaviour
{
    /// <summary>
    /// Unity doesn't show Interfaces in inspector I guess. So this is just for assignment to setActives list.
    /// </summary>
    [SerializeField] private List<GameObject> setActiveGameObjects = new List<GameObject>();
    [SerializeField] private GameObject walls;
    [SerializeField] private GameObject wallsCut;

    /// <summary>
    /// Collection of stuff inside the room that needs to be activated or deactivated based on if the player is present. Things like Actors, traps, etc.
    /// </summary>
    private List<ISetActive> setActives = new List<ISetActive>();

    private void Start()
    {
        // fill list of stuff in the room, set them all to inactive initially.
        foreach (GameObject go in setActiveGameObjects)
        {
            if (go.activeInHierarchy)
            {
                ISetActive setActive = go.GetComponent<ISetActive>();
                setActives.Add(setActive);
                setActive.DeActivate();
            }
        }

        RoomTrigger trigger = GetComponentInChildren<RoomTrigger>();
        trigger.OnRoomExit.AddListener(UpdateRoomState);
        trigger.OnRoomEnter.AddListener(UpdateRoomState);
    }

    /// <summary>
    /// Set cut away walls active and hide furniture, activate actors.
    /// </summary>
    private void UpdateRoomState()
    {
        // if the walls are currently active, the player is now entering the room
        bool playerIsEntering = walls.activeInHierarchy;

        foreach (ISetActive a in setActives)
        {
            if (playerIsEntering)
            {
                a.Activate();
            }
            else
            {
                a.DeActivate();
            }
        }

        walls.SetActive(!walls.activeInHierarchy);
        wallsCut.SetActive(!wallsCut.activeInHierarchy);
    }
}
