using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

using MeshXtensions;

public class Example_Threading : MonoBehaviour
{

    Thread t;

    void Start()
    {
        t = new Thread(ThreadRun);
        t.Start();

        StartCoroutine(ThreadWait());
    }

    // WITHOUT THREADING: move to separate example script
    public void MeshTest()
    {
        Geom g1 = MeshX.GeomCube(Vector3.one);

        for (int i = 0; i < 10; i++)
        {
            Geom g = MeshX.GeomCube(Vector3.one);
            g.Translate(Vector3.forward * (1 + i) * 2);

            g1.CombineWith(g);
        }

        float t_convert = Time.realtimeSinceStartup;
        Mesh m = g1.ToMesh();
        Debug.Log("Mesh conversion completed in: " + (Time.realtimeSinceStartup - t_convert));

        float t_assign = Time.realtimeSinceStartup;
        gameObject.InitMesh(m);
        Debug.Log("Mesh assignment completed in: " + (Time.realtimeSinceStartup - t_assign));
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

    public string somethingggg;

    public Vector3[] vectors;

    Geom g;

    void ThreadRun()
    {
        int dim = 13;
        g = MeshX.GeomCube(Vector3.one);

        for (int x = 0; x < dim; x++)
        {
            for (int y = 0; y < dim; y++)
            {
                for (int z = 0; z < dim; z++)
                {
                    Geom ga = MeshX.GeomCube(Vector3.one);
                    ga.Translate(new Vector3(x, y, z) * 2);

                    g.CombineWith(ga);
                }
            }
        }
    }

    void ThreadEnd()
    {
        float t_convert = Time.realtimeSinceStartup;
        Mesh m = g.ToMesh();
        Debug.Log("Mesh conversion completed in: " + (Time.realtimeSinceStartup - t_convert));

        float t_assign = Time.realtimeSinceStartup;
        gameObject.InitMesh(m);
        Debug.Log("Mesh assignment completed in: " + (Time.realtimeSinceStartup - t_assign));
    }
}
