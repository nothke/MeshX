using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MeshXtensions
{
    /// <summary>
    /// A Geom is a threadable Mesh
    /// </summary>
    public class Geom
    {
        public List<Vector3> vertices;
        public List<Vector3> normals;
        public List<Vector4> tangents;
        public List<Vector2> uvs;
        public List<Triangle> triangles;
        public List<Color> colors;
        //public List<int> tris;

        #region Transformations

        public void Translate(Vector3 by)
        {
            if (vertices == null || by == Vector3.zero) return;
            if (vertices.Count == 0) return;

            int vertexCount = vertices.Count;

            for (int vert = 0; vert < vertexCount; vert++)
                vertices[vert] += by;
        }

        public void Scale(Vector3 by)
        {
            if (vertices == null || by == Vector3.one) return;
            if (vertices.Count == 0) return;

            int vertexCount = vertices.Count;

            for (int vert = 0; vert < vertexCount; vert++)
                vertices[vert] = new Vector3(
                    vertices[vert].x * by.x,
                    vertices[vert].y * by.y,
                    vertices[vert].z * by.z);
        }

        public void Rotate(float angle, Vector3 axis)
        {
            if (vertices == null) return;
            if (vertices.Count == 0) return;

            if (angle == 0 || axis == Vector3.zero) return;

            int vertexCount = vertices.Count;

            Quaternion qAngle = Quaternion.AngleAxis(angle, axis);
            for (int vert = 0; vert < vertexCount; vert++)
            {
                vertices[vert] = qAngle * vertices[vert];
            }
        }

        public void AddAsTris(List<int> tris)
        {
            triangles = new List<Triangle>();

            int ct = tris.Count;

            for (int i = 0; i < ct; i += 3)
            {
                triangles.Add(new Triangle(tris[i], tris[i + 1], tris[i + 2]));
            }
        }

        public void CombineWith(Geom geom)
        {
            int vertCount = vertices.Count;

            if (vertices != null && geom.vertices != null)
                vertices.AddRange(geom.vertices);

            if (normals != null && geom.normals != null)
                normals.AddRange(geom.normals);

            if (tangents != null && geom.tangents != null)
                tangents.AddRange(geom.tangents);

            if (uvs != null && geom.uvs != null)
                uvs.AddRange(geom.uvs);

            if (triangles != null && geom.triangles != null)
            {
                int oTriCount = geom.triangles.Count;

                for (int i = 0; i < oTriCount; i++)
                    triangles.Add(geom.triangles[i].Offset(vertCount));
            }
        }

        #endregion

        /// <summary>
        /// Converts to UnityEngine.Mesh. CANNOT RUN ON A THREAD!
        /// </summary>
        public Mesh ToMesh()
        {
            Mesh m = new Mesh();

            m.SetVertices(vertices);
            m.SetNormals(normals);
            m.SetTangents(tangents);
            m.SetUVs(0, uvs);
            m.SetColors(colors);

            /*
            if (this.tris != null)
            {
                m.SetTriangles(this.tris, 0, true);
                return m;
            }*/

            List<int> tris = new List<int>();

            for (int i = 0; i < triangles.Count; i++)
            {
                tris.Add(triangles[i].v1);
                tris.Add(triangles[i].v2);
                tris.Add(triangles[i].v3);
            }

            m.SetTriangles(tris, 0, true);

            //m.RecalculateBounds();

            return m;
        }

        /// <summary>
        /// Returns a copy of this Geom
        /// </summary>
        public Geom Clone()
        {
            Geom g = new Geom();

            if (vertices != null)
                g.vertices = new List<Vector3>(vertices);

            if (normals != null)
                g.normals = new List<Vector3>(normals);

            if (tangents != null)
                g.tangents = new List<Vector4>(tangents);

            if (uvs != null)
                g.uvs = new List<Vector2>(uvs);

            if (triangles != null)
                g.triangles = new List<Triangle>(triangles);

            if (colors != null)
                g.colors = new List<Color>(colors);

            return g;
        }
    }

    public struct MeshElement
    {
        public Vector3[] vertices;
        public Vector3[] normals;
        public Vector3[] tangents;
        public Vector3[] uvs;
        public Triangle[] triangles;
    }

    public struct Triangle
    {
        public int v1;
        public int v2;
        public int v3;

        public Triangle(int v1, int v2, int v3)
        {
            this.v1 = v1;
            this.v2 = v2;
            this.v3 = v3;
        }

        public Triangle Offset(int by)
        {
            return new Triangle(v1 + by, v2 + by, v3 + by);
        }

        public static int[] ToIntArray(Triangle[] triangles)
        {
            int[] tris = new int[triangles.Length * 3];

            for (int i = 0; i < triangles.Length; i++)
            {
                tris[i * 3] = triangles[i].v1;
                tris[i * 3 + 1] = triangles[i].v2;
                tris[i * 3 + 2] = triangles[i].v3;
            }

            return tris;
        }

    }

    public static class MeshX
    {
        //----------------
        // GEOM CREATION
        //----------------

        public static Geom GeomCube(Vector3 size)
        {
            Geom geom = new Geom();

            #region Vertices
            Vector3 p0 = new Vector3(-size.x * .5f, -size.y * .5f, size.z * .5f);
            Vector3 p1 = new Vector3(size.x * .5f, -size.y * .5f, size.z * .5f);
            Vector3 p2 = new Vector3(size.x * .5f, -size.y * .5f, -size.z * .5f);
            Vector3 p3 = new Vector3(-size.x * .5f, -size.y * .5f, -size.z * .5f);

            Vector3 p4 = new Vector3(-size.x * .5f, size.y * .5f, size.z * .5f);
            Vector3 p5 = new Vector3(size.x * .5f, size.y * .5f, size.z * .5f);
            Vector3 p6 = new Vector3(size.x * .5f, size.y * .5f, -size.z * .5f);
            Vector3 p7 = new Vector3(-size.x * .5f, size.y * .5f, -size.z * .5f);

            Vector3[] vertices = new Vector3[]
            {
	            // Bottom
	            p0, p1, p2, p3,
 
	            // Left
	            p7, p4, p0, p3,
 
	            // Front
	            p4, p5, p1, p0,
 
	            // Back
	            p6, p7, p3, p2,
 
	            // Right
	            p5, p6, p2, p1,
 
	            // Top
	            p7, p6, p5, p4
            };

            geom.vertices = new List<Vector3>(vertices);

            #endregion

            #region Normals
            Vector3 up = Vector3.up;
            Vector3 down = Vector3.down;
            Vector3 front = Vector3.forward;
            Vector3 back = Vector3.back;
            Vector3 left = Vector3.left;
            Vector3 right = Vector3.right;

            Vector3[] normals = new Vector3[]
            {
	        // Bottom
	        down, down, down, down,
 
	        // Left
	        left, left, left, left,
 
	        // Front
	        front, front, front, front,
 
	        // Back
	        back, back, back, back,
 
	        // Right
	        right, right, right, right,
 
	        // Top
	        up, up, up, up
            };

            geom.normals = new List<Vector3>(normals);
            #endregion

            #region Triangles

            geom.triangles = new List<Triangle>();

            // Bottom
            geom.triangles.Add(new Triangle(3, 1, 0));
            geom.triangles.Add(new Triangle(3, 2, 1));

            // Left
            geom.triangles.Add(new Triangle(3 + 4 * 1, 1 + 4 * 1, 0 + 4 * 1));
            geom.triangles.Add(new Triangle(3 + 4 * 1, 2 + 4 * 1, 1 + 4 * 1));

            // Front
            geom.triangles.Add(new Triangle(3 + 4 * 2, 1 + 4 * 2, 0 + 4 * 2));
            geom.triangles.Add(new Triangle(3 + 4 * 2, 2 + 4 * 2, 1 + 4 * 2));

            // Back
            geom.triangles.Add(new Triangle(3 + 4 * 3, 1 + 4 * 3, 0 + 4 * 3));
            geom.triangles.Add(new Triangle(3 + 4 * 3, 2 + 4 * 3, 1 + 4 * 3));

            // Right
            geom.triangles.Add(new Triangle(3 + 4 * 4, 1 + 4 * 4, 0 + 4 * 4));
            geom.triangles.Add(new Triangle(3 + 4 * 4, 2 + 4 * 4, 1 + 4 * 4));

            // Top
            geom.triangles.Add(new Triangle(3 + 4 * 5, 1 + 4 * 5, 0 + 4 * 5));
            geom.triangles.Add(new Triangle(3 + 4 * 5, 2 + 4 * 5, 1 + 4 * 5));


            #endregion

            return geom;
        }

        public static Geom GeomGrid(int xSize, int ySize, float xSeparation = 1, float ySeparation = 1, bool center = false)
        {
            Geom g = new Geom();

            // Vertices

            g.vertices = new List<Vector3>();

            float xOffset = center ? -(xSeparation * xSize) * 0.5f : 0;
            float yOffset = center ? -(ySeparation * ySize) * 0.5f : 0;

            for (int i = 0, y = 0; y <= ySize; y++)
                for (int x = 0; x <= xSize; x++, i++)
                    g.vertices.Add(new Vector3(xOffset + x * xSeparation, 0, yOffset + y * ySeparation));

            // Normals

            g.normals = new List<Vector3>();

            int vertCount = g.vertices.Count;

            for (int i = 0; i < vertCount; i++)
                g.normals.Add(Vector3.up);

            // Triangles

            g.triangles = new List<Triangle>();

            int[] triangles = new int[xSize * ySize * 6];
            for (int ti = 0, vi = 0, y = 0; y < ySize; y++, vi++)
            {
                for (int x = 0; x < xSize; x++, ti += 6, vi++)
                {
                    triangles[ti] = vi;
                    triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                    triangles[ti + 4] = triangles[ti + 1] = vi + xSize + 1;
                    triangles[ti + 5] = vi + xSize + 2;
                }
            }

            int triangleNum = xSize * ySize * 2;

            for (int i = 0; i < triangleNum; i++)
            {
                g.triangles.Add(new Triangle(
                    triangles[i * 3],
                    triangles[i * 3 + 1],
                    triangles[i * 3 + 2]));
            }

            return g;
        }

        //----------------
        // MESH CREATION
        //----------------




        /// <summary>
        /// Creates unit cube
        /// </summary>
        public static Mesh Cube() { return Cube(Vector3.one); }

        public static Mesh Cube(Vector3 size, float uvScale = 1, int uvTiles = 1, float uvOffsetX = 0, float uvOffsetY = 0, float uvOffsetZ = 0)
        {
            Mesh mesh = new Mesh();
            mesh.Clear();

            #region Vertices
            Vector3 p0 = new Vector3(-size.x * .5f, -size.y * .5f, size.z * .5f);
            Vector3 p1 = new Vector3(size.x * .5f, -size.y * .5f, size.z * .5f);
            Vector3 p2 = new Vector3(size.x * .5f, -size.y * .5f, -size.z * .5f);
            Vector3 p3 = new Vector3(-size.x * .5f, -size.y * .5f, -size.z * .5f);

            Vector3 p4 = new Vector3(-size.x * .5f, size.y * .5f, size.z * .5f);
            Vector3 p5 = new Vector3(size.x * .5f, size.y * .5f, size.z * .5f);
            Vector3 p6 = new Vector3(size.x * .5f, size.y * .5f, -size.z * .5f);
            Vector3 p7 = new Vector3(-size.x * .5f, size.y * .5f, -size.z * .5f);

            Vector3[] vertices = new Vector3[]
            {
	            // Bottom
	            p0, p1, p2, p3,
 
	            // Left
	            p7, p4, p0, p3,
 
	            // Front
	            p4, p5, p1, p0,
 
	            // Back
	            p6, p7, p3, p2,
 
	            // Right
	            p5, p6, p2, p1,
 
	            // Top
	            p7, p6, p5, p4
            };

            #endregion

            #region Normals
            Vector3 up = Vector3.up;
            Vector3 down = Vector3.down;
            Vector3 front = Vector3.forward;
            Vector3 back = Vector3.back;
            Vector3 left = Vector3.left;
            Vector3 right = Vector3.right;

            Vector3[] normals = new Vector3[]
            {
	        // Bottom
	        down, down, down, down,
 
	        // Left
	        left, left, left, left,
 
	        // Front
	        front, front, front, front,
 
	        // Back
	        back, back, back, back,
 
	        // Right
	        right, right, right, right,
 
	        // Top
	        up, up, up, up
            };
            #endregion

            #region UVs

            Vector3 uvOffset = new Vector3(uvOffsetX, uvOffsetY, uvOffsetZ);
            Vector2[] uvs = NormalProjection(RoundToSubV3(vertices, uvTiles, uvScale), uvOffset, normals);

            #endregion

            #region Triangles
            int[] triangles = new int[]
            {
	        // Bottom
	        3, 1, 0,
            3, 2, 1,			
 
	        // Left
	        3 + 4 * 1, 1 + 4 * 1, 0 + 4 * 1,
            3 + 4 * 1, 2 + 4 * 1, 1 + 4 * 1,
 
	        // Front
	        3 + 4 * 2, 1 + 4 * 2, 0 + 4 * 2,
            3 + 4 * 2, 2 + 4 * 2, 1 + 4 * 2,
 
	        // Back
	        3 + 4 * 3, 1 + 4 * 3, 0 + 4 * 3,
            3 + 4 * 3, 2 + 4 * 3, 1 + 4 * 3,
 
	        // Right
	        3 + 4 * 4, 1 + 4 * 4, 0 + 4 * 4,
            3 + 4 * 4, 2 + 4 * 4, 1 + 4 * 4,
 
	        // Top
	        3 + 4 * 5, 1 + 4 * 5, 0 + 4 * 5,
            3 + 4 * 5, 2 + 4 * 5, 1 + 4 * 5,
            };
            #endregion

            mesh.vertices = vertices;
            mesh.normals = normals;
            mesh.uv = uvs;
            mesh.triangles = triangles;

            mesh.RecalculateBounds();
            ;

            return mesh;

        }

        public static Mesh IsoSphere(float radius, int recursionLevel = 3)
        {
            List<Vector3> vertList = new List<Vector3>();
            Dictionary<long, int> middlePointIndexCache = new Dictionary<long, int>();
            //int index = 0;

            // create 12 vertices of a icosahedron
            float t = (1f + Mathf.Sqrt(5f)) / 2f;

            vertList.Add(new Vector3(-1f, t, 0f).normalized * radius);
            vertList.Add(new Vector3(1f, t, 0f).normalized * radius);
            vertList.Add(new Vector3(-1f, -t, 0f).normalized * radius);
            vertList.Add(new Vector3(1f, -t, 0f).normalized * radius);

            vertList.Add(new Vector3(0f, -1f, t).normalized * radius);
            vertList.Add(new Vector3(0f, 1f, t).normalized * radius);
            vertList.Add(new Vector3(0f, -1f, -t).normalized * radius);
            vertList.Add(new Vector3(0f, 1f, -t).normalized * radius);

            vertList.Add(new Vector3(t, 0f, -1f).normalized * radius);
            vertList.Add(new Vector3(t, 0f, 1f).normalized * radius);
            vertList.Add(new Vector3(-t, 0f, -1f).normalized * radius);
            vertList.Add(new Vector3(-t, 0f, 1f).normalized * radius);


            // create 20 triangles of the icosahedron
            List<Triangle> faces = new List<Triangle>();

            // 5 faces around point 0
            faces.Add(new Triangle(0, 11, 5));
            faces.Add(new Triangle(0, 5, 1));
            faces.Add(new Triangle(0, 1, 7));
            faces.Add(new Triangle(0, 7, 10));
            faces.Add(new Triangle(0, 10, 11));

            // 5 adjacent faces 
            faces.Add(new Triangle(1, 5, 9));
            faces.Add(new Triangle(5, 11, 4));
            faces.Add(new Triangle(11, 10, 2));
            faces.Add(new Triangle(10, 7, 6));
            faces.Add(new Triangle(7, 1, 8));

            // 5 faces around point 3
            faces.Add(new Triangle(3, 9, 4));
            faces.Add(new Triangle(3, 4, 2));
            faces.Add(new Triangle(3, 2, 6));
            faces.Add(new Triangle(3, 6, 8));
            faces.Add(new Triangle(3, 8, 9));

            // 5 adjacent faces 
            faces.Add(new Triangle(4, 9, 5));
            faces.Add(new Triangle(2, 4, 11));
            faces.Add(new Triangle(6, 2, 10));
            faces.Add(new Triangle(8, 6, 7));
            faces.Add(new Triangle(9, 8, 1));


            // refine triangles
            for (int i = 0; i < recursionLevel; i++)
            {
                List<Triangle> faces2 = new List<Triangle>();
                foreach (var tri in faces)
                {
                    // replace triangle by 4 triangles
                    int a = GetMiddlePoint(tri.v1, tri.v2, ref vertList, ref middlePointIndexCache, radius);
                    int b = GetMiddlePoint(tri.v2, tri.v3, ref vertList, ref middlePointIndexCache, radius);
                    int c = GetMiddlePoint(tri.v3, tri.v1, ref vertList, ref middlePointIndexCache, radius);

                    faces2.Add(new Triangle(tri.v1, a, c));
                    faces2.Add(new Triangle(tri.v2, b, a));
                    faces2.Add(new Triangle(tri.v3, c, b));
                    faces2.Add(new Triangle(a, b, c));
                }
                faces = faces2;
            }

            Mesh mesh = new Mesh();
            mesh.vertices = vertList.ToArray();

            List<int> triList = new List<int>();
            for (int i = 0; i < faces.Count; i++)
            {
                triList.Add(faces[i].v1);
                triList.Add(faces[i].v2);
                triList.Add(faces[i].v3);
            }
            mesh.triangles = triList.ToArray();
            mesh.uv = new Vector2[vertList.Count];

            Vector3[] normales = new Vector3[vertList.Count];
            for (int i = 0; i < normales.Length; i++)
                normales[i] = vertList[i].normalized;


            mesh.normals = normales;

            mesh.RecalculateBounds();
            ;

            return mesh;
        }

        public static Mesh Cylinder(float height, float radius, int segments = 18)
        {
            return Cone(height, radius, radius, segments);
        }

        public static Mesh Cone(float height, float bottomRadius, float topRadius = 1, int segments = 18)
        {
            int nbSides = segments;
            int nbHeightSeg = 1; // Not implemented yet

            int nbVerticesCap = nbSides + 1;
            #region Vertices

            // bottom + top + sides
            Vector3[] vertices = new Vector3[nbVerticesCap + nbVerticesCap + nbSides * nbHeightSeg * 2 + 2];
            int vert = 0;
            float _2pi = Mathf.PI * 2f;

            // Bottom cap
            vertices[vert++] = new Vector3(0f, 0f, 0f);
            while (vert <= nbSides)
            {
                float rad = (float)vert / nbSides * _2pi;
                vertices[vert] = new Vector3(Mathf.Cos(rad) * bottomRadius, 0f, Mathf.Sin(rad) * bottomRadius);
                vert++;
            }

            // Top cap
            vertices[vert++] = new Vector3(0f, height, 0f);
            while (vert <= nbSides * 2 + 1)
            {
                float rad = (float)(vert - nbSides - 1) / nbSides * _2pi;
                vertices[vert] = new Vector3(Mathf.Cos(rad) * topRadius, height, Mathf.Sin(rad) * topRadius);
                vert++;
            }

            // Sides
            int v = 0;
            while (vert <= vertices.Length - 4)
            {
                float rad = (float)v / nbSides * _2pi;
                vertices[vert] = new Vector3(Mathf.Cos(rad) * topRadius, height, Mathf.Sin(rad) * topRadius);
                vertices[vert + 1] = new Vector3(Mathf.Cos(rad) * bottomRadius, 0, Mathf.Sin(rad) * bottomRadius);
                vert += 2;
                v++;
            }
            vertices[vert] = vertices[nbSides * 2 + 2];
            vertices[vert + 1] = vertices[nbSides * 2 + 3];
            #endregion

            #region Normales

            // bottom + top + sides
            Vector3[] normales = new Vector3[vertices.Length];
            vert = 0;

            // Bottom cap
            while (vert <= nbSides)
            {
                normales[vert++] = Vector3.down;
            }

            // Top cap
            while (vert <= nbSides * 2 + 1)
            {
                normales[vert++] = Vector3.up;
            }

            // Sides
            v = 0;
            while (vert <= vertices.Length - 4)
            {
                float rad = (float)v / nbSides * _2pi;
                float cos = Mathf.Cos(rad);
                float sin = Mathf.Sin(rad);

                normales[vert] = new Vector3(cos, 0f, sin);
                normales[vert + 1] = normales[vert];

                vert += 2;
                v++;
            }
            normales[vert] = normales[nbSides * 2 + 2];
            normales[vert + 1] = normales[nbSides * 2 + 3];
            #endregion

            #region UVs
            Vector2[] uvs = new Vector2[vertices.Length];

            // Bottom cap
            int u = 0;
            uvs[u++] = new Vector2(0.5f, 0.5f);
            while (u <= nbSides)
            {
                float rad = (float)u / nbSides * _2pi;
                uvs[u] = new Vector2(Mathf.Cos(rad) * .5f + .5f, Mathf.Sin(rad) * .5f + .5f);
                u++;
            }

            // Top cap
            uvs[u++] = new Vector2(0.5f, 0.5f);
            while (u <= nbSides * 2 + 1)
            {
                float rad = (float)u / nbSides * _2pi;
                uvs[u] = new Vector2(Mathf.Cos(rad) * .5f + .5f, Mathf.Sin(rad) * .5f + .5f);
                u++;
            }

            // Sides
            int u_sides = 0;
            while (u <= uvs.Length - 4)
            {
                float t = (float)u_sides / nbSides;
                uvs[u] = new Vector3(t, 1f);
                uvs[u + 1] = new Vector3(t, 0f);
                u += 2;
                u_sides++;
            }
            uvs[u] = new Vector2(1f, 1f);
            uvs[u + 1] = new Vector2(1f, 0f);
            #endregion

            #region Triangles
            int nbTriangles = nbSides + nbSides + nbSides * 2;
            int[] triangles = new int[nbTriangles * 3 + 3];

            // Bottom cap
            int tri = 0;
            int i = 0;
            while (tri < nbSides - 1)
            {
                triangles[i] = 0;
                triangles[i + 1] = tri + 1;
                triangles[i + 2] = tri + 2;
                tri++;
                i += 3;
            }
            triangles[i] = 0;
            triangles[i + 1] = tri + 1;
            triangles[i + 2] = 1;
            tri++;
            i += 3;

            // Top cap
            //tri++;
            while (tri < nbSides * 2)
            {
                triangles[i] = tri + 2;
                triangles[i + 1] = tri + 1;
                triangles[i + 2] = nbVerticesCap;
                tri++;
                i += 3;
            }

            triangles[i] = nbVerticesCap + 1;
            triangles[i + 1] = tri + 1;
            triangles[i + 2] = nbVerticesCap;
            tri++;
            i += 3;
            tri++;

            // Sides
            while (tri <= nbTriangles)
            {
                triangles[i] = tri + 2;
                triangles[i + 1] = tri + 1;
                triangles[i + 2] = tri + 0;
                tri++;
                i += 3;

                triangles[i] = tri + 1;
                triangles[i + 1] = tri + 2;
                triangles[i + 2] = tri + 0;
                tri++;
                i += 3;
            }
            #endregion

            Mesh mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.normals = normales;
            mesh.uv = uvs;
            mesh.triangles = triangles;

            mesh.RecalculateBounds();
            ;

            return mesh;
        }

        // Prism
        public static Mesh Prism(float height, float baseRadius, int sides = 3)
        {
            return Frustum(height, baseRadius, baseRadius, sides);
        }

        // Frustum
        public static Mesh Frustum(float height, float baseRadius, float topRadius, int sides = 3)
        {
            throw new System.NotImplementedException();
        }

        public static Mesh Sweep(Vector2[] shapePoints, Vector3[] pathPoints, float uvPathScale = 1, bool uvWorldPathDistance = false, bool uvWorldShapeDistance = false, float uvShapeScale = 1)
        {
            Vector3[] vertices = new Vector3[shapePoints.Length * pathPoints.Length];
            Vector3[] meshNormals = new Vector3[vertices.Length];
            Vector2[] uvs = new Vector2[vertices.Length];
            //Triangle[] triangles = new Triangle[(shapePoints.Length * 2 - 2) * pathPoints.Length]; // should be 2 less ??

            List<Triangle> triangleList = new List<Triangle>(); // optimize this

            Vector3 dir = Vector3.zero;

            float[] shapeDistances = null;

            if (uvWorldShapeDistance)
            {
                shapeDistances = new float[shapePoints.Length - 1];

                float totalShapeDistance = 0;

                for (int i = 0; i < shapeDistances.Length; i++)
                {
                    totalShapeDistance += Vector2.Distance(shapePoints[i], shapePoints[i + 1]);
                    shapeDistances[i] = totalShapeDistance;
                }
            }

            float totalPathLength = 0;

            for (int i = 0; i < pathPoints.Length; i++)
            {
                if (i < pathPoints.Length - 1)
                    dir = pathPoints[i + 1] - pathPoints[i];

                // find up and right
                Vector3 up = Vector3.up; // for now just use up up
                Vector3 right = -Vector3.Cross(dir.normalized, up);

                Vector3 pp = pathPoints[i];

                float uvY = i * uvPathScale;

                if (uvWorldPathDistance)
                {
                    if (i != 0)
                    {
                        totalPathLength += Vector3.Distance(pathPoints[i], pathPoints[i - 1]);

                        uvY = totalPathLength * uvPathScale;
                    }
                }

                // vertices
                for (int s = 0; s < shapePoints.Length; s++)
                {
                    int index = i * shapePoints.Length + s;
                    vertices[index] = pp + right * shapePoints[s].x + up * shapePoints[s].y;
                    meshNormals[index] = Vector3.up;



                    if (!uvWorldShapeDistance)
                        uvs[index] = new Vector2((s / ((float)shapePoints.Length - 1)) * uvShapeScale, uvY);
                    else
                        uvs[index] = new Vector2(s == 0 ? 0 : shapeDistances[s - 1] * uvShapeScale, uvY);

                    if (i > 0 && s > 0)
                    {
                        int lowIndex = index - shapePoints.Length;

                        triangleList.Add(new Triangle(lowIndex - 1, index - 1, lowIndex));
                        triangleList.Add(new Triangle(lowIndex, index - 1, index));
                    }
                }
            }

            Mesh m = NewMesh(vertices, meshNormals, uvs, triangleList.ToArray());
            m.RecalculateNormals();
            return m;
        }

        // QuadStrip
        public static Mesh Quadstrip(Vector3[] points, Vector3[] normals, float width, float uvScale = 1)
        {
            if (points.Length < 2) return null;

            Vector3[] vertices = new Vector3[points.Length * 2];
            Vector3[] meshNormals = new Vector3[points.Length * 2];
            Vector2[] uvs = new Vector2[points.Length * 2];
            Triangle[] triangles = new Triangle[points.Length * 2]; // should be 2 less

            Vector3 dir = Vector3.zero;

            for (int i = 0; i < points.Length; i++)
            {
                if (i != points.Length - 1) dir = points[i + 1] - points[i];
                Vector3 right = Vector3.Cross(dir.normalized, normals[i]);

                Vector3 p0 = points[i] + right * (width * 0.5f);
                Vector3 p1 = points[i] + right * -(width * 0.5f);

                vertices[i * 2] = p0;
                vertices[i * 2 + 1] = p1;

                // normals
                meshNormals[i * 2] = meshNormals[i * 2 + 1] = normals[i];

                // uvs
                uvs[i * 2] = new Vector2(0, i * uvScale);
                uvs[i * 2 + 1] = new Vector2(1, i * uvScale);

                // triangles
                if (i != 0)
                {
                    int v = i * 2;

                    triangles[i * 2] = new Triangle(v - 2, v, v - 1);
                    triangles[i * 2 + 1] = new Triangle(v - 1, v, v + 1);

                    /*
                    triangles[i * 2] = new Triangle(v - 2, v, v - 1);
                    triangles[i * 2 + 1] = new Triangle(v - 1, v, v + 1);
                    */
                }
            }

            return NewMesh(vertices, meshNormals, uvs, triangles);
        }

        public static Mesh StairsBounds(float width, float totalHeight, float totalDepth, bool sideWalls = true, float targetStepHeight = 0.3f)
        {
            if (targetStepHeight <= 0) targetStepHeight = 0.3f;

            int stepsNum = Mathf.RoundToInt(totalHeight / targetStepHeight);

            float stepDepth = totalDepth / stepsNum;
            float stepHeight = totalHeight / stepsNum;

            return Stairs(stepsNum, width, stepHeight, stepDepth, sideWalls);
        }

        public static Mesh Stairs(int stepsNum, float width, float stepHeight, float stepDepth, bool sideWalls = true)
        {
            if (stepsNum < 1) return null;

            // halfwidth
            float hW = width * 0.5f;

            // vertices

            int vertCount = sideWalls ? 22 : 12;

            Vector3[] vertices = new Vector3[stepsNum * vertCount];
            //Vector3[] normals = new Vector3[vertices.Length];

            float depth = stepDepth;
            float height = stepHeight;

            for (int step = 0; step < stepsNum; step++)
            {
                int v = step * vertCount;

                float z = step * depth;
                float z2 = (step + 1) * depth;
                float z3 = (step + 2) * depth;
                float y = step * height;
                float yUp = (step + 1) * height;
                //float yUp2 = (step + 2) * height;

                // front side
                vertices[v + 0] = new Vector3(-hW, y, z);
                vertices[v + 1] = new Vector3(hW, y, z);
                vertices[v + 2] = new Vector3(-hW, yUp, z);
                vertices[v + 3] = new Vector3(hW, yUp, z);

                // upper side
                vertices[v + 4] = new Vector3(-hW, yUp, z);
                vertices[v + 5] = new Vector3(hW, yUp, z);
                vertices[v + 6] = new Vector3(-hW, yUp, z2);
                vertices[v + 7] = new Vector3(hW, yUp, z2);

                // back slant side
                vertices[v + 8] = new Vector3(hW, y, z2);
                vertices[v + 9] = new Vector3(-hW, y, z2);
                vertices[v + 10] = new Vector3(hW, yUp, z3);
                vertices[v + 11] = new Vector3(-hW, yUp, z3);

                if (sideWalls)
                {
                    // left side
                    vertices[v + 12] = new Vector3(-hW, y, z);
                    vertices[v + 13] = new Vector3(-hW, y, z2);
                    vertices[v + 14] = new Vector3(-hW, yUp, z);
                    vertices[v + 15] = new Vector3(-hW, yUp, z2);
                    vertices[v + 16] = new Vector3(-hW, yUp, z3);

                    // right side
                    vertices[v + 17] = new Vector3(hW, y, z);
                    vertices[v + 18] = new Vector3(hW, y, z2);
                    vertices[v + 19] = new Vector3(hW, yUp, z);
                    vertices[v + 20] = new Vector3(hW, yUp, z2);
                    vertices[v + 21] = new Vector3(hW, yUp, z3);
                }
            }

            // Triangles
            List<Triangle> triangles = new List<Triangle>();

            for (int step = 0; step < stepsNum; step++)
            {
                int startV = step * vertCount;

                for (int quad = 0; quad < 3; quad++)
                {
                    int q = startV + quad * 4;
                    triangles.Add(new Triangle(q, q + 2, q + 1));
                    triangles.Add(new Triangle(q + 1, q + 2, q + 3));
                }

                if (sideWalls)
                {
                    int swq = startV + 12;
                    triangles.Add(new Triangle(swq, swq + 1, swq + 2));
                    triangles.Add(new Triangle(swq + 1, swq + 3, swq + 2));
                    if (step != stepsNum - 1) triangles.Add(new Triangle(swq + 1, swq + 4, swq + 3));

                    swq = startV + 17;
                    triangles.Add(new Triangle(swq, swq + 2, swq + 1));
                    triangles.Add(new Triangle(swq + 1, swq + 2, swq + 3));
                    if (step != stepsNum - 1) triangles.Add(new Triangle(swq + 1, swq + 3, swq + 4));

                }
            }

            Mesh m = NewMesh(vertices, null, null, triangles.ToArray());
            m.RecalculateNormals();

            return m;
        }

        static Mesh NewMesh(Vector3[] vertices, Vector3[] normals, Vector2[] uvs, Triangle[] triangles)
        {
            int[] intTris = Triangle.ToIntArray(triangles);

            return NewMesh(vertices, normals, uvs, intTris);
        }

        static Mesh NewMesh(Vector3[] vertices, Vector3[] normals, Vector2[] uvs, int[] tris)
        {
            Mesh m = new Mesh();
            m.vertices = vertices;
            m.normals = normals;
            m.uv = uvs;
            m.triangles = tris;

            return m;
        }

        #region DO NOT USE deprecated, sorta
        // old cubes - used in some projects
        // draw quad? need to see that

        [System.Obsolete]
        public static Mesh GetCube(Vector3 rootPos, float length, float width, float height, bool bottomPivot, int subTiles, float uvScale)
        {
            Mesh mesh = new Mesh();
            mesh.Clear();

            #region Vertices
            Vector3 p0 = rootPos + new Vector3(-length * .5f, -height * .5f, width * .5f);
            Vector3 p1 = rootPos + new Vector3(length * .5f, -height * .5f, width * .5f);
            Vector3 p2 = rootPos + new Vector3(length * .5f, -height * .5f, -width * .5f);
            Vector3 p3 = rootPos + new Vector3(-length * .5f, -height * .5f, -width * .5f);

            Vector3 p4 = rootPos + new Vector3(-length * .5f, height * .5f, width * .5f);
            Vector3 p5 = rootPos + new Vector3(length * .5f, height * .5f, width * .5f);
            Vector3 p6 = rootPos + new Vector3(length * .5f, height * .5f, -width * .5f);
            Vector3 p7 = rootPos + new Vector3(-length * .5f, height * .5f, -width * .5f);

            Vector3[] vertices = new Vector3[]
            {
	    // Bottom
	    p0, p1, p2, p3,
 
	    // Left
	    p7, p4, p0, p3,
 
	    // Front
	    p4, p5, p1, p0,
 
	    // Back
	    p6, p7, p3, p2,
 
	    // Right
	    p5, p6, p2, p1,
 
	    // Top
	    p7, p6, p5, p4
            };

            if (bottomPivot)
            {
                for (int i = 0; i < vertices.Length; i++)
                {
                    vertices[i] += Vector3.up * (height / 2);
                }
            }

            #endregion

            #region Normals
            Vector3 up = Vector3.up;
            Vector3 down = Vector3.down;
            Vector3 front = Vector3.forward;
            Vector3 back = Vector3.back;
            Vector3 left = Vector3.left;
            Vector3 right = Vector3.right;

            Vector3[] normals = new Vector3[]
            {
	// Bottom
	down, down, down, down,
 
	// Left
	left, left, left, left,
 
	// Front
	front, front, front, front,
 
	// Back
	back, back, back, back,
 
	// Right
	right, right, right, right,
 
	// Top
	up, up, up, up
            };
            #endregion

            #region UVs

            Vector2[] uvs = NormalProjection(RoundToSubV3(vertices, subTiles, uvScale), normals);

            #endregion

            #region Triangles
            int[] triangles = new int[]
            {
	// Bottom
	3, 1, 0,
    3, 2, 1,			
 
	// Left
	3 + 4 * 1, 1 + 4 * 1, 0 + 4 * 1,
    3 + 4 * 1, 2 + 4 * 1, 1 + 4 * 1,
 
	// Front
	3 + 4 * 2, 1 + 4 * 2, 0 + 4 * 2,
    3 + 4 * 2, 2 + 4 * 2, 1 + 4 * 2,
 
	// Back
	3 + 4 * 3, 1 + 4 * 3, 0 + 4 * 3,
    3 + 4 * 3, 2 + 4 * 3, 1 + 4 * 3,
 
	// Right
	3 + 4 * 4, 1 + 4 * 4, 0 + 4 * 4,
    3 + 4 * 4, 2 + 4 * 4, 1 + 4 * 4,
 
	// Top
	3 + 4 * 5, 1 + 4 * 5, 0 + 4 * 5,
    3 + 4 * 5, 2 + 4 * 5, 1 + 4 * 5,

            };
            #endregion

            mesh.vertices = vertices;
            mesh.normals = normals;
            mesh.uv = uvs;
            mesh.triangles = triangles;

            mesh.RecalculateBounds();

            return mesh;
        }

        [System.Obsolete]
        public static Mesh GetCubeTest(Vector3 rootPos, float length, float width, float height, bool bottomPivot, int subTiles, float uvScale)
        {
            Mesh mesh = new Mesh();
            mesh.Clear();

            #region Vertices
            Vector3 p0 = rootPos + new Vector3(-length * .5f, -height * .5f, width * .5f);
            Vector3 p1 = rootPos + new Vector3(length * .5f, -height * .5f, width * .5f);
            Vector3 p2 = rootPos + new Vector3(length * .5f, -height * .5f, -width * .5f);
            Vector3 p3 = rootPos + new Vector3(-length * .5f, -height * .5f, -width * .5f);

            Vector3 p4 = rootPos + new Vector3(-length * .5f, height * .5f, width * .5f);
            Vector3 p5 = rootPos + new Vector3(length * .5f, height * .5f, width * .5f);
            Vector3 p6 = rootPos + new Vector3(length * .5f, height * .5f, -width * .5f);
            Vector3 p7 = rootPos + new Vector3(-length * .5f, height * .5f, -width * .5f);

            Vector3[] vertices = new Vector3[]
            {
	    // Bottom
	    p0, p1, p2, p3,
 
	    // Left
	    p7, p4, p0, p3,
 
	    // Front
	    p4, p5, p1, p0,
 
	    // Back
	    p6, p7, p3, p2,
 
	    // Right
	    p5, p6, p2, p1,
 
	    // Top
	    p7, p6, p5, p4
            };

            if (bottomPivot)
            {
                for (int i = 0; i < vertices.Length; i++)
                {
                    vertices[i] += Vector3.up * (height / 2);
                }
            }

            #endregion

            #region Normals
            Vector3 up = Vector3.up;
            Vector3 down = Vector3.down;
            Vector3 front = Vector3.forward;
            Vector3 back = Vector3.back;
            Vector3 left = Vector3.left;
            Vector3 right = Vector3.right;

            Vector3[] normals = new Vector3[]
            {
	// Bottom
	down, down, down, down,
 
	// Left
	left, left, left, left,
 
	// Front
	front, front, front, front,
 
	// Back
	back, back, back, back,
 
	// Right
	right, right, right, right,
 
	// Top
	up, up, up, up
            };
            #endregion

            #region UVs

            Vector2[] uvs = NormalProjection(RoundToSubV3(vertices, subTiles, uvScale), normals);

            #endregion

            #region Triangles
            int[] triangles = new int[]
            {
            	// Front
            	3 + 4 * 2, 1 + 4 * 2, 0 + 4 * 2,
                3 + 4 * 2, 2 + 4 * 2, 1 + 4 * 2,
            };
            #endregion

            mesh.vertices = vertices;
            mesh.normals = normals;
            mesh.uv = uvs;
            mesh.triangles = triangles;

            mesh.RecalculateBounds();
            ;

            return mesh;
        }

        [System.Obsolete]
        public static Mesh Quad(Vector3 rootPos, float width, float height, Vector3 normalDir, float deviateNormal = 0, bool bottomPivot = false, int subTiles = 1, float uvScale = 1)
        {
            Mesh mesh = new Mesh();
            mesh.Clear();

            #region Vertices
            Vector3 p0 = new Vector3(-width * .5f, height * .5f);
            Vector3 p1 = new Vector3(width * .5f, height * .5f);
            Vector3 p2 = new Vector3(-width * .5f, -height * .5f);
            Vector3 p3 = new Vector3(width * .5f, -height * .5f);

            Vector3[] vertices = new Vector3[]
            {
            p0, p1, p2, p3,
            };

            vertices = FromToDirVertices(vertices, normalDir);

            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] += rootPos;

                if (bottomPivot)
                    vertices[i] += Vector3.up * (height / 2);
            }

            #endregion

            #region Normals

            Vector3[] normals = new Vector3[4];

            Vector3 n = normalDir;
            if (deviateNormal != 0)
                n = DeviateDir(n, deviateNormal);

            for (int i = 0; i < 4; i++)
            {
                normals[i] = n;

                //if (deviateNormal != 0)
                //normals[i] = DeviateDir(normals[i], deviateNormal);
            }
            #endregion

            #region UVs

            Vector2[] uvs = NormalProjection(RoundToSubV3(vertices, subTiles, uvScale), normals);

            #endregion

            #region Triangles
            int[] triangles = new int[]
            {
            2, 1, 0,
            2, 3, 1
            };
            #endregion

            mesh.vertices = vertices;
            mesh.normals = normals;
            mesh.uv = uvs;
            mesh.triangles = triangles;

            mesh.RecalculateBounds();
            ;

            return mesh;
        }

        #endregion


        /*
        public static void CleanMesh(Mesh mesh)
        {
            // get its arrays:
            var tris = mesh.triangles;
            var verts = mesh.vertices;
            var uvs = mesh.uv;
            var normals = mesh.normals;

            // create the lists:
            var newVerts = new List<Vector3>();
            var newUvs = new List<Vector2>();
            var newNormals = new List<Vector3>();

            // iterate through all triangle entries:
            for (var i = 0; i < tris.Length; i++)
            {
                // for each triangle entry...
                var t = tris[i];
                // get its vertex, uv and normal
                var vert = verts[t];
                var normal = normals[t];
                var uv = uvs[t];
                // vertexFound is the compatible vertex index, if any:
                var vertexFound = -1;

                // check if the vertex is already in the list:
                for (var j = 0; j < newVerts; j++)
                {
                    // if compatible vertex already in the new list...
                    if (vert == newVerts[j] && normal == newNormals[j] && uv == newUvs[j])
                    {
                        vertexFound = j; // get its index...
                        break; // and stop the loop
                    }
                }
                if (vertexFound < 0)
                { // if no compatible vertex in the list...
                  // get the index of the next new element...
                    vertexFound = newVerts.Count;
                    // then add the vertex and attributes to the list
                    newVerts.Add(vert); // add new vertex...
                    newUvs.Add(uv); // and attributes to the list
                    newNormals.Add(normal);
                }
                tris[i] = vertexFound; // anyway, update triangle entry...
            }

            mesh.vertices = newVerts.ToArray();
            mesh.uv = newUvs.ToArray();
            mesh.normals = newNormals.ToArray();
        }
        */



        public static CombineInstance GetBlockInstance(Mesh m, int index = 0, bool destroyImmediate = false)
        {
            CombineInstance combine = new CombineInstance();
            combine.mesh = m;
            combine.transform = Matrix4x4.identity;
            combine.subMeshIndex = index;

            if (!destroyImmediate)
                GameObject.Destroy(m);

            return combine;
        }


        public static Mesh Combine(Mesh[] meshes, bool recalculateBounds = true, bool destroyImmediate = false)
        {
            CombineInstance[] instances = new CombineInstance[meshes.Length];

            for (int i = 0; i < meshes.Length; i++)
                instances[i] = meshes[i].GetCombineInstance(0, false);

            Mesh m = new Mesh();
            m.CombineMeshes(instances, true);

            for (int i = 0; i < meshes.Length; i++)
                if (destroyImmediate) GameObject.DestroyImmediate(meshes[i]);
                else GameObject.Destroy(meshes[i]);

            if (recalculateBounds)
                m.RecalculateBounds();


            return m;
        }

        // Combine Instance

        public static CombineInstance GetCombineInstance(this Mesh m, int index = 0, bool destroyMesh = true, bool destroyImmediate = false)
        {
            CombineInstance instance = new CombineInstance();
            instance.mesh = m;
            instance.transform = Matrix4x4.identity;
            instance.subMeshIndex = index;

            if (destroyMesh)
                if (!destroyImmediate)
                    GameObject.Destroy(m);
                else
                    GameObject.DestroyImmediate(m);

            return instance;
        }

        //----------------
        // INITIALIZATION
        //----------------

        /// <summary>
        /// Creates necessary components to render a mesh
        /// </summary>
        /// <param name="go"></param>
        /// <param name="mesh">A mesh to be set to MeshFilter</param>
        /// <param name="material">A material to be set to the MeshRenderer</param>
        public static void InitMesh(this GameObject go, Mesh mesh = null, Material material = null)
        {
            if (material == null) material = MeshX.GetDefaultMaterial();

            MeshFilter mf = go.GetComponent<MeshFilter>();

            if (mf == null)
                mf = go.AddComponent<MeshFilter>();

            if (mesh) mf.mesh = mesh;

            MeshRenderer r = go.GetComponent<MeshRenderer>();

            if (r == null)
                r = go.AddComponent<MeshRenderer>();

            r.sharedMaterial = material;
        }

        /// <summary>
        /// Creates a new GameObject at this object's place, and initializes it for rendering a mesh
        /// Adds a MeshFilter and MeshRenderer and assigns mesh and material, if present
        /// </summary>
        /// <param name="thisObject">Parent Object</param>
        /// <param name="mesh">The mesh to be applied to MeshFilter</param>
        /// <param name="material">The material to be assigned to MeshRenderer</param>
        /// <returns>Returns new separate object</returns>
        public static GameObject InitChildMesh(this GameObject thisObject, Mesh mesh = null, Material material = null)
        {
            GameObject go = new GameObject("MeshObject");
            go.transform.parent = thisObject.transform;
            go.transform.localPosition = Vector3.zero;
            go.transform.localEulerAngles = Vector3.zero;

            go.InitMesh(mesh, material);

            return go;
        }

        public static void SetMesh(this GameObject thisObject, Mesh mesh, bool autoAddFilter = true)
        {
            MeshFilter mf = thisObject.GetComponent<MeshFilter>();

            if (!mf)
            {
                if (autoAddFilter)
                {
                    thisObject.InitMesh(mesh);
                    return;
                }
                else
                {
                    Debug.LogWarning("The object doesn't have a MeshFilter");
                    return;
                }
            }

            GameObject.Destroy(mf.sharedMesh);
            mf.sharedMesh = mesh;
        }

        public static void SetMaterial(this GameObject thisObject, Material material, bool autoAddRenderer = true)
        {
            MeshRenderer mr = thisObject.GetComponent<MeshRenderer>();

            if (!mr)
            {
                if (autoAddRenderer)
                {
                    thisObject.InitMesh(null, material); // TEST this
                    return;
                }
                else
                {
                    Debug.LogWarning("The object doesn't have a MeshRenderer");
                    return;
                }
            }

            mr.material = material;
        }

        #region Mesh Transformations

        // Mesh operations

        /// <summary>
        /// Centers mesh to average vertices point
        /// </summary>
        public static void CenterToAverage(this Mesh mesh)
        {
            Vector3 sum = Vector3.zero;

            for (int i = 0; i < mesh.vertices.Length; i++)
            {
                sum = mesh.vertices[i];
            }

            sum /= mesh.vertices.Length; // DIVISION BY ZERO

            mesh.Translate(-sum);
        } // untested

        /// <summary>
        /// Centers mesh to it's bounds center. Prefered variant over CenterToAverage
        /// </summary>
        /// <param name="mesh"></param>
        public static void CenterToBounds(this Mesh mesh)
        {
            Vector3 boundsCenter = mesh.bounds.center;

            mesh.Translate(-boundsCenter);
        } // untested

        public static void Translate(this Mesh mesh, Vector3 by)
        {
            if (mesh == null) return;

            mesh.vertices = MeshX.TranslateVertices(mesh.vertices, by);
        }

        public static void Rotate(this Mesh mesh, float degrees, Vector3 axis)
        {
            if (mesh == null) return;

            mesh.vertices = MeshX.RotateVertices(mesh.vertices, degrees, axis);
            mesh.normals = MeshX.RotateVertices(mesh.normals, degrees, axis);
        }



        [System.Obsolete]
        public static void RotateMesh(Mesh mesh, float degrees, Vector3 axis)
        {
            mesh.vertices = RotateVertices(mesh.vertices, degrees, axis);
            mesh.normals = RotateVertices(mesh.normals, degrees, axis);
        }

        public static Vector3[] TranslateVertices(Vector3[] vertices, Vector3 by)
        {
            if (vertices == null) return null;
            if (vertices.Length == 0) return vertices;

            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] += by;
            }

            return vertices;
        }

        public static Vector3[] RotateVertices(Vector3[] vertices, float angle, Vector3 axis)
        {
            if (vertices == null) return null;
            if (vertices.Length == 0) return vertices;

            //Vector3[] rotatedVerts = new Vector3[vertices.Length];

            Quaternion qAngle = Quaternion.AngleAxis(angle, axis);
            for (int vert = 0; vert < vertices.Length; vert++)
            {
                vertices[vert] = qAngle * vertices[vert];
            }

            return vertices;
        }


        public static CombineInstance GetInstanceWithMatrix(this Mesh m, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            Matrix4x4 m4 = new Matrix4x4();

            m4.SetTRS(position, rotation, scale);

            CombineInstance ci = new CombineInstance();
            ci.mesh = m;
            ci.transform = m4;

            return ci;
        }



        #endregion

        #region Matrix Utils

        public static void FromMatrix4x4(this Transform transform, Matrix4x4 matrix)
        {
            transform.localScale = matrix.GetScale();
            transform.rotation = matrix.GetRotation();
            transform.position = matrix.GetPosition();
        }

        public static Quaternion GetRotation(this Matrix4x4 matrix)
        {
            var qw = Mathf.Sqrt(1f + matrix.m00 + matrix.m11 + matrix.m22) / 2;
            var w = 4 * qw;
            var qx = (matrix.m21 - matrix.m12) / w;
            var qy = (matrix.m02 - matrix.m20) / w;
            var qz = (matrix.m10 - matrix.m01) / w;

            return new Quaternion(qx, qy, qz, qw);
        }

        public static Vector3 GetPosition(this Matrix4x4 matrix)
        {
            var x = matrix.m03;
            var y = matrix.m13;
            var z = matrix.m23;

            return new Vector3(x, y, z);
        }

        public static Vector3 GetScale(this Matrix4x4 m)
        {
            var x = Mathf.Sqrt(m.m00 * m.m00 + m.m01 * m.m01 + m.m02 * m.m02);
            var y = Mathf.Sqrt(m.m10 * m.m10 + m.m11 * m.m11 + m.m12 * m.m12);
            var z = Mathf.Sqrt(m.m20 * m.m20 + m.m21 * m.m21 + m.m22 * m.m22);

            return new Vector3(x, y, z);
        }

        #endregion

        #region Utils of utils

        static MethodInfo getBuiltinExtraResourcesMethod;

        /// <summary>
        /// Gets Unity editor's default material. NOTE: It does not work in build, null will be returned.
        /// </summary>
        public static Material GetDefaultMaterial()
        {
#if !UNITY_EDITOR
            return null;
#else
            if (getBuiltinExtraResourcesMethod == null)
            {
                BindingFlags bfs = BindingFlags.NonPublic | BindingFlags.Static;
                getBuiltinExtraResourcesMethod = typeof(EditorGUIUtility).GetMethod("GetBuiltinExtraResource", bfs);
            }
            return (Material)getBuiltinExtraResourcesMethod.Invoke(null, new object[] { typeof(Material), "Default-Diffuse.mat" });
#endif
        }

        public static Vector3[] FromToDirVertices(Vector3[] vertices, Vector3 toDir)
        {
            Vector3[] rotatedVerts = new Vector3[vertices.Length];

            Vector3 fromDir = Vector3.forward;

            Quaternion qAngle = Quaternion.FromToRotation(fromDir, toDir);
            for (int vert = 0; vert < vertices.Length; vert++)
            {
                rotatedVerts[vert] = qAngle * vertices[vert];
            }

            return rotatedVerts;
        }

        static Vector3[] RoundToSubV3(Vector3[] vectors, int subTiles, float uvScale)
        {
            Vector3[] vs = (Vector3[])vectors.Clone();

            for (int i = 0; i < vs.Length; i++)
            {
                if (subTiles != 0)
                    vs[i] = new Vector3(RtF(vs[i].x * uvScale, subTiles), RtF(vs[i].y * uvScale, subTiles), RtF(vs[i].z * uvScale, subTiles));
                else
                    vs[i] = new Vector3(Mathf.Round(vs[i].x * subTiles), Mathf.Round(vs[i].y), Mathf.Round(vs[i].z));
            }

            return vs;
        }

        /// <summary>
        /// Round to fraction
        /// </summary>
        /// <param name="f">Input value</param>
        /// <param name="fraction">Fraction to round to: 4 means value will be rounded to 0.25f</param>
        static float RtF(float f, int fraction)
        {
            if (fraction == 0) fraction = 1; // overrides divisions

            float sf = f;

            // 4.2    (4.2 * 4) = 17            17 /4 = 4.25
            // 0.1    (0.1 * 4) = 0.4 = 0         

            sf *= fraction;

            if (sf <= 0.5f && sf != 0)
                sf = Mathf.Ceil(sf);
            else
            {
                sf = Mathf.Round(sf);
            }

            return sf / fraction;
        }

        static Vector2[] NormalProjection(Vector3[] vertices, Vector3[] normals)
        {
            return NormalProjection(vertices, Vector3.zero, normals);
        }

        static Vector2[] NormalProjection(Vector3[] vertices, Vector3 uvOffset, Vector3[] normals)
        {
            Vector2[] uv = new Vector2[vertices.Length];

            for (int i = 0; i < vertices.Length; i++)
            {
                Vector3 v = vertices[i] + uvOffset;

                if (VAbs(normals[i]) == Vector3.up)
                {
                    uv[i].x = v.x;
                    uv[i].y = v.z;
                }
                else
                    if (VAbs(normals[i]) == Vector3.forward)
                {
                    uv[i].x = v.x;
                    uv[i].y = v.y;
                }
                else
                {
                    uv[i].x = v.z;
                    uv[i].y = v.y;
                }
            }

            return uv;
        }

        public static Vector3 DeviateDir(Vector3 dir, float deviation)
        {
            return dir + new Vector3(Random.Range(-deviation, deviation), Random.Range(-deviation, deviation), Random.Range(-deviation, deviation));
        }

        // return index of point in the middle of p1 and p2
        private static int GetMiddlePoint(int p1, int p2, ref List<Vector3> vertices, ref Dictionary<long, int> cache, float radius)
        {
            // first check if we have it already
            bool firstIsSmaller = p1 < p2;
            long smallerIndex = firstIsSmaller ? p1 : p2;
            long greaterIndex = firstIsSmaller ? p2 : p1;
            long key = (smallerIndex << 32) + greaterIndex;

            int ret;
            if (cache.TryGetValue(key, out ret))
            {
                return ret;
            }

            // not in cache, calculate it
            Vector3 point1 = vertices[p1];
            Vector3 point2 = vertices[p2];
            Vector3 middle = new Vector3
            (
                (point1.x + point2.x) / 2f,
                (point1.y + point2.y) / 2f,
                (point1.z + point2.z) / 2f
            );

            // add vertex makes sure point is on unit sphere
            int i = vertices.Count;
            vertices.Add(middle.normalized * radius);

            // store it, return index
            cache.Add(key, i);

            return i;
        }

        /// <summary>
        /// Returns Vector3 with absolute values
        /// </summary>
        /// <returns>Vector3 with absolute values</returns>
        static Vector3 VAbs(Vector3 v) { return new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z)); }

        [System.Obsolete("Use Vector3 extension method instead")]
        /// <summary>
        /// Converts Vector3 array to Vector2 array
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        static Vector2[] ToV2A(Vector3[] v)
        {
            Vector2[] v2 = new Vector2[v.Length];

            for (int i = 0; i < v.Length; i++)
            {
                v2[i].x = v[i].x;
                v2[i].y = v[i].y;
            }

            return v2;
        }

        /// <summary>
        /// Converts Vector3 array to Vector2 array by discarding the z value
        /// </summary>
        public static Vector2[] ToVector2Array(this Vector3[] v3s)
        {
            Vector2[] v2s = new Vector2[v3s.Length];

            for (int i = 0; i < v3s.Length; i++)
            {
                v2s[i].x = v3s[i].x;
                v2s[i].y = v3s[i].y;
            }

            return v2s;
        }

        #endregion
    }
}
