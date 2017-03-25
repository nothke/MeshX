﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MeshXtensions;

public class TerrainTile : MonoBehaviour
{

    public Vector2 offset;

    public int tileVertices = 32;
    public float width = 100;

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
        float separation = width / (tileVertices - 1);

        Grid grid = Grid.Create(tileVertices, tileVertices, separation);

        Vector3[] points = grid.vertices;

        Vector3 offset = GetV3Offset();

        for (int i = 0; i < points.Length; i++)
        {
            points[i] = TerrainManager.e.GetWorldSurfacePoint(offset + points[i]) - offset;
        }

        gameObject.InitMesh(MeshXN.Combine(grid));

        if (GetComponent<MeshCollider>())
            GetComponent<MeshCollider>().sharedMesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
    }

    Vector3 GetV3Offset()
    {
        return new Vector3(offset.x, 0, offset.y);
    }
}
