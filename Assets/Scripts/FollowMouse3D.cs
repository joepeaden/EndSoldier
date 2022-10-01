using UnityEngine;

/// <summary>
/// Attatched to an object will make the object follow where the mouse points in 3D.
/// </summary>
/// <author>
/// Joe
/// </author>
public class FollowMouse3D : MonoBehaviour
{
    [SerializeField] private GameObject raycastPlane;
    public LayerMask mask;

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, mask))
        {
            Debug.Log(hit.transform.name);
            if (hit.transform.gameObject == raycastPlane)
            {
                transform.position = hit.point;
            }
        }
    }
}
