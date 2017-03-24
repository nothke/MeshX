﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainManager : MonoBehaviour
{

    public static TerrainManager e;
    void Awake() { e = this; }

    public float frequency = 0.1f;
    public float gain = 1;
    public float frequency2 = 0.5f;
    public float gain2 = 1;

    public Vector3 GetWorldSurfacePoint(Vector3 point)
    {
        float perlin = Mathf.PerlinNoise(frequency * point.x, frequency * point.z) * gain;
        float perlin2 = Mathf.PerlinNoise(frequency2 * point.x, frequency2 * point.z) * gain2;
        float perlin3 = Mathf.PerlinNoise(frequency * 2 * point.x, frequency * 2 * point.z) * gain / 2;

        float h = perlin + perlin2;

        return new Vector3(point.x, h, point.z);
    }
}