using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MeshXtensions;

public class Example_QuadSphere : MonoBehaviour
{

    void Start()
    {
        Geom g = MeshX.GeomQuadSphere(100, 10);

        for (int i = 0; i < g.vertices.Count; i++)
        {
            g.vertices[i] *= 1 + Random.value * 0.02f;
        }

        Mesh m = g.ToMesh();
        m.RecalculateNormals();

        gameObject.InitMesh(m);
    }
}
