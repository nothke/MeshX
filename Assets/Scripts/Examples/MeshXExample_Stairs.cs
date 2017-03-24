using UnityEngine;
using System.Collections;
using MeshXtensions;
using System.Collections.Generic;

public class MeshXExample_Stairs : MonoBehaviour
{
    public int stairsNum = 10;
    public float stepWidth = 1;
    public float stepHeight = 0.2f;
    public float stepDepth = 0.2f;

    public bool total = true;
    public float totalHeight = 3;
    public float totalDepth = 10;

    public bool stairWell;

    public int floors;
    public float floorHeight = 3;
    public float slabDepth;
    public float separation = 0.3f;

    void Start()
    {

        gameObject.InitMesh(MeshX.Stairs(stairsNum, stepWidth, stepHeight, stepDepth));

    }

    void OnValidate()
    {
        if (!Application.isPlaying) return;

        if (!stairWell)
            if (!total)
                gameObject.SetMesh(MeshX.Stairs(stairsNum, stepWidth, stepHeight, stepDepth));
            else
                gameObject.SetMesh(MeshX.StairsBounds(stepWidth, totalHeight, totalDepth));

        if (!stairWell) return;

        //floors = 1;

        if (floors < 1) return;

        Mesh combined = null;

        List<Mesh> meshes = new List<Mesh>();

        float hH = floorHeight / 2;

        for (int f = 0; f < floors; f++)
        {

            float y = f * floorHeight;


            Mesh leftFlight = MeshX.StairsBounds(stepWidth, hH, totalDepth);
            Mesh rightFlight = MeshX.StairsBounds(stepWidth, hH, totalDepth);

            leftFlight.Translate(new Vector3(0, y));

            rightFlight.Rotate(180, Vector3.up);
            rightFlight.Translate(new Vector3(stepWidth + separation, y + hH, totalDepth));

            float slabThickness = 0.3f;

            Mesh slab = MeshX.Cube(new Vector3(stepWidth * 2 + separation, slabThickness, slabDepth));
            slab.Translate(new Vector3((stepWidth + separation) / 2, y + hH - slabThickness / 2, totalDepth + (slabDepth / 2)));


            meshes.Add(leftFlight);
            meshes.Add(rightFlight);
            meshes.Add(slab);

        }

        combined = MeshX.Combine(meshes.ToArray());
        gameObject.SetMesh(combined);
    }


}
