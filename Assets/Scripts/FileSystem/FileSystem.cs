using System;
using System.Collections.Generic;
using UnityEngine;

public class INode {
    public string name;
    public string path;
    public List<INode> children;
    public INode parent;
    public bool file = false;

    public void Init(string n, bool isFile = false) {
        name = n;
        path = "C:";
        file = isFile;
        children = new List<INode>();
    }

    public void AddChild(INode node) {
        children.Add(node);
        node.parent = this;
        node.path = path + "\\" + node.name;
    }

}

public class FileSystem : MonoBehaviour
{
    private INode _root;
    private INode _currentNode;
    private bool _guided;
    public event Action CorrectCMDReceived;
    public event Action IncorrectCMDReceived;
    private TerminalTextHandler terminal;

    void Awake()
    {
        CreateFilesys();
    }

    // Start is called before the first frame update
    void Start()
    {
        terminal = GameController.Get.Terminal;
        terminal.Init(_currentNode.path);
    }

    
    public void SetGuidedMode(bool isGuided) {
        _guided = isGuided;
    }

    public void ReceiveCommand(string command, List<string> args) {
        switch (command) {
            case "cd":
            if (args.Count != 1) {
                terminal.DisplayError(ErrorMessageType.InvalidNumberOfArgs, command);
                break;
            }
            ChangeDirectory(args[0]);
            break;

            case "ls":
            if (args.Count != 0) {
                terminal.DisplayError(ErrorMessageType.InvalidNumberOfArgs, command);
                break;
            }
            List();
            break;

            case "pwd":
            if (args.Count != 0) {
                terminal.DisplayError(ErrorMessageType.InvalidNumberOfArgs, command);
                break;
            }
            PrintWorkingDirectory();
            break;

            case "clear":
            if (args.Count != 0) {
                terminal.DisplayError(ErrorMessageType.InvalidNumberOfArgs, command);
                if (_guided) {IncorrectCMDReceived?.Invoke();}
                break;
            }

            if (_guided) {
                CorrectCMDReceived?.Invoke();
            }
            terminal.ClearTerminal();
            break;

            case "python3":
            //....
            break;

            default:
            if (_guided) {
                if (GameController.Get.Dialogue.CheckIfCorrect(CMDType.UNKNOWN, command)) {
                    CorrectCMDReceived?.Invoke();
                } else {
                    IncorrectCMDReceived?.Invoke();
                    return;
                }
            }
            terminal.DisplayError(ErrorMessageType.UnrecognizedCommand, command);
            break;
        }
    }

    //changes directory to given path
    private void ChangeDirectory(string path) {
        if (path.Length == 0) {
            terminal.DisplayError(ErrorMessageType.PathNotFound, path);
            return;
        }

        bool isAbsolute = path[0] == '\\';
        INode startingNode = isAbsolute ? _root : _currentNode;
        if (isAbsolute) {
            path = path.Substring(1, path.Length - 1); //remove that first backslash
        }
        string[] dirs = path.Split('\\');
        INode result = CDHelper(startingNode, dirs[dirs.Length - 1], dirs, 0);
        
        if (result == null) {
            terminal.DisplayError(ErrorMessageType.PathNotFound, path);
        } else if (result.file) {
            terminal.DisplayError(ErrorMessageType.FileNotDirectory, path);
        } else {
            if (_guided) {
                if (GameController.Get.Dialogue.CheckIfCorrect(CMDType.CD, result.name)) {
                    CorrectCMDReceived?.Invoke();
                } else {
                    IncorrectCMDReceived?.Invoke(); return;
                }
            }

            _currentNode = result;
            terminal.SetCurrentPath(_currentNode.path);
            return;
        }

        if (_guided) { IncorrectCMDReceived?.Invoke();}
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

    public void ForceCD(string absolutePath) {
        absolutePath = absolutePath.Substring(1, absolutePath.Length - 1); //remove that first backslash
        string[] dirs = absolutePath.Split('\\');
        INode result = CDHelper(_root, dirs[dirs.Length - 1], dirs, 0);
        _currentNode = result;
        GameController.Get.Terminal.SetCurrentPath(_currentNode.path);
    }

    //lists out the current directory's elements
    private void List() {
        if (_guided) {
            if (GameController.Get.Dialogue.CheckIfCorrect(CMDType.LS, "")) {
                CorrectCMDReceived?.Invoke();
            } else {
                IncorrectCMDReceived?.Invoke();
                return;
            }  
        }

        List<INode> children = _currentNode.children;
        string resultingList = "";
        foreach (INode node in children) {
            resultingList += node.name + "\n";
        }
        terminal.DisplayMessage(resultingList);
    }

    //prints current working directory
    private void PrintWorkingDirectory() {
        if (_guided) {
            if (GameController.Get.Dialogue.CheckIfCorrect(CMDType.PWD, "")) {
                CorrectCMDReceived?.Invoke();
            } else {
                IncorrectCMDReceived?.Invoke();
                return;
            }  
        }

        terminal.DisplayMessage("\n" + _currentNode.path + "\n");
    }
    private void CreateFilesys() {
        _root = new INode();
        _root.Init("C:");
        
        INode users = new INode();
        users.Init("Users");
        _root.AddChild(users);

        INode username = new INode();
        username.Init("baggu");
        users.AddChild(username);

        INode docs = new INode();
        docs.Init("Documents");
        username.AddChild(docs);

        INode termDemo = new INode();
        termDemo.Init("TerminalDemo");
        docs.AddChild(termDemo);

        INode pyScripts = new INode();
        pyScripts.Init("PythonScripts");
        docs.AddChild(pyScripts);

        INode moneyPy = new INode();
        moneyPy.Init("money.py");
        pyScripts.AddChild(moneyPy);

        INode reporttxt = new INode();
        reporttxt.Init("report.txt");
        pyScripts.AddChild(reporttxt);

        INode daystxt = new INode();
        daystxt.Init("days.txt");
        pyScripts.AddChild(daystxt);

        INode dls = new INode();
        dls.Init("Downloads");
        username.AddChild(dls);

        INode mickey = new INode();
        mickey.Init("test.png", true);
        dls.AddChild(mickey);

        INode drip = new INode();
        drip.Init("testdrip.png", true);
        dls.AddChild(drip);

        _currentNode = username;
    }



}


