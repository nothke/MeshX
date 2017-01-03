using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeshXtensions
{
    public static class MeshX_QuadSphere
    {
        public enum Side { Top, Bottom, Left, Right, Front, Back }

        public static Geom GeomQuadSphere(int divisions, float radius = 1)
        {
            // The top
            Geom geom = MeshX.GeomGrid(divisions, divisions, 1, 1, true);
            geom.Translate(Vector3.up * divisions * 0.5f);

            Geom bottom = geom.Clone();
            bottom.Rotate(180, Vector3.right);
            geom.CombineWith(bottom);

            Geom leftRight = geom.Clone();
            leftRight.Rotate(90, Vector3.right);

            Geom frontBack = leftRight.Clone();
            frontBack.Rotate(90, Vector3.up);

            geom.CombineWith(leftRight);
            geom.CombineWith(frontBack);

            geom.normals = new List<Vector3>();

            for (int i = 0; i < geom.vertices.Count; i++)
            {
                geom.vertices[i] = geom.vertices[i].normalized * radius;
                geom.normals.Add(geom.vertices[i].normalized);
            }

            return geom;
        }


        public static Geom GeomQuadSphereSide(Side side, int divisions = 10, float radius = 1)
        {
            Geom geom = MeshX.GeomGrid(divisions, divisions, 1, 1, true);
            geom.Translate(Vector3.up * divisions * 0.5f);

            switch (side)
            {
                case Side.Top:
                    break;
                case Side.Bottom:
                    geom.Rotate(180, Vector3.right);
                    break;
                case Side.Left:
                    geom.Rotate(90, Vector3.forward);
                    break;
                case Side.Right:
                    geom.Rotate(-90, Vector3.forward);
                    break;
                case Side.Front:
                    geom.Rotate(90, Vector3.right);
                    break;
                case Side.Back:
                    geom.Rotate(-90, Vector3.right);
                    break;
            }

            for (int i = 0; i < geom.vertices.Count; i++)
            {
                geom.vertices[i] = geom.vertices[i].normalized * radius;
                //geom.normals.Add(geom.vertices[i].normalized);
            }

            return geom;
        }

        public static Geom GeomQuadSpherePatch(Side side, int step, float posX, float posY, int divisions = 10, float radius = 1)
        {
            if (step < 0) return null;

            float separation = 1.0f / (step + 1.0f);

            Geom geom = MeshX.GeomGrid(divisions, divisions, separation, separation, true);
            geom.Translate(new Vector3(posX, divisions * 0.5f, posY));

            switch (side)
            {
                case Side.Top:
                    break;
                case Side.Bottom:
                    geom.Rotate(180, Vector3.right);
                    break;
                case Side.Left:
                    geom.Rotate(90, Vector3.forward);
                    break;
                case Side.Right:
                    geom.Rotate(-90, Vector3.forward);
                    break;
                case Side.Front:
                    geom.Rotate(90, Vector3.right);
                    break;
                case Side.Back:
                    geom.Rotate(-90, Vector3.right);
                    break;
            }

            for (int i = 0; i < geom.vertices.Count; i++)
            {
                geom.vertices[i] = geom.vertices[i].normalized * radius;
                //geom.normals.Add(geom.vertices[i].normalized);
            }

            return geom;
        }
    }
}