using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Based on Sebastian Lague's Field of View script (with edits).
/// https://www.youtube.com/watch?v=rQG9aUWarwE
/// </summary>
public class FieldOfView : MonoBehaviour
{
    public float viewRadius;
    [Range(0,360)]
    public float viewAngle;

    public LayerMask targetMask;
    public LayerMask obstacleMask;

    public float meshResolution;
    public int edgeResolveIterations;

    public List<Transform> visibleThings = new List<Transform>();

    public MeshFilter viewMeshFilter;
    public MeshFilter viewMeshFilter2;
    public MeshFilter squareMeshFilter1;
    public MeshFilter squareMeshFilter2;
    private Mesh viewMesh;
    private Mesh squareMesh1;
    private Mesh squareMesh2;
    private Mesh viewMesh2;
    public float edgeDstThreshold;


    public float upwardsValue;
    public float mesh2Height;

    void Start()
    {
        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        viewMeshFilter.mesh = viewMesh;
        viewMesh2 = new Mesh();
        viewMesh2.name = "View Mesh2";
        viewMeshFilter2.mesh = viewMesh2;

        squareMesh1 = new Mesh();
        squareMesh1.name = "Square Mesh1";
        squareMeshFilter1.mesh = squareMesh1;

        squareMesh2 = new Mesh();
        squareMesh2.name = "Square Mesh2";
        squareMeshFilter2.mesh = squareMesh2;

        // This will be useful for AI target finding.
        StartCoroutine("FindTargetsWithDelay", .2f);
    }

    void LateUpdate() 
    {
        DrawFieldOfView(viewMesh, 0);
        // DrawFieldOfView(viewMesh2, mesh2Height);

        // // squares that cover sides of the view mesh and connect viewMesh to viewMesh2
        // Vector3[] verts = 
        // {
        //     viewMesh.vertices[0], viewMesh.vertices[1], viewMesh2.vertices[0], viewMesh2.vertices[1]
        // };

        // // make triangles for square mesh
        // int[] tris = 
        // {
        //     0, 1, 2, 2, 1, 3
        // };

        // squareMesh1.Clear();
        // squareMesh1.vertices = verts;
        // squareMesh1.triangles = tris;
        // squareMesh1.RecalculateNormals();

        // // make verts for second square mesh on other side of viewMesh
        // verts = new Vector3[]
        // {
        //     viewMesh.vertices[0], viewMesh.vertices[viewMesh.vertices.Length - 1], viewMesh2.vertices[0], viewMesh2.vertices[viewMesh2.vertices.Length - 1]
        // };
        // // flipped tris
        // tris = new int[]
        // {
        //     0, 2, 1, 2, 3, 1
        // };
        // // assign
        // squareMesh2.Clear();
        // squareMesh2.vertices = verts;
        // squareMesh2.triangles = tris;
        // squareMesh2.RecalculateNormals();
    }

    // This will be useful for AI target finding.
    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            List<Transform> oldVisibleThings = new List<Transform>(visibleThings);
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
            foreach (Transform thing in oldVisibleThings)
            {
                if (!visibleThings.Contains(thing))
                {
                    if (thing.GetComponent<Actor>() != null)
                    {
                        thing.GetComponent<Actor>().SetVisibility(false);
                    }
                    else if (thing.GetComponent<RevealOnLOS>() != null)
                    {
                        thing.GetComponent<RevealOnLOS>().Hide();
                    }
                    // if (thing.GetComponent<MeshRenderer>() != null)
                    // {
                    //     thing.GetComponent<MeshRenderer>().material.SetInt("_IsVisible", 0);
                    // } 

                    // foreach (MeshRenderer mr in thing.GetComponentsInChildren<MeshRenderer>())
                    // {
                    //     mr.material.SetInt("_IsVisible", 0);
                    // }
                }
            }
        }
    }

    void DrawFieldOfView(Mesh mesh, float ypos) 
    {
        int stepCount = Mathf.RoundToInt(viewAngle * meshResolution);
        float stepAngleSize = viewAngle / stepCount;
        List<Vector3> viewPoints = new List<Vector3>();
        ViewCastInfo oldViewCast = new ViewCastInfo();

        for (int i = 0; i <= stepCount; i++)
        {
            float angle = transform.eulerAngles.y - viewAngle / 2 + stepAngleSize * i;
            ViewCastInfo newViewCast = ViewCast(angle, ypos);

            if (i < 0)
            {
                bool edgeDstThresholdExceeded = Mathf.Abs(oldViewCast.dst - newViewCast.dst) > edgeDstThreshold;
                if (oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && edgeDstThresholdExceeded))
                {
                    EdgeInfo edge = FindEdge(oldViewCast, newViewCast);
                    if (edge.pointA != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointA);
                    }
                    if (edge.pointB != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointB);
                    }
                }
            } 

            viewPoints.Add(newViewCast.point);
            oldViewCast = newViewCast;
        }

        int vertexCount = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];

        vertices[0] = new Vector3(0, ypos, 0);
        for (int i = 0; i < vertexCount - 1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);

            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }

    EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
    {
        float minAngle = minViewCast.angle;
        float maxAngle = maxViewCast.angle;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        for (int i = 0; i < edgeResolveIterations; i++)
        {
            float angle = (minAngle + maxAngle) / 2;
            ViewCastInfo newViewCast = ViewCast(angle);

            bool edgeDstThresholdExceeded = Mathf.Abs(minViewCast.dst - newViewCast.dst) > edgeDstThreshold;
            if (newViewCast.hit == minViewCast.hit && !edgeDstThresholdExceeded)
            {
                minAngle = angle;
                minPoint = newViewCast.point;
            }
            else
            {
                maxAngle = angle;
                maxPoint = newViewCast.point;
            }
        }

        return new EdgeInfo(minPoint, maxPoint);
    }

    ViewCastInfo ViewCast(float globalAngle, float ypos = 0f)
    {
        Vector3 castOrigin = transform.position;
        if (ypos != 0f)
        {
            castOrigin.y = ypos;
        }

        Vector3 dir = DirFromAngle(globalAngle, true);
        if (ypos != 0)
        {
            dir.y += upwardsValue;
        }

        RaycastHit hit;

        if (Physics.Raycast(castOrigin, dir, out hit, viewRadius, obstacleMask))
        {
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        }
        else
        {
            return new ViewCastInfo(false, castOrigin + dir * viewRadius, viewRadius, globalAngle);
        }
    }

    void FindVisibleTargets()
    {
        visibleThings.Clear();
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;

            // skip self
            if (target == transform)
                continue;

            Vector3 dirToTarget = (target.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                float dstToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
                {
                    if (target.GetComponent<Actor>() != null)
                    {
                        target.GetComponent<Actor>().SetVisibility(true);
                    }

                    if (target.GetComponent<RevealOnLOS>() != null)
                    {
                        target.GetComponent<RevealOnLOS>().Reveal();
                    }

                    // if (target.GetComponent<MeshRenderer>() != null)
                    // {
                    //     target.GetComponent<MeshRenderer>().materials[0].SetInt("_IsVisible", 1);
                    // }

                    // foreach (MeshRenderer mr in target.GetComponentsInChildren<MeshRenderer>())
                    // {
                    //     mr.material.SetInt("_IsVisible", 1);
                    // }
                    
                    visibleThings.Add(target);
                }
            }
        }
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    public struct ViewCastInfo 
    {
        public bool hit;
        public Vector3 point;
        public float dst;
        public float angle;

        public ViewCastInfo(bool _hit, Vector3 _point, float _dst, float _angle)
        {
            hit = _hit;
            point = _point;
            dst = _dst;
            angle = _angle;
        }
    }

    public struct EdgeInfo
    {
        public Vector3 pointA;
        public Vector3 pointB;

        public EdgeInfo(Vector3 _pointA, Vector3 _pointB)
        {
            pointA = _pointA;
            pointB = _pointB;
        }
    }
}
