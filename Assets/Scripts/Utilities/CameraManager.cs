using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Cinemachine.PostFX;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

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

    public VolumeProfile postProcProfile;

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

        postProcProfile = vCam.GetComponent<CinemachineVolumeSettings>().m_Profile;

        // this sucks but I don't feel like trying harder right now. Need to figure out how to not permanently modify it.
        Vignette v;
        postProcProfile.TryGet(out v);
        v.intensity.Override(0f);
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

    public void SetVignette(float percent)
    {
        Vignette v;
        postProcProfile.TryGet(out v);
        v.intensity.Override(percent);
    }

}
