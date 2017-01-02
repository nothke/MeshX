using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MeshXtensions;

public class Example_Simplest : MonoBehaviour
{
    public Material material;

    void Start()
    {
        gameObject.InitializeMesh(MeshX.Cube(), material);
    }
}
