using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

using MeshXtensions;

public class Example_QuadSphereSide : MonoBehaviour
{
    public Material material;
    public Gradient gradient;

    public MeshX_QuadSphere.Side side;

    Thread t;

    void Start()
    {
        t = new Thread(CreateGeom);
        t.Start();

        StartCoroutine(ThreadWait());
    }

    IEnumerator ThreadWait()
    {
        int i = 0;

        while (t.IsAlive)
        {
            yield return null;
            i++;
        }

        Debug.Log("Thread ended after " + i + " frames");
        ThreadEnd();
    }

    void ThreadEnd()
    {
        Mesh m = g.ToMesh();

        gameObject.InitMesh(m, material);
    }

    Geom g;

    void CreateGeom2()
    {
        g = MeshX_QuadSphere.GeomQuadSphereSide(side, 254, 10);
    }

    void CreateGeom()
    {
        g = MeshX_QuadSphere.GeomQuadSphereSide(side, 254, 200);

        NoiseMethod method = Noise.simplexMethods[2];

        float mult = 0.05f;
        float thres = -0.1f;

        g.colors = new List<Color>();

        for (int i = 0; i < g.vertices.Count; i++)
        {
            NoiseSample sample = Noise.Sum(method, g.vertices[i], 0.01f, 5, 2, 0.5f);

            Vector3 normal = g.vertices[i].normalized;

            if (sample.value < thres)
            {
                g.vertices[i] *= 1 + mult * thres;
                g.normals[i] = normal;
            }
            else
            {
                g.vertices[i] *= 1 + sample.value * mult;
                g.normals[i] = (normal - sample.derivative * 0.3f).normalized;
            }

            g.colors.Add(gradient.Evaluate(0.5f + sample.value));
        }
    }
}
