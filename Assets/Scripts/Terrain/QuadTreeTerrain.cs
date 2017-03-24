using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadTreeTerrain : MonoBehaviour
{
    public static QuadTreeTerrain e;
    void Awake() { e = this; }

    MyOwnQuadTree quadTree;

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
        for (int i = 0; i < terrains.Count; i++)
        {
            Destroy(terrains[i]);
        }

        terrains.Clear();
    }

    List<GameObject> terrains = new List<GameObject>();

    void GenerateTerrain()
    {
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

            terrains.Add(tile.gameObject);
        }
    }
}
