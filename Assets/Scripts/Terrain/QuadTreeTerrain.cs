using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadTreeTerrain : MonoBehaviour
{

    MyOwnQuadTree quadTree;

    void Start()
    {
        quadTree = GetComponent<MyOwnQuadTree>();

        StartCoroutine(WaitFrame());
    }

    IEnumerator WaitFrame()
    {
        yield return null;

        List<TreeNode> treeNodes = quadTree.tree.root.GetAllChildren();

        foreach (var treeNode in treeNodes)
        {
            if (treeNode.IsSplit) continue;

            GameObject go = new GameObject();

            Vector3 pos = treeNode.bounds.min;
            pos.z = pos.y;
            pos.y = 0;

            go.transform.position = pos;

            TerrainTile tile = go.AddComponent<TerrainTile>();
            tile.offset = treeNode.bounds.min;
            tile.width = quadTree.size / Mathf.Pow(2, treeNode.depth);
        }
    }
}
