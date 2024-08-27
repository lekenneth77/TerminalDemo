using System.Collections.Generic;
using UnityEngine;

public class INode {
    public string name;
    public string path;
    public List<INode> children;
    public INode parent;

    public void Init(string n, string p) {
        name = n;
        path = p;
        children = new List<INode>();
    }

    public void AddChild(INode node) {
        children.Add(node);
    }

    public void SetParent(INode node) {
        parent = node;
    }
}

public class FileSystem : MonoBehaviour
{
    private INode _root;
    private INode _currentNode;

    void Awake()
    {
        CreateFilesys();
    }

    // Start is called before the first frame update
    void Start()
    {
        GameController.Get.Terminal.Init(_currentNode.path);
    }

    public void ReceiveCommand(string command, List<string> args) {
        switch (command) {
            case "cd":
            if (args.Count != 1) {
                GameController.Get.Terminal.DisplayError(ErrorMessageType.InvalidNumberOfArgs, command);
                break;
            }
            ChangeDirectory(args[0]);
            break;

            case "ls":
            if (args.Count != 0) {
                GameController.Get.Terminal.DisplayError(ErrorMessageType.InvalidNumberOfArgs, command);
                break;
            }
            List();
            break;

            case "pwd":
            if (args.Count != 0) {
                GameController.Get.Terminal.DisplayError(ErrorMessageType.InvalidNumberOfArgs, command);
                break;
            }
            PrintWorkingDirectory();
            break;

            case "clear":
            if (args.Count != 0) {
                GameController.Get.Terminal.DisplayError(ErrorMessageType.InvalidNumberOfArgs, command);
                break;
            }
            GameController.Get.Terminal.ClearTerminal();
            break;

            default:
            GameController.Get.Terminal.DisplayError(ErrorMessageType.UnrecognizedCommand, command);
            break;
        }
    }

    //changes directory to given path
    private void ChangeDirectory(string path) {
        if (path.Length == 0) {
            GameController.Get.Terminal.DisplayError(ErrorMessageType.PathNotFound, path);
            return;
        }

        bool isAbsolute = path[0] == '\\';
        INode startingNode = isAbsolute ? _root : _currentNode;
        if (isAbsolute) {
            path.Substring(1, path.Length - 1); //remove that first backslash
        }
        string[] dirs = path.Split('\\');
        INode result = CDHelper(startingNode, dirs[dirs.Length - 1], dirs, 0);
        if (result == null) {
            GameController.Get.Terminal.DisplayError(ErrorMessageType.PathNotFound, path);
        } else {
            _currentNode = result;
            GameController.Get.Terminal.SetCurrentPath(_currentNode.path);

        }
    }

    private INode CDHelper(INode curNode, string target, string[] dirs, int index) {
        if (curNode == null) {
            return null;
        }  else if (curNode.name == target) {
            return curNode;
        } else if (index >= dirs.Length) {
            return null;
        }

        if (dirs[index] == "..") {
            if (curNode == _root) {
                return null; //can't go back past the root buddy
            } else if (index == dirs.Length - 1) {
                return curNode.parent;
            } else {
                return CDHelper(curNode.parent, target, dirs, index + 1);
            }
        } else if (dirs[index] == ".") {
            if (index == dirs.Length - 1) {
                return curNode;
            } else {
                return CDHelper(curNode, target, dirs, index + 1);
            }
        }

        foreach (INode child in curNode.children) {
            if (child.name == dirs[index]) {
                return CDHelper(child, target, dirs, index + 1);
            }
        }
        return null;
    }

    //lists out the current directory's elements
    private void List() {
        List<INode> children = _currentNode.children;
        string resultingList = "";
        foreach (INode node in children) {
            resultingList += node.name + "\n";
        }
        GameController.Get.Terminal.DisplayMessage(resultingList);
    }

    //prints current working directory
    private void PrintWorkingDirectory() {
        GameController.Get.Terminal.DisplayMessage("\n" + _currentNode.path + "\n");
    }
    private void CreateFilesys() {
        _root = new INode();
        string path = "C:";
        _root.Init("C:", path);
        
        INode users = new INode();
        path += "\\Users";
        users.Init("Users", path);
        _root.AddChild(users);
        users.SetParent(_root);

        INode username = new INode();
        path += "\\baggu";
        username.Init("baggu", path);
        users.AddChild(username);
        username.SetParent(users);

        INode docs = new INode();
        path += "\\Documents";
        docs.Init("Documents", path);
        username.AddChild(docs);
        docs.SetParent(username);

        _currentNode = username;
    }



}


