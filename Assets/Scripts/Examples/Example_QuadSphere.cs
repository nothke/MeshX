using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MeshXtensions;

public class Example_QuadSphere : MonoBehaviour
{
    public Material material;
    public Gradient gradient;

    void Start()
    {
        Geom g = MeshX_QuadSphere.GeomQuadSphere(100, 10);

        NoiseMethod method = Noise.simplexMethods[2];

        float mult = 0.05f;
        float thres = -0.1f;

        g.colors = new List<Color>();

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

            g.colors.Add(gradient.Evaluate(0.5f + sample.value));
        }

        Mesh m = g.ToMesh();

        gameObject.InitMesh(m, material);
    }
}
