using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MeshXtensions;

public class Example_OilTank : MonoBehaviour
{

    private void Start()
    {
        //gameObject.InitMesh(new ConicStrip(10, 2, 1, 1).ToMesh());
        gameObject.InitMesh(new OilTank(1, 5, 50, 0.3f).ToMesh());
    }
}
