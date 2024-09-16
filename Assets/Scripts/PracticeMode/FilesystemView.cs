using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class FilesystemView : MonoBehaviour
{
    [SerializeField] private RectTransform _container;
    [SerializeField] private Transform _inodeViewContainer;
    [SerializeField] private PracticeFilesys _filesys;
    [SerializeField] private INodeView _inodeViewFab;
    [SerializeField] private Button _regenerateButton;
    [SerializeField] private RectTransform _moveToTrans;
    [SerializeField] private CanvasGroup _terminalGrp;
    [Header("Sprites")]
    [SerializeField] private Sprite _folderSprite;
    [SerializeField] private Sprite _pythonSprite;
    [SerializeField] private Sprite _textFileSprite;
    [Header("Colors")]
    [SerializeField] private Color _curNodeColor;
    [SerializeField] private Color _tgtNodeColor;

    private Dictionary<INode, INodeView> _nodeViewDict = new Dictionary<INode, INodeView>();
    private Dictionary<INode, Dictionary<INode, GameObject>> _nodePairEdgeDict = new Dictionary<INode, Dictionary<INode, GameObject>>(); 
    private const float DFS_FADE_IN_DELAY = 0.3f;
    private const float DFS_FADE_IN_DURATION = 0.5f;

     
    private const float SCALE_DOWN = 0.6f;
    private const float ANIM_DURATION = 2f;

    // Start is called before the first frame update
    void Start()
    {
        _regenerateButton.onClick.AddListener(Regenerate);
        _filesys.CreateRandomFilesys(GameMode.EASYCD);
        DrawFilesys();
        var seq = DOTween.Sequence();
        seq.AppendInterval(2f);
        seq.AppendCallback(DFSAppear);
        seq.Play();
    }

    //temp function
    public void GameStart() {
        INode startingNode = Practice.Get.Filesys.GetRandomDir();
        INode targetNode = Practice.Get.Filesys.GetRandomDir();
        Practice.Get.Terminal.Init(startingNode.path);
        
        INodeView startingView = _nodeViewDict[startingNode];
        INodeView targetView = _nodeViewDict[targetNode];
        startingView.SetBackgroundColor(_curNodeColor);
        targetView.SetBackgroundColor(_tgtNodeColor);
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
        Sprite spr = _folderSprite;
        if (curNode.file) {
            string fileSuffix = curNode.name.Substring(curNode.name.Length - 3, 3);
            spr = fileSuffix == ".py" ? _pythonSprite : _textFileSprite;
        }
        curNodeView.Init(curNode.name, spr); 
        _nodeViewDict.Add(curNode, curNodeView);
        
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
            _nodePairEdgeDict[curNode] = new Dictionary<INode, GameObject>();
            for (int i = 0; i < childViews.Count; i++) {
                INodeView childView = childViews[i];
                GameObject line = curNodeView.DrawLine(childView.transform.position);
                _nodePairEdgeDict[curNode].Add(curNode.children[i], line);
            }
        }
        return curNodeView;
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

    private List<INode> GetDFSList(INode root) {
        List<INode> dfsList = new List<INode>();
        GetDFSListHelper(root, dfsList);
        return dfsList;    
    }

    private void GetDFSListHelper(INode curNode, List<INode> dfsList) {
        if (curNode == null) {
            return;
        }

        dfsList.Add(curNode);
        foreach(INode child in curNode.children) {
            GetDFSListHelper(child, dfsList);
        }
    }


    private void DFSAppear() {
        List<INode> dfs = GetDFSList(_filesys.GetRoot());
        var seq = DOTween.Sequence();
        float interval = 0f;
        foreach (INode node in dfs) {
            CanvasGroup nodeViewGrp = _nodeViewDict[node].GetComponent<CanvasGroup>();
            seq.Insert(interval, nodeViewGrp.DOFade(1, DFS_FADE_IN_DURATION));
            INode parent = node.parent;
            if (parent != null) {
                CanvasGroup edge = _nodePairEdgeDict[parent][node].GetComponent<CanvasGroup>();
                seq.Insert(interval, edge.DOFade(1, DFS_FADE_IN_DURATION));
            }
            interval += DFS_FADE_IN_DELAY;
        }

        seq.AppendInterval(3f);
        seq.Append(_container.DOScale(SCALE_DOWN, ANIM_DURATION));
        seq.Join(_container.DOAnchorPosY(_moveToTrans.anchoredPosition.y, ANIM_DURATION));
        seq.Join(_terminalGrp.DOFade(1, ANIM_DURATION));
        seq.OnComplete(GameStart);
        seq.Play();
    }
  
}
