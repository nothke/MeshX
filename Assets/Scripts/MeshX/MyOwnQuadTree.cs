using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patch
{

}

public class Tree
{
    public TreeNode root;

    Rect bounds;

    public Tree(Rect bounds)
    {
        this.bounds = bounds;
        root = new TreeNode(bounds, 0, null);
    }

    public void Debug()
    {
        root.Debug(recursive: true);
    }

    public void GenerateDepthForPoint(Vector3 point, int maxDepth = 4)
    {
        root = null;

        Vector2 p = new Vector2(point.x, point.z);

        root = new TreeNode(bounds, 0, null);

        root.Within(p, maxDepth);
    }

    public void SplitToPoints(Vector3[] points, int maxDepth = 4)
    {
        root = null;
        root = new TreeNode(bounds, 0, null);

        for (int i = 0; i < points.Length; i++)
        {
            Vector2 p = new Vector2(points[i].x, points[i].z);

            root.Within(p, maxDepth);
        }
    }

    public void Balance()
    {
        root.Balance();
    }

    public void BalanceT()
    {
        List<TreeNode> nodeList = root.GetAllChildren();

        //UnityEngine.Debug.Log(nodeList.Count);

        Queue<TreeNode> L = new Queue<TreeNode>();

        // . insert all leaves of T into a linear list L
        for (int i = 0; i < nodeList.Count; i++)
        {
            if (!nodeList[i].IsSplit)
                L.Enqueue(nodeList[i]);
        }

        // while L is not empty
        while (L.Count != 0)
        {
            // do remove a leave µ from L
            TreeNode node = L.Dequeue();
            //L.RemoveAt(0);

            if (node == null) break;

            // if σ(µ) has to be split

            // then make µ an internal node with 4 children;

            // if µ stores a point, store it in the correct leave

            // insert the 4 new leaves into L

            //  check if σ(µ) had neighbors that now need to be split,
            // and if so, insert them into L
            TreeNode W = node.W;

            if (W != null && node.depth - W.depth > 1)
            {
                W.Split();
                L.Enqueue(W.children[0]);
                L.Enqueue(W.children[1]);
                L.Enqueue(W.children[2]);
                L.Enqueue(W.children[3]);
            }

            TreeNode E = node.E;

            if (E != null && node.depth - E.depth > 1)
            {
                E.Split();
                L.Enqueue(E.children[0]);
                L.Enqueue(E.children[1]);
                L.Enqueue(E.children[2]);
                L.Enqueue(E.children[3]);
            }

            TreeNode N = node.N;

            if (N != null && node.depth - N.depth > 1)
            {
                N.Split();
                L.Enqueue(N.children[0]);
                L.Enqueue(N.children[1]);
                L.Enqueue(N.children[2]);
                L.Enqueue(N.children[3]);
            }

            TreeNode S = node.S;

            if (S != null && node.depth - S.depth > 1)
            {
                S.Split();
                L.Enqueue(S.children[0]);
                L.Enqueue(S.children[1]);
                L.Enqueue(S.children[2]);
                L.Enqueue(S.children[3]);
            }
        }
    }
}

public static class Extension
{
    public static bool IsNorth(this Quadrant q) { return q == Quadrant.NE || q == Quadrant.NW; }
    public static bool IsSouth(this Quadrant q) { return q == Quadrant.SE || q == Quadrant.SW; }
    public static bool IsEast(this Quadrant q) { return q == Quadrant.NE || q == Quadrant.SE; }
    public static bool IsWest(this Quadrant q) { return q == Quadrant.NW || q == Quadrant.SW; }
}

public enum Side { North, West, East, South };
public enum Quadrant { NE, NW, SE, SW };

public class TreeNode
{
    public Rect bounds;

    public int depth;

    public TreeNode parent;
    public TreeNode[] children;

    public TreeNode w;
    public TreeNode e;
    public TreeNode n;
    public TreeNode s;

    public TreeNode NW { get { return children[0]; } }
    public TreeNode NE { get { return children[1]; } }
    public TreeNode SW { get { return children[2]; } }
    public TreeNode SE { get { return children[3]; } }

    #region Neighbours

    public TreeNode N
    {
        get
        {
            if (parent == null)
                return null;

            if (parent.SW == this)
            {
                if (parent.children[0].IsSplit)
                    return parent.children[0].children[2];

                return parent.children[0];
            }


            if (parent.SE == this)
                return parent.children[1];

            return parent.N;
        }
    }



    public static bool IsNorth(Quadrant q) { return q == Quadrant.NE || q == Quadrant.NW; }
    public static bool IsSouth(Quadrant q) { return q == Quadrant.SE || q == Quadrant.SW; }
    public static bool IsEast(Quadrant q) { return q == Quadrant.NE || q == Quadrant.SE; }
    public static bool IsWest(Quadrant q) { return q == Quadrant.NW || q == Quadrant.SW; }

    public Quadrant Quadrant;

    public TreeNode GetNeighbour(Side side)
    {
        if (parent == null)
            return null;

        TreeNode node = this;
        Stack<Quadrant> climbStack = new Stack<Quadrant>();

        if (side == Side.North)
        {
        Climb:
            if (!node.Quadrant.IsSouth())
            {
                climbStack.Push(node.Quadrant);
                node = node.parent;

                if (node == null) return null;

                goto Climb;
            }
            else
            {
                if (node.parent == null) return null;

                // Get sibiling
                if (node.Quadrant.IsWest())
                    node = node.parent.NW;
                else node = node.parent.NE;

                Descend:

                if (node.depth == depth)
                    return node;

                if (node.IsSplit)
                {
                    Quadrant q = climbStack.Pop();
                    if (q.IsWest())
                        node = node.SW;
                    else
                        node = node.SE;

                    goto Descend;
                }
                else return node;
            }
        }

        else if (side == Side.South)
        {
        Climb:
            if (!node.Quadrant.IsNorth())
            {
                climbStack.Push(node.Quadrant);
                node = node.parent;

                if (node == null) return null;

                goto Climb;
            }
            else
            {
                if (node.parent == null) return null;

                // Get sibiling
                if (node.Quadrant.IsWest())
                    node = node.parent.SW;
                else node = node.parent.SE;

                Descend:

                if (node.depth == depth)
                    return node;

                if (node.IsSplit)
                {
                    Quadrant q = climbStack.Pop();
                    if (q.IsWest())
                        node = node.NW;
                    else
                        node = node.NE;

                    goto Descend;
                }
                else return node;
            }
        }

        else if (side == Side.West)
        {
        Climb:
            if (!node.Quadrant.IsEast())
            {
                climbStack.Push(node.Quadrant);
                node = node.parent;

                if (node == null) return null;

                goto Climb;
            }
            else
            {
                if (node.parent == null) return null;

                // Get sibiling
                if (node.Quadrant.IsNorth())
                    node = node.parent.NW;
                else node = node.parent.SW;

                Descend:

                if (node.depth == depth)
                    return node;

                if (node.IsSplit)
                {
                    Quadrant q = climbStack.Pop();
                    if (q.IsNorth())
                        node = node.NE;
                    else
                        node = node.SE;

                    goto Descend;
                }
                else return node;
            }
        }

        else if (side == Side.East)
        {
        Climb:
            if (!node.Quadrant.IsWest())
            {
                climbStack.Push(node.Quadrant);
                node = node.parent;

                if (node == null) return null;

                goto Climb;
            }
            else
            {
                if (node.parent == null) return null;

                // Get sibiling
                if (node.Quadrant.IsNorth())
                    node = node.parent.NE;
                else node = node.parent.SE;

                Descend:

                if (node.depth == depth)
                    return node;

                if (node.IsSplit)
                {
                    Quadrant q = climbStack.Pop();
                    if (q.IsNorth())
                        node = node.NW;
                    else
                        node = node.SW;

                    goto Descend;
                }
                else return node;
            }
        }

        return null;
    }

    public TreeNode S
    {
        get
        {
            if (parent == null)
                return null;

            if (parent.children[0] == this)
                return parent.children[2];

            if (parent.children[1] == this)
                return parent.children[3];

            return parent.S;
        }
    }

    public TreeNode W
    {
        get
        {
            if (parent == null)
                return null;

            if (parent.children[1] == this)
                return parent.children[0];

            if (parent.children[3] == this)
                return parent.children[2];

            return parent.W;
        }
    }

    public TreeNode E
    {
        get
        {
            if (parent == null)
                return null;

            if (parent.children[0] == this)
                return parent.children[1];

            if (parent.children[2] == this)
                return parent.children[3];

            return parent.E;
        }
    }

    #endregion

    public TreeNode(Rect bounds, int depth, TreeNode parent)
    {
        this.bounds = bounds;
        this.depth = depth;
        children = new TreeNode[4];
        this.parent = parent;
    }

    public bool IsSplit { get { return children[0] != null; } }

    public void Split()
    {
        if (IsSplit) return;

        // Generate children
        Vector2 size = bounds.size / 2;
        Rect NW = new Rect(bounds.position + new Vector2(0, size.y), size);
        Rect NE = new Rect(bounds.position + new Vector2(size.x, size.y), size);
        Rect SW = new Rect(bounds.position + new Vector2(0, 0), size);
        Rect SE = new Rect(bounds.position + new Vector2(size.x, 0), size);

        children[0] = new TreeNode(NW, depth + 1, this);
        children[0].Quadrant = Quadrant.NW;
        children[1] = new TreeNode(NE, depth + 1, this);
        children[1].Quadrant = Quadrant.NE;
        children[2] = new TreeNode(SW, depth + 1, this);
        children[2].Quadrant = Quadrant.SW;
        children[3] = new TreeNode(SE, depth + 1, this);
        children[3].Quadrant = Quadrant.SE;

        // Assign neighbours

        /*
        children[0].s = children[2];
        children[1].s = children[3];

        children[0].e = children[1];
        children[2].e = children[3];

        children[1].w = children[0];
        children[3].w = children[2];

        children[2].n = children[0];
        children[3].n = children[1];


        children[0].w = w;
        children[2].w = w;

        children[0].n = n;
        children[1].n = n;

        children[2].s = s;
        children[3].s = s;

        children[1].e = e;
        children[2].e = e;*/
    }

    public void Collapse()
    {
        // Destroy children
        for (int i = 0; i < children.Length; i++)
        {
            children[i] = null;
        }
    }

    bool isTarget;



    public static TreeNode targetNode;

    public void Within(Vector2 point, int maxDepth = 4)
    {
        if (depth == maxDepth)
        {
            if (bounds.Contains(point))
            {
                //BalanceNeighbours();
                isTarget = true;
                targetNode = this;
            }

            return;
        }

        if (!bounds.Contains(point)) return;

        Split();

        for (int i = 0; i < children.Length; i++)
            children[i].Within(point, maxDepth);
    }

    public void BalanceNeighbours()
    {
        s = S;
        e = E;
        n = N;
        w = W;

        BalanceNeighbour(ref s);
        BalanceNeighbour(ref e);
        BalanceNeighbour(ref n);
        BalanceNeighbour(ref w);
    }

    void BalanceNeighbour(ref TreeNode node)
    {
        if (node == null) return;

        if (depth - node.depth > 1)
        {
            //UnityEngine.Debug.Log("GAAI");
            node.Split();
        }

    }

    public void Debug(bool recursive = false)
    {
        Gizmos.color = Color.HSVToRGB(depth / 10.0f, 1, 1);

        Gizmos.DrawLine(new Vector3(bounds.x, 0, bounds.y), new Vector3(bounds.x, 0, bounds.y + bounds.height));
        Gizmos.DrawLine(new Vector3(bounds.x, 0, bounds.y), new Vector3(bounds.x + bounds.width, 0, bounds.y));
        Gizmos.DrawLine(new Vector3(bounds.x + bounds.width, 0, bounds.y), new Vector3(bounds.x + bounds.width, 0, bounds.y + bounds.height));
        Gizmos.DrawLine(new Vector3(bounds.x, 0, bounds.y + bounds.height), new Vector3(bounds.x + bounds.width, 0, bounds.y + bounds.height));

        if (!recursive) return;

        for (int i = 0; i < children.Length; i++)
        {
            if (children[i] != null)
                children[i].Debug(true);
        }
    }

    public void DebugNeighbours()
    {
        n = GetNeighbour(Side.North);
        e = GetNeighbour(Side.East);
        w = GetNeighbour(Side.West);
        s = GetNeighbour(Side.South);

        //Gizmos.color = Color.blue;
        Gizmos.color = new Color(0.3f, 0.5f, 1, 0.5f);


        if (w != null)
            Gizmos.DrawCube(new Vector3(w.bounds.center.x, 0, w.bounds.center.y), new Vector3(w.bounds.size.x, 0, w.bounds.size.y));


        if (e != null)
            Gizmos.DrawCube(new Vector3(e.bounds.center.x, 0, e.bounds.center.y), new Vector3(e.bounds.size.x, 0, e.bounds.size.y));


        if (n != null)
            Gizmos.DrawCube(new Vector3(n.bounds.center.x, 0, n.bounds.center.y), new Vector3(n.bounds.size.x, 0, n.bounds.size.y));


        if (s != null)
            Gizmos.DrawCube(new Vector3(s.bounds.center.x, 0, s.bounds.center.y), new Vector3(s.bounds.size.x, 0, s.bounds.size.y));

    }

    /// <summary>
    /// Makes sure neighbours are not more than 1 depth difference
    /// </summary>
    public void Balance(int maxDepth = 4)
    {
        if (!IsSplit) return;

        if (depth == maxDepth) return;

        bool shouldSplitChildren = false;

        for (int i = 0; i < children.Length; i++)
        {
            if (children[i].IsSplit)
            {
                for (int j = 0; j < children[i].children.Length; j++)
                {
                    if (children[i].children[j].IsSplit)
                        shouldSplitChildren = true;
                }
            }
        }

        for (int i = 0; i < children.Length; i++)
        {
            if (shouldSplitChildren)
                children[i].Split();

            children[i].Balance();
        }
    }

    public List<TreeNode> GetAllChildren()
    {
        List<TreeNode> list = new List<TreeNode>();

        list.Add(this);

        if (!IsSplit) return list;

        foreach (var child in children)
        {
            list.AddRange(child.GetAllChildren());
        }

        return list;
    }
}

public class MyOwnQuadTree : MonoBehaviour
{
    Tree tree;

    int maxDepth = 3;

    public float size = 100;

    public Transform trackThis;

    void Start()
    {
        tree = new Tree(new Rect(-size * 0.5f, -size * 0.5f, size, size));

        //GetPos();

        //tree.BalanceT();

        //SimpleTest();
        //tree.BalanceT();
    }

    void SimpleTest()
    {
        tree.root.Split();
        tree.root.children[0].Split();
        tree.root.children[0].children[2].Split();
    }

    void Update()
    {
        GetPos();

        tree.BalanceT();
    }

    void GetPos()
    {
        if (trackThis.childCount == 0)
            tree.GenerateDepthForPoint(trackThis.position, 6);
        else
        {
            Vector3[] childPs = new Vector3[trackThis.childCount];

            for (int i = 0; i < childPs.Length; i++)
            {
                childPs[i] = trackThis.GetChild(i).position;
            }

            tree.SplitToPoints(childPs, 5);
        }
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        tree.Debug();

        TreeNode.targetNode.DebugNeighbours();
    }
}
