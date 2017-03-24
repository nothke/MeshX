using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace MeshXtensions
{
    public class Grid : Surf
    {
        public int width = 2;

        public int length { get { return (vertices.Length / width); } }

        public int size { get { return vertices.Length; } }

        public Grid(int width, params Vector3[] points) : base(points)
        {
            this.width = width;
        }

        public Grid(int width, params float[] pointCoords) : base(pointCoords)
        {
            this.width = width;
        }

        public static Grid Create(int width, int length, float separation)
        {
            Vector3[] points = new Vector3[width * length];

            for (int y = 0; y < length; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    points[y * width + x] = new Vector3(x * separation, 0, y * separation);
                }
            }

            return new Grid(width, points);
        }

        public static Grid operator +(Grid grid, float[] heightfield2)
        {
            for (int y = 0; y < grid.length; y++)
            {
                for (int x = 0; x < grid.width; x++)
                {
                    grid.vertices[y * grid.width + x] += Vector3.up * heightfield2[y * grid.width + x];
                }
            }

            return grid;
        }

        public static Grid operator +(Grid grid, Vector3[] vectorField)
        {
            for (int y = 0; y < grid.length; y++)
            {
                for (int x = 0; x < grid.width; x++)
                {
                    grid.vertices[y * grid.width + x] += vectorField[y * grid.width + x];
                }
            }

            return grid;
        }

        public static Grid operator -(Grid grid, float[] heightfield2)
        {
            for (int y = 0; y < grid.length; y++)
            {
                for (int x = 0; x < grid.width; x++)
                {
                    grid.vertices[y * grid.width + x] -= Vector3.up * heightfield2[y * grid.width + x];
                }
            }

            return grid;
        }

        public static Grid operator -(Grid grid, Vector3[] vectorField)
        {
            for (int y = 0; y < grid.length; y++)
            {
                for (int x = 0; x < grid.width; x++)
                {
                    grid.vertices[y * grid.width + x] -= vectorField[y * grid.width + x];
                }
            }

            return grid;
        }

        public static Grid operator +(Grid grid, float height)
        {
            for (int y = 0; y < grid.length; y++)
            {
                for (int x = 0; x < grid.width; x++)
                {
                    grid.vertices[y * grid.width + x] += Vector3.up * height;
                }
            }

            return grid;
        }

        public static Grid operator -(Grid grid, float height)
        {
            for (int y = 0; y < grid.length; y++)
            {
                for (int x = 0; x < grid.width; x++)
                {
                    grid.vertices[y * grid.width + x] -= Vector3.up * height;
                }
            }

            return grid;
        }

        public void AddHeights(float[] heights)
        {
            for (int y = 0; y < length; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    vertices[y * width + x] += Vector3.up * heights[y * width + x];
                }
            }
        }

        public void AddVectors(Vector3[] v3s)
        {
            for (int y = 0; y < length; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    vertices[y * width + x] += v3s[y * width + x];
                }
            }
        }

        public void AddVector(Vector3 v3)
        {
            for (int y = 0; y < length; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    vertices[y * width + x] += v3;
                }
            }
        }

        public override List<int> GetTris(int startIndex = 0)
        {
            if (width < 2) width = 2;

            int vertLength = vertices.Length;

            if (vertLength < 3) return null;

            List<int> indices = new List<int>();

            // 0, width, 1
            // 1, width, width + 1

            // 2, width + 1, 3
            // 3, width + 1, width + 2

            // i, i + width + 1, i + 1
            // i + 1, i * width + 1, i + width + 2;

            for (int i = 0; i < vertLength - 1; i++)
            {
                //int y = i + width;

                if (i % width == width - 1) continue;

                if (startIndex + i + width + 1 >= vertLength)
                    return indices;

                // tri 1
                indices.Add(startIndex + i);
                indices.Add(startIndex + i + width);
                indices.Add(startIndex + i + 1);

                // tri 2
                indices.Add(startIndex + i + 1);
                indices.Add(startIndex + i + width);
                indices.Add(startIndex + i + width + 1);

            }

            return indices;
        }
    }
}
