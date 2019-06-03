using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MeshXtensions;

public class MeshTVSGeom : MonoBehaviour
{

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(3);

        MeshWay();



        yield return new WaitForSeconds(1);

        MeshTWay();

        yield return new WaitForSeconds(1);

        CompoWay();

        yield return new WaitForSeconds(1);

        GridWay();
    }

    void MeshWay()
    {
        float t2 = Time.realtimeSinceStartup;

        Mesh[] meshes = new Mesh[1000];

        for (int i = 0; i < 1000; i++)
        {
            meshes[i] = MeshX.Quad(Vector3.zero, 1, 1, Vector3.forward);
            meshes[i].Translate(Vector3.forward * i);
        }

        gameObject.InitChildMesh(MeshX.Combine(meshes));

        Debug.Log("Using Mesh: " + (Time.realtimeSinceStartup - t2));
    }

    void MeshTWay()
    {
        float t = Time.realtimeSinceStartup;

        var quad = MeshTPrimitive.Quad(Vector2.one);
        var meshT = quad;

        for (int i = 0; i < 1000; i++)
        {
            var q = MeshTPrimitive.Quad(Vector2.one);
            q.Translate(Vector3.forward * i);
            meshT.Combine(q);
        }

        gameObject.InitChildMesh(meshT.ToMesh());

        Debug.Log("Using MeshT: " + (Time.realtimeSinceStartup - t));
    }

    void CompoWay()
    {
        float t = Time.realtimeSinceStartup;

        Compo[] c = new Compo[1000];

        for (int i = 0; i < 1000; i++)
        {
            c[i] = new Strip(
                new Vector3(-0.5f, -0.5f, i),
                new Vector3(-0.5f, 0.5f, i),
                new Vector3(0.5f, -0.5f, i),
                new Vector3(0.5f, 0.5f, i));
        }

        gameObject.InitChildMesh(MeshXN.Combine(c));

        Debug.Log("CompoWay completed in: " + (Time.realtimeSinceStartup - t));
    }

    void GridWay()
    {
        float t = Time.realtimeSinceStartup;

        GridMesh g = GridMesh.Create(130, 130, 1);

        gameObject.InitChildMesh(g.ToMesh());

        Debug.Log("Grid completed in: " + (Time.realtimeSinceStartup - t));
    }
}
