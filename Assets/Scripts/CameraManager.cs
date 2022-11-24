using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

// Author: Joseph Peaden

/// <summary>
/// Manages the camera. Camerawork should go through here.
/// </summary>
public class CameraManager : MonoBehaviour
{
    private static CameraManager _instance;
    public static CameraManager Instance { get { return _instance; } }

    [SerializeField] private CinemachineVirtualCamera vCam;
    // is this ref needed?
    [SerializeField] private Camera mainCam;

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

        //Player.OnPlayerBeginAim += FollowReticle;
        //Player.OnPlayerEndAim += FollowPlayer;
    }

    private void FollowPlayer()
    {
        FollowTarget(GameManager.Instance.GetPlayerGO().transform);
    }

    private void FollowReticle()
    {
        FollowTarget(GameManager.Instance.GetReticleGO().transform);
    }

    public void FollowTarget(Transform toFollow)
    {
        vCam.Follow = toFollow;
    }

}
