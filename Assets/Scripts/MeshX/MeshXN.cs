using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MeshXtensions
{



    public class Compo
    {
        public Vector3[] vertices;
        public int[] trisIndices;

        public virtual List<int> GetTris(int startIndex = 0)
        {
            return null;
        }

        public Mesh ToMesh()
        {
            return MeshXN.Combine(this);
        }
    }

    public class Surf : Compo
    {
        public virtual void Add(Vector3 point)
        {

        }

        public Surf(params Vector3[] points)
        {
            this.vertices = points;
        }

        public Surf(params float[] pointCoords)
        {
            if (pointCoords == null) Debug.Log("No point coordinates passed");
            if (pointCoords.Length == 0) Debug.Log("No point coordinates passed");

            vertices = new Vector3[pointCoords.Length / 3];

            for (int i = 0; i < pointCoords.Length; i += 3)
            {
                vertices[i / 3] = new Vector3(pointCoords[i], pointCoords[i + 1], pointCoords[i + 2]);
            }
        }
    }

    public class Fan : Surf
    {
        public Fan(params Vector3[] points) : base(points) { }
        public Fan(params float[] pointCoords) : base(pointCoords) { }

        public override List<int> GetTris(int startIndex = 0)
        {
            int vertLength = vertices.Length;

            if (vertLength < 3) return null;

            List<int> indices = new List<int>();

            for (int i = 0; i < vertLength - 2; i++)
            {
                indices.Add(startIndex + 0);
                indices.Add(startIndex + i + 1);
                indices.Add(startIndex + i + 2);
            }

            return indices;

            // 3 verts == 1 tris
            // 4 verts == 2 tris
            // 5 verts == 3 tris

            // 5 verts: 0, 1, 2, 0, 2, 3, 0, 3, 4
        }
    }

    public class Strip : Surf
    {
        public Strip(params Vector3[] points) : base(points) { }
        public Strip(params float[] pointCoords) : base(pointCoords) { }


        public override List<int> GetTris(int startIndex = 0)
        {
            int vertLength = vertices.Length;

            if (vertLength < 3) return null;

            List<int> indices = new List<int>();

            for (int i = 0; i < vertLength - 2; i++)
            {
                indices.Add(startIndex + i);

                if (i % 2 == 0)
                {
                    indices.Add(startIndex + i + 1);
                    indices.Add(startIndex + i + 2);
                }
                else
                {
                    indices.Add(startIndex + i + 2);
                    indices.Add(startIndex + i + 1);
                }
            }

            // 3 verts = 1 tris
            // 4 verts = 2 tris
            // 5 verts = 3 tris
            // 6 verts = 4 tris..

            // 6 verts == 0, 1, 2| 1, 3, 2| 2, 3, 4| 3, 5, 4 

            return indices;
        }
    }

    public class MeshXN
    {
        public static Mesh Combine(params Compo[] compos)
        {
            if (compos.Length == 0) return null;

            Mesh m = new Mesh();

            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();

            int triStart = 0;

            for (int i = 0; i < compos.Length; i++)
            {
                if (compos[i].vertices == null) Debug.Log("vertices are null");
                if (compos[i].vertices.Length == 0) Debug.Log("vertices are empty");

                vertices.AddRange(compos[i].vertices);
                triangles.AddRange(compos[i].GetTris(triStart));

                triStart += compos[i].vertices.Length;
            }

            m.SetVertices(vertices);
            m.SetTriangles(triangles, 0);
            m.RecalculateNormals();
            m.RecalculateBounds();

            return m;
        }
    }

    public class TestClass
    {
        Mesh TestMesh()
        {
            Strip s = new Strip(
                -1, 0, -1,
                -1, 0, 1,
                1, 0, -1,
                1, 0, 1,
                2, 0, -1,
                -2, 0, 1);

            Fan f = new Fan(
                -1, 0, -1,
                -1, 0, 1,
                1, 0, -1,
                1, 0, 1,
                2, 0, -1,
                -2, 0, 1);

            Vector3 v1a = new Vector3(-1, 0, -1);
            Vector3 v1b = new Vector3(-1, 0, 1);
            Vector3 v2a = new Vector3(1, 0, -1);
            Vector3 v2b = new Vector3(1, 0, 1);
            Vector3 v3a = new Vector3(2, 0, -1);
            Vector3 v3b = new Vector3(2, 0, -1);

            Fan f2 = new Fan(v1a, v1b, v2a, v2b, v3a, v3b);

            return MeshXN.Combine(s, f, f2);
        }
    }

}
