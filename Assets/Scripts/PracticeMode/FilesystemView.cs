using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FilesystemView : MonoBehaviour
{
    [SerializeField] private RectTransform _container;
    [SerializeField] private Transform _inodeViewContainer;
    [SerializeField] private PracticeFilesys _filesys;
    [SerializeField] private INodeView _inodeViewFab;
    [SerializeField] private Button _regenerateButton;
    /*
    The cool algorithm:
    https://www.codeproject.com/Articles/5290521/Drawing-a-Tree

    1) Put each inode into their own row
    2) Starting from the bottom of the tree, put the parent in the same row as their first child
    3) Delete empty rows
    4) Recursively center the parent based on their children
    So basically, to represent "rows", we're going to use a list of stack of inodes

    */
    // Start is called before the first frame update
    void Start()
    {
        _regenerateButton.onClick.AddListener(Regenerate);
        _filesys.CreateRandomFilesys(GameMode.EASYCD);
        DrawFilesys();
    }

    private void Regenerate()
    {
        foreach (Transform child in _inodeViewContainer) {
            Destroy(child.gameObject);
        }
        _filesys.CreateRandomFilesys(GameMode.EASYCD);
        DrawFilesys();
    }

    public void DrawFilesys()
    {
        INode root = _filesys.GetRoot();
        Dictionary<INode, int> depthMap = GetDepthMap(root);
        int maxDepth = 0;
        foreach (int val in depthMap.Values) {
            maxDepth = Math.Max(val, maxDepth);
        }

        List<INode> leaves = GetLeaves(root);
        int numLeaves = leaves.Count;
        float widthSpacing = (_container.rect.width / maxDepth) * 0.75f;
        float heightSpacing = _container.rect.height / numLeaves;
        numLeavesPlaced = 0;
        PlaceNodesHelper(root, 0, numLeaves, widthSpacing, heightSpacing);
    }

    private int numLeavesPlaced = 0;

    private INodeView PlaceNodesHelper(INode curNode, int depth, int numLeaves, float widthSpacing, float heightSpacing) {
        if (curNode == null) {
            return null;
        }

        INodeView curNodeView = Instantiate(_inodeViewFab, _inodeViewContainer);
        curNodeView.Init(curNode.name); 
        float x = depth * widthSpacing;
        if (curNode.NumChildren() == 0) {
            //we're a leaf, place ourselves based on numLeaves and depth, used numLeavePlaced to handle the y value
            float y = numLeavesPlaced * heightSpacing;
            numLeavesPlaced++;
            curNodeView.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);
        } else {
            //we're a parent, place our y based on the average of our children
            float ySum = 0;
            List<INodeView> childViews = new List<INodeView>();
            foreach (INode child in curNode.children) {
                INodeView childView = PlaceNodesHelper(child, depth + 1, numLeaves, widthSpacing, heightSpacing);
                childViews.Add(childView);
                ySum += childView.GetComponent<RectTransform>().anchoredPosition.y;
            }
            float yAvg = ySum / curNode.NumChildren();
            curNodeView.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, yAvg);
            foreach (INodeView childView in childViews) {
                curNodeView.DrawLine(childView.transform.position);
            }
        }
        return curNodeView;
    }

    private List<INode> GetFlatTree(INode root) {
        Queue<INode> bfs = new Queue<INode>();
        List<INode> ls = new List<INode>(); 
        bfs.Enqueue(root);
        while (bfs.Count > 0) {
            INode node = bfs.Dequeue();
            ls.Add(node);
            foreach (INode child in node.children) {
                bfs.Enqueue(child);
            }
        }
        return ls;
    }

    private Dictionary<INode, int> GetDepthMap(INode root) {
        Dictionary<INode, int> depthMap = new Dictionary<INode, int>();
        Queue<INode> bfs = new Queue<INode>();
        bfs.Enqueue(root);
        int curDepth = 0;
        while (bfs.Count > 0) {
            int depthSize = bfs.Count;
            while (depthSize > 0) {
                INode node = bfs.Dequeue();
                depthMap.Add(node, curDepth);
                foreach (INode child in node.children) {
                    bfs.Enqueue(child);
                }
                depthSize--;
            }
            curDepth++;
        }
        return depthMap;
    }

    private List<INode> GetLeaves(INode root) {
        List<INode> leaves = new List<INode>();
        GetLeavesHelper(root, leaves);
        return leaves;
    }

    private void GetLeavesHelper(INode curNode, List<INode> leaves) {
        if (curNode == null) {
            return;
        }

        if (curNode.NumChildren() == 0) {
            leaves.Add(curNode);
        } else {
            foreach (INode child in curNode.children) {
                GetLeavesHelper(child, leaves);
            }
        }
    }

    private int GetMaxDepth(INode root) {
        return MaxDepthHelper(root, 0);
    }

    private int MaxDepthHelper(INode curNode, int depth) {
        if (curNode == null) {
            return depth;
        }

        int maxDepth = depth;
        foreach (INode child in curNode.children) {
            maxDepth = Math.Max(maxDepth, MaxDepthHelper(child, depth + 1));
        }
        return maxDepth;
    }

    private int GetMaxWidth(INode root) {
        if (root == null) {
            return 0;
        }

        Queue<INode> nodeQ = new Queue<INode>();
        nodeQ.Enqueue(root);
        int maxWidth = 0;
        while (nodeQ.Count > 0) {
            int curWidth = nodeQ.Count; 
            maxWidth = Math.Max(curWidth, maxWidth);

            INode curNode = nodeQ.Dequeue();
            foreach (INode child in curNode.children) {
                nodeQ.Enqueue(child);
            }
        }
        return maxWidth;
    }

  
}
