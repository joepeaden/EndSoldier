using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Handles toggling cut away walls when player enters or exits a room.
/// </summary>
public class Room : MonoBehaviour
{
    [SerializeField] private GameObject walls;
    [SerializeField] private GameObject wallsCut;
    [SerializeField] private GameObject roomObjectsParent;

    private RoomContent[] roomObjects;

    private void Start()
    {
        roomObjects = roomObjectsParent.GetComponentsInChildren<RoomContent>();

        RoomTrigger trigger = GetComponentInChildren<RoomTrigger>();
        trigger.OnRoomExit.AddListener(UpdateRoomVisuals);
        trigger.OnRoomEnter.AddListener(UpdateRoomVisuals);
    }

    /// <summary>
    /// Set cut away walls active and hide furniture.
    /// </summary>
    private void UpdateRoomVisuals()
    {
        foreach (RoomContent content in roomObjects)
        {
            content.SetTransparency(!walls.activeInHierarchy);
        }
        

        walls.SetActive(!walls.activeInHierarchy);
        wallsCut.SetActive(!wallsCut.activeInHierarchy);

    }
}
