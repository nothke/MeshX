using System.Collections;
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

    public NoiseMethod noiseMethod;

    void Start()
    {
        noiseMethod = Noise.perlinMethods[2];
    }

    public Vector3 GetWorldSurfacePoint(Vector3 point)
    {

        /*
        float perlin = Mathf.PerlinNoise(frequency * point.x, frequency * point.z) * gain;
        float perlin2 = Mathf.PerlinNoise(frequency2 * point.x, frequency2 * point.z) * gain2;
        float perlin3 = Mathf.PerlinNoise(frequency * 2 * point.x, frequency * 2 * point.z) * gain / 2;

        float perlin05 = Mathf.PerlinNoise(1000 + frequency * 0.5f * point.x, frequency * 0.5f * point.z) * gain * 2;

        float h = perlin + perlin2 + perlin3 + perlin05;*/

        NoiseSample sample = Noise.Sum(noiseMethod, point, 0.002f, 6, 2, 0.5f) * 500;

        float h = sample.value;

        if (h < 0) h = 0;

        return new Vector3(point.x, h, point.z);
    }

    public Vector3 GetWorldSurfaceNormal(Vector3 point)
    {
        NoiseSample sample = Noise.Sum(noiseMethod, point, 0.002f, 6, 2, 0.5f) * 500;
        return new Vector3(-sample.derivative.x, 1f, -sample.derivative.y).normalized;
    }
}
