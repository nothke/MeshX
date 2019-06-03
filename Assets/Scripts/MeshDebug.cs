#if UNITY_EDITOR

// Shows vertices, normals, triangles of a mesh in editor view
// CURRENTLY ONLY WORKS IF TRANSFORM IS AT CENTER WITH NO ROTATION OR SCALING
// TODO: ^ convert from mesh space to global space

using UnityEngine;
using System.Collections;
using UnityEditor;


public class MeshDebug : MonoBehaviour
{
    public bool vertexNumbers = true;
    public bool normals = true;
    public float normalGizmoLength = 0.2f;
    public bool triangles;
    public float gizmoScale = 1;

    void OnDrawGizmos()
    {

        if (!GetComponent<MeshFilter>()) return;

        Mesh mesh = GetComponent<MeshFilter>().sharedMesh;

        if (mesh == null) return;
        if (mesh.vertexCount == 0) return;

        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            Vector3 vPos = transform.TransformPoint(mesh.vertices[i]);

            if (vertexNumbers)
                Handles.Label(vPos, i.ToString());



            Gizmos.color = Color.green;

            if (normals && i < mesh.normals.Length)
            {
                Vector3 nDir = transform.TransformDirection(mesh.normals[i]);
                Gizmos.DrawRay(vPos, nDir * normalGizmoLength);
            }
        }

        if (triangles)
        {
            for (int i = 0; i < mesh.triangles.Length; i += 3)
            {
                int v0 = mesh.triangles[i];
                int v1 = mesh.triangles[i + 1];
                int v2 = mesh.triangles[i + 2];


                Gizmos.DrawLine(mesh.vertices[v0], mesh.vertices[v1]);
                Gizmos.DrawLine(mesh.vertices[v1], mesh.vertices[v2]);
                Gizmos.DrawLine(mesh.vertices[v2], mesh.vertices[v0]);

                Gizmos.color = Color.blue * 0.5f;
                Gizmos.DrawSphere(mesh.vertices[v0], gizmoScale);

                Gizmos.color = Color.green * 0.5f;
                Gizmos.DrawSphere(mesh.vertices[v1], gizmoScale);

                Gizmos.color = Color.red * 0.5f;
                Gizmos.DrawSphere(mesh.vertices[v2], gizmoScale);
            }
        }
    }
}
#endif