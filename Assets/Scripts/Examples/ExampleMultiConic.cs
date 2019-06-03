using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MeshXtensions;

public class ExampleMultiConic : MonoBehaviour
{
    public int segments = 10;
    public Vector2[] bands;

    void Start()
    {
        gameObject.InitMesh();
    }

    void Update()
    {
        Geom g = new Geom();
        g.vertices = new List<Vector3>();
        g.triangles = new List<Triangle>();
        g.normals = new List<Vector3>();

        float totalHeight = 0;

        float chamferHeight = 0.06f;

        Vector3[] lastNormals = new Vector3[segments + 1];

        for (int i = 1; i < bands.Length; i++)
        {
            ConicStrip cs = new ConicStrip(segments, bands[i].x, bands[i - 1].y, bands[i].y);
            cs.Translate(new Vector3(0, totalHeight + bands[i].x / 2, 0));
            totalHeight += bands[i].x;

            for (int n = 0; n < segments + 1; n++)
            {
                lastNormals[n] = cs.normals[n];
            }

            if (i != 0)
            {
                ConicStrip chamfer = new ConicStrip(segments, chamferHeight, bands[i].y, bands[i].y);
                chamfer.Translate(new Vector3(0, totalHeight + chamferHeight / 2, 0));

                for (int v = 0; v < segments + 1; v++)
                {
                    chamfer.normals[v] = lastNormals[v];
                }

                totalHeight += chamferHeight;

                g.CombineWith(chamfer);
            }



            g.CombineWith(cs);
        }

        gameObject.SetMesh(g.ToMesh());
    }
}
