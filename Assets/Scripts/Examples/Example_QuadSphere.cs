using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MeshXtensions;

public class Example_QuadSphere : MonoBehaviour
{

    void Start()
    {
        Geom g = MeshX.GeomQuadSphere(100, 10);

        NoiseMethod method = Noise.simplexMethods[2];

        float mult = 0.05f;
        float thres = -0.1f;

        for (int i = 0; i < g.vertices.Count; i++)
        {
            //NoiseSample sample = Noise.Simplex3D(g.vertices[i], 0.6f);

            NoiseSample sample = Noise.Sum(method, g.vertices[i], 0.1f, 4, 2, 0.5f);

            Vector3 normal = g.vertices[i].normalized;

            if (sample.value < thres)
            {
                g.vertices[i] *= 1 + mult * thres;
                g.normals[i] = normal;
            }
            else
            {
                g.vertices[i] *= 1 + sample.value * mult;
                g.normals[i] = (normal - sample.derivative * 1).normalized;
            }
        }

        Mesh m = g.ToMesh();
        //m.RecalculateNormals();

        gameObject.InitMesh(m);
    }
}
