using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MeshXtensions;

public class TerrainTile : MonoBehaviour
{

    public Vector2 offset;

    void Start()
    {
        Generate();
    }

    private void OnValidate()
    {
        if (!Application.isPlaying) return;

        Generate();
    }

    void Generate()
    {
        Grid grid = Grid.Create(64, 64, 1);

        Vector3[] points = grid.vertices;

        Vector3 offset = GetV3Offset();

        for (int i = 0; i < points.Length; i++)
        {
            points[i] = TerrainManager.e.GetWorldSurfacePoint(offset + points[i]) - offset;
        }

        gameObject.InitMesh(MeshXN.Combine(grid));
    }

    Vector3 GetV3Offset()
    {
        return new Vector3(offset.x, 0, offset.y);
    }
}
