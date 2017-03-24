using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MeshXtensions;

public class Example_Grid : MonoBehaviour
{
    public int width = 100;
    public float separation = 1;


    void Start()
    {

        Grid g = Grid.Create(width, width, separation);

        float[] f = g.MOD_RidgedPerlinGrid(0.01f, 20);
        float[] f2 = g.MOD_RidgedPerlinGrid(0.08f, 3);
        float[] f3 = g.MOD_CanyonPerlinGrid(0.1f, 3);

        g = g + f + f2 + f3 + 50;

        Mesh m = MeshXN.Combine();

        gameObject.InitMesh(MeshXN.Combine(g));
    }
}
