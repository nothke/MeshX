using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeshXtensions
{

    public static class MeshXN_Grid
    {
        // HEIGHT MODS:

        public static float[] MOD_SmoothSide(this Grid grid, float[] heights, int side = 0, int smoothWidth = 10, float targetHeight = 0)
        {
            if (smoothWidth == 0) return heights;


            for (int y = 0; y < grid.length; y++)
            {
                for (int x = 0; x < grid.width; x++)
                {
                    float curHeight = heights[y * grid.width + x];

                    float mult = 1;

                    if (side == 0)
                        mult = Mathf.Clamp01((float)x / smoothWidth);
                    else
                        mult = Mathf.Clamp01((float)(grid.width - 1 - x) / smoothWidth);


                    heights[y * grid.width + x] = Mathf.Lerp(targetHeight, curHeight, mult);
                }
            }

            return heights;
        }

        public static float[] MOD_SpatialRidgedGrid(this Grid grid,
            float frequency, float scale,
            float xSeparation = 1, float ySeparation = 1,
            float xOffset = 0, float yOffset = 0, float absPlane = 0.5f)
        {
            float[] heights = new float[grid.width * grid.length];

            if (scale == 0 || frequency == 0) return heights;

            for (int y = 0; y < grid.length; y++)
                for (int x = 0; x < grid.width; x++)
                {
                    float value =

                        Mathf.PerlinNoise(
                            xOffset / xSeparation + x * frequency * xSeparation,
                            yOffset / ySeparation + y * frequency * ySeparation)
                            ;

                    if (value > absPlane)
                        value -= (value - absPlane);

                    heights[y * grid.width + x] = value * scale;
                }

            return heights;
        }

        public static float[] MOD_SpatialPerlinGrid(this Grid grid,
            float frequency, float scale,
            float xSeparation = 1, float ySeparation = 1,
            float xOffset = 0, float yOffset = 0)
        {
            float[] heights = new float[grid.width * grid.length];

            if (scale == 0 || frequency == 0) return heights;

            for (int y = 0; y < grid.length; y++)
                for (int x = 0; x < grid.width; x++)
                    heights[y * grid.width + x] =

                        Mathf.PerlinNoise(
                            xOffset / xSeparation + x * frequency * xSeparation,
                            yOffset / ySeparation + y * frequency * ySeparation)
                            * scale;

            return heights;
        }

        public static float[] MOD_PerlinGrid(this Grid grid, float frequency, float scale, float xOffset = 0, float yOffset = 0)
        {
            float[] heights = new float[grid.width * grid.length];

            for (int y = 0; y < grid.length; y++)
            {
                for (int x = 0; x < grid.width; x++)
                {
                    heights[y * grid.width + x] = Mathf.PerlinNoise(xOffset + x * frequency, yOffset + y * frequency) * scale;
                }
            }

            return heights;
        }

        public static float[] MOD_CanyonPerlinGrid(this Grid grid, float frequency, float scale)
        {
            float[] heights = new float[grid.width * grid.length];

            for (int y = 0; y < grid.length; y++)
            {
                for (int x = 0; x < grid.width; x++)
                {
                    float value = Mathf.PerlinNoise(x * frequency, y * frequency);

                    if (value > 0.5f) value *= -1;

                    heights[y * grid.width + x] = value * scale;
                }
            }

            return heights;
        }

        public static float[] MOD_RidgedPerlinGrid(this Grid grid, float frequency, float scale)
        {
            float[] heights = new float[grid.width * grid.length];

            Debug.Log(grid.length + " " + grid.width);
            Debug.Log(heights.Length);
            Debug.Log(grid.vertices.Length);

            for (int y = 0; y < grid.length; y++)
            {
                for (int x = 0; x < grid.width; x++)
                {
                    float value = Mathf.PerlinNoise(x * frequency, y * frequency);

                    if (value > 0.5f) value = -value + 1;

                    heights[y * grid.width + x] = value * scale * 2 - 50;
                }
            }

            return heights;
        }
    }
}
