using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MeshXtensions;

public class Example_Sphere : MonoBehaviour
{

    private void Start()
    {
        Geom s = new CutoffSphere(20, 10, 1, 0.5f, 0.9f);

        gameObject.InitMesh(s.ToMesh());
    }

    public float handleBottom;
    public float handleTop;

    private void Update()
    {
        Geom s = new CutoffSphere(20, 10, 2, handleBottom, handleTop);
        gameObject.SetMesh(s.ToMesh());
    }
}
