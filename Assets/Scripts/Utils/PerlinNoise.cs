using UnityEngine;
using System.Collections;

[System.Serializable]
public class Perlin1D
{
    public float seed = 2324.234f;
    public float frequency = 1;
    public float scale = 1;

    public float Get(float at)
    {
        return (-0.5f + Mathf.PerlinNoise(seed, at * frequency)) * scale;
    }

    public float Get01(float at)
    {
        return Mathf.PerlinNoise(seed, at * frequency);
    }

    public float GetHeight(Vector3 v3)
    {
        return (-0.5f + Mathf.PerlinNoise(seed + v3.x, seed + v3.z));
    }

    public float GetHeight01(Vector3 v3)
    {
        return GetHeight(v3) + 0.5f;
    }
}

[System.Serializable]
public class Perlin2D
{
    public float horizontalSeed = 5349.324f;
    public float verticalSeed = 3412.21f;
    public float horizontalFrequency = 1;
    public float horizontalGain = 1;
    public float verticalFrequency = 1;
    public float verticalGain = 1;

    public Vector3 GetPosition(float at)
    {
        float x = (-0.5f + Mathf.PerlinNoise(horizontalSeed, at * horizontalFrequency)) * horizontalGain;
        float y = (-0.5f + Mathf.PerlinNoise(verticalSeed, at * verticalFrequency)) * verticalGain;

        return new Vector3(x, y, at);
    }

    public Vector3 GetDirection(float at, float samplingDistance = 2)
    {
        Vector3 fP = GetPosition(at + samplingDistance);
        Vector3 rP = GetPosition(at - samplingDistance);

        return (fP - rP).normalized;
    }
}