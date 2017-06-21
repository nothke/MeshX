using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class QuadTreeTerrain : MonoBehaviour
{
    public static QuadTreeTerrain e;
    void Awake() { e = this; }

    MyOwnQuadTree quadTree;

    public Material material;

    public int tileVertexSize = 32;

    void Start()
    {
        quadTree = GetComponent<MyOwnQuadTree>();

        StartCoroutine(WaitFrame());
    }


    IEnumerator WaitFrame()
    {
        yield return null;

        GenerateTerrain();
    }

    public void RegenerateTerrain()
    {
        DestroyTerrain();
        GenerateTerrain();
    }


    void DestroyTerrain()
    {
        treeNodes = quadTree.tree.root.GetAllChildren();

        // destroy all tiles that don't match bounds of new tiles
        for (int i = tilesList.Count - 1; i >= 0; i--)
        {
            bool exists = false;

            for (int tn = 0; tn < treeNodes.Count; tn++)
            {
                if (tilesList[i].bounds == treeNodes[tn].bounds
                    && !treeNodes[tn].IsSplit)
                {

                    exists = true;
                    break;
                }
            }

            if (!exists)
            {
                Destroy(tilesList[i].gameObject);
                tilesList.RemoveAt(i);
            }
        }

        //tilesList = tilesList.Where(i => i != null).ToList();
    }

    //List<GameObject> terrains = new List<GameObject>();

    public List<TerrainTile> tilesList = new List<TerrainTile>();

    public List<TreeNode> treeNodes;

    bool TileWithBoundsExists(TreeNode node)
    {
        for (int i = 0; i < tilesList.Count; i++)
        {
            if (tilesList[i].bounds == node.bounds)
                return true;
        }

        return false;
    }

    void GenerateTerrain()
    {
        StartCoroutine(GenerateCo());
    }

    IEnumerator GenerateCo()
    {
        if (treeNodes == null)
            treeNodes = quadTree.tree.root.GetAllChildren();
        yield return null;
        foreach (var treeNode in treeNodes)
        {
            

            if (treeNode.IsSplit) continue;

            if (TileWithBoundsExists(treeNode))
                continue;

            GameObject go = new GameObject();

            Vector3 pos = treeNode.bounds.min;
            pos.z = pos.y;
            pos.y = 0;

            go.transform.position = pos;

            TerrainTile tile = go.AddComponent<TerrainTile>();
            tile.bounds = treeNode.bounds;
            tile.tileVertices = tileVertexSize;
            tile.offset = treeNode.bounds.min;
            tile.width = quadTree.size / Mathf.Pow(2, treeNode.depth);

            tile.material = material;

            tilesList.Add(tile);

            if (treeNode.depth == quadTree.maxDepth)
                tile.gameObject.AddComponent<MeshCollider>();

        }
    }
}
