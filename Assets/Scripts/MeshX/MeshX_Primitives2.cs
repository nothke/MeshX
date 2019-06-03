using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeshXtensions
{
    public static class PointUtils
    {
        public static Vector3[] CirclePoints(int sides, float radius)
        {
            Vector3[] points = new Vector3[sides + 1];

            for (int i = 0; i < sides + 1; i++)
            {
                float theta = i * 2 * Mathf.PI / sides;

                Vector3 ci = new Vector3(Mathf.Cos(theta) * radius, 0, Mathf.Sin(theta) * radius);

                points[i] = ci;
            }

            return points;
        }

        public static Vector3[] CirclePoints(int sides, float radius, Vector3 normal)
        {
            normal = normal.normalized;
            Vector3 forward = normal == Vector3.up ?
                Vector3.ProjectOnPlane(Vector3.forward, normal).normalized :
                Vector3.ProjectOnPlane(Vector3.up, normal);
            Vector3 right = Vector3.Cross(normal, forward);

            Vector3[] points = new Vector3[sides + 1];

            for (int i = 0; i < sides + 1; i++)
            {
                float theta = i * 2 * Mathf.PI / sides;

                Vector3 ci = forward * Mathf.Cos(theta) * radius + right * Mathf.Sin(theta) * radius;

                points[i] = ci;
            }

            return points;
        }
    }

    public class CutoffSphere : Geom
    {
        public CutoffSphere(int radialSegments, int verticalSegments, float radius, float cutoffMin, float cutoffMax)
        {
            if (radialSegments < 3) radialSegments = 3;
            if (verticalSegments < 3) verticalSegments = 3;

            vertices = new List<Vector3>();
            triangles = new List<Triangle>();

            for (int v = 0; v < verticalSegments + 1; v++)
            {
                float dist = cutoffMin + ((float)v / verticalSegments) * (cutoffMax - cutoffMin);

                float height = -Mathf.Cos(dist * Mathf.PI) * radius;
                float ringRadius = Mathf.Sin(dist * Mathf.PI) * radius;
                Vector3[] verts = PointUtils.CirclePoints(radialSegments, ringRadius);

                for (int i = 0; i < radialSegments + 1; i++)
                {
                    verts[i] += new Vector3(0, height, 0);
                }

                vertices.AddRange(verts);
            }

            GridMesh g = new GridMesh(radialSegments + 1, vertices.ToArray());
            //tris = g.GetTris();
            AddAsTris(g.GetTris());

            normals = new List<Vector3>();

            for (int i = 0; i < vertices.Count; i++)
                normals.Add(vertices[i].normalized);
        }
    }

    /// <summary>
    /// A longitude/latitude sphere
    /// </summary>
    public class Sphere : Geom
    {
        public Sphere(int radialSegments, int verticalSegments, float radius)
        {
            if (radialSegments < 3) radialSegments = 3;
            if (verticalSegments < 3) verticalSegments = 3;

            vertices = new List<Vector3>();
            triangles = new List<Triangle>();

            for (int v = 0; v < verticalSegments + 1; v++)
            {
                float height = -Mathf.Cos((float)v / verticalSegments * Mathf.PI) * radius;
                float ringRadius = Mathf.Sin((float)v / verticalSegments * Mathf.PI) * radius;
                Vector3[] verts = PointUtils.CirclePoints(radialSegments, ringRadius);

                for (int i = 0; i < radialSegments + 1; i++)
                {
                    verts[i] += new Vector3(0, height, 0);
                }

                vertices.AddRange(verts);
            }

            GridMesh g = new GridMesh(radialSegments + 1, vertices.ToArray());
            AddAsTris(g.GetTris());

            normals = new List<Vector3>();

            for (int i = 0; i < vertices.Count; i++)
                normals.Add(vertices[i].normalized);
        }
    }

    public class ConicStrip : Geom
    {
        public ConicStrip(int radialSegments, float height, float bottomRadius, float topRadius)
        {
            if (radialSegments < 3) radialSegments = 3;

            vertices = new List<Vector3>();

            // Bottom circle
            Vector3[] verts = PointUtils.CirclePoints(radialSegments, bottomRadius);
            for (int i = 0; i < verts.Length; i++) { verts[i] += new Vector3(0, -height * 0.5f); }

            vertices.AddRange(verts);

            // Top circle
            verts = PointUtils.CirclePoints(radialSegments, topRadius);
            for (int i = 0; i < verts.Length; i++) { verts[i] += new Vector3(0, +height * 0.5f); }

            vertices.AddRange(verts);

            GridMesh g = new GridMesh(radialSegments + 1, vertices.ToArray());

            AddAsTris(g.GetTris());

            float dR = bottomRadius - topRadius;
            Vector2 nr2 = new Vector2(height, dR).normalized;

            Vector3[] norms = new Vector3[vertices.Count];


            for (int i = 0; i < radialSegments + 1; i++)
            {
                Vector3 normal = vertices[i].normalized;
                normal.y = nr2.y;
                normal = normal.normalized;

                norms[i] = normal;
                norms[i + radialSegments + 1] = normal;
            }

            normals = new List<Vector3>(norms);
        }
    }

    public class Cylinder : Geom
    {
        public Cylinder(int radialSegments, float height, float radius)
        {
            if (radialSegments < 3) radialSegments = 3;

            vertices = new List<Vector3>();

            // Bottom circle
            Vector3[] verts = PointUtils.CirclePoints(radialSegments, radius);
            for (int i = 0; i < verts.Length; i++) { verts[i] += new Vector3(0, height * 0.5f); }

            vertices.AddRange(verts);

            // Top circle
            verts = PointUtils.CirclePoints(radialSegments, radius);
            for (int i = 0; i < verts.Length; i++) { verts[i] += new Vector3(0, height * 0.5f); }

            vertices.AddRange(verts);

            Strip s = new Strip(vertices.ToArray());

            AddAsTris(s.GetTris());

            normals = new List<Vector3>();

            for (int i = 0; i < vertices.Count; i++)
            {
                Vector3 normal = vertices[i].normalized;
                normal.y = 0;

                normals.Add(normal);
            }
        }
    }

    public class OilTank : Geom
    {
        public OilTank(float radius, float height, int sides, float squash = 1)
        {
            vertices = new List<Vector3>();
            triangles = new List<Triangle>();
            normals = new List<Vector3>();

            Geom topSphere = new CutoffSphere(sides, sides / 2, radius, 0.5f, 1);
            topSphere.Scale(new Vector3(1, squash, 1));
            topSphere.Translate(new Vector3(0, height * 0.5f, 0));

            Geom bottomSphere = new CutoffSphere(sides, sides / 2, radius, 0, 0.5f);
            bottomSphere.Scale(new Vector3(1, squash, 1));
            bottomSphere.Translate(new Vector3(0, -height * 0.5f, 0));

            Geom cylinder = new ConicStrip(sides, height, radius, radius);

            CombineWith(topSphere);
            CombineWith(cylinder);
            CombineWith(bottomSphere);
        }
    }

}