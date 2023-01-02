using System.Collections;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Straight up Sebastian Lague's Field of View script.
/// https://www.youtube.com/watch?v=rQG9aUWarwE
/// </summary>
[CustomEditor(typeof(FieldOfView))]
public class FieldOfViewEditor : Editor
{
    private void OnSceneGUI()
    {
        FieldOfView fov = (FieldOfView)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(fov.transform.position, Vector3.up, Vector3.forward, 360, fov.viewRadius);
        Vector3 viewAngleA = fov.DirFromAngle(-fov.viewAngle / 2, false);
        Vector3 viewAngleB = fov.DirFromAngle(fov.viewAngle / 2, false);

        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleA * fov.viewRadius);
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleB * fov.viewRadius);

        Handles.color = Color.red;
        // foreach (ISetActive visibleTarget in fov.visibleThings)
        // {
        //     Handles.DrawLine(fov.transform.position, visibleTarget.GetComponent<Transform>().position);
        // }
    }
}
