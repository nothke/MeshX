/// A threadable mesh

using UnityEngine;

namespace MeshXtensions
{
    public class MeshT
    {
        // Vertex data:
        public Vector3[] vertices;
        public Vector3[] normals;
        public Vector4[] tangents;
        public Vector2[] uvs;
        public Color[] colors;

        // TODO: add other uvs

        // Triangles:
        public Triangle[] triangles;

        public void Combine(params MeshT[] meshTs)
        {
            int vertexCount = vertices == null ? 0 : vertices.Length;
            int triangleCount = triangles == null ? 0 : triangles.Length;

            for (int m = 0; m < meshTs.Length; m++)
            {
                vertexCount += meshTs[m].vertices == null ? 0 : meshTs[m].vertices.Length;
                triangleCount += meshTs[m].triangles == null ? 0 : meshTs[m].triangles.Length;
            }

            Vector3[] _vertices = new Vector3[vertexCount];
            Vector3[] _normals = new Vector3[vertexCount];
            Vector4[] _tangents = new Vector4[vertexCount];
            Vector2[] _uvs = new Vector2[vertexCount];

            int v = 0;

            for (int i = 0; i < vertices.Length; i++)
            {
                _vertices[v] = vertices[i];

                if (normals != null) _normals[v] = normals[i];
                if (tangents != null) _tangents[v] = tangents[i];
                if (uvs != null) _uvs[v] = uvs[i];

                v++;
            }

            for (int m = 0; m < meshTs.Length; m++)
            {
                for (int i = 0; i < meshTs[m].vertices.Length; i++)
                {
                    _vertices[v] = meshTs[m].vertices[i];
                    if (meshTs[m].normals != null) _normals[v] = meshTs[m].normals[i];
                    if (meshTs[m].tangents != null) _tangents[v] = meshTs[m].tangents[i];
                    if (meshTs[m].uvs != null) _uvs[v] = meshTs[m].uvs[i];

                    v++;
                }
            }

            Triangle[] _triangles = new Triangle[triangleCount];

            int t = 0;

            for (int i = 0; i < triangles.Length; i++)
            {
                _triangles[t] = triangles[i];

                t++;
            }

            int vertNum = vertices.Length;

            for (int m = 0; m < meshTs.Length; m++)
            {
                for (int i = 0; i < meshTs[m].triangles.Length; i++)
                {
                    _triangles[t] = meshTs[m].triangles[i].Offset(vertNum);

                    t++;
                }

                vertNum += meshTs[m].vertices.Length;
            }

            vertices = _vertices;
            normals = _normals;
            tangents = _tangents;
            uvs = _uvs;

            triangles = _triangles;
        }

        // NOT THREADABLE!
        public Mesh ToMesh()
        {
            Mesh m = new Mesh();
            m.vertices = vertices;
            m.normals = normals;
            m.tangents = tangents;
            m.uv = uvs;
            m.triangles = Triangle.ToIntArray(triangles);

            return m;
        }

        public void Translate(Vector3 by)
        {
            for (int i = 0; i < vertices.Length; i++)
                vertices[i] += by;
        }

        public void Scale(float by)
        {
            for (int i = 0; i < vertices.Length; i++)
                vertices[i] *= by;
        }
    }

    public static class MeshTPrimitive
    {
        public static MeshT Quad(Vector2 size)
        {
            MeshT meshT = new MeshT();
            meshT.vertices = new Vector3[4];
            meshT.vertices[0] = new Vector3(-size.x * 0.5f, -size.y * 0.5f);
            meshT.vertices[1] = new Vector3(-size.x * 0.5f, size.y * 0.5f);
            meshT.vertices[2] = new Vector3(size.x * 0.5f, -size.y * 0.5f);
            meshT.vertices[3] = new Vector3(size.x * 0.5f, size.y * 0.5f);

            meshT.normals = new Vector3[4];
            for (int i = 0; i < 4; i++)
                meshT.normals[i] = Vector3.back;

            meshT.triangles = new Triangle[2];
            meshT.triangles[0] = new Triangle(0, 1, 2);
            meshT.triangles[1] = new Triangle(1, 3, 2);

            return meshT;
        }
    }
}