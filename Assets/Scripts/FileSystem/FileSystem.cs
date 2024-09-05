using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class INode {
    public string name;
    public string path;
    public List<INode> children;
    public INode parent;
    public bool file = false;
    public Trie trie;

    public void Init(string n, bool isFile = false) {
        name = n;
        path = "C:";
        file = isFile;
        children = new List<INode>();
        trie = new Trie();
    }

    public void AddChild(INode node) {
        children.Add(node);
        node.parent = this;
        node.path = path + "\\" + node.name;
        trie.AddWord(node.name);
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

            if (_guided && GameController.Get.Dialogue.CheckIfCorrect(CMDType.CLEAR, "")) {
                CorrectCMDReceived?.Invoke();
            }
            terminal.ClearTerminal();
            break;

            case "python3":
            if (args.Count == 0) {
                terminal.DisplayError(ErrorMessageType.InvalidNumberOfArgs, command);
                if (_guided) {IncorrectCMDReceived?.Invoke();}
                break;
            }
            Python(args);
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

       INode result = GetINode(path);
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
            terminal.SetCurrentPath(_currentNode.path, true);
            return;
        }

        if (_guided) { IncorrectCMDReceived?.Invoke();}
    }

    //DOESN'T ACTUALLY CD , CHOOSE A BETTER NAME MAN
    private INode CDHelper(INode startingNode, INode curNode, string target, string[] dirs, int index) {
        if (curNode == null) {
            return null;
        }  else if (curNode.name == target && curNode != startingNode) {
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
                return CDHelper(startingNode, curNode.parent, target, dirs, index + 1);
            }
        } else if (dirs[index] == ".") {
            if (index == dirs.Length - 1) {
                return curNode;
            } else {
                return CDHelper(startingNode, curNode, target, dirs, index + 1);
            }
        }

        foreach (INode child in curNode.children) {
            if (child.name == dirs[index]) {
                return CDHelper(startingNode, child, target, dirs, index + 1);
            }
        }
        return null;
    }

    public void ForceCD(string absolutePath) {
        absolutePath = absolutePath.Substring(1, absolutePath.Length - 1); //remove that first backslash
        string[] dirs = absolutePath.Split('\\');
        INode result = CDHelper(_root, _root, dirs[dirs.Length - 1], dirs, 0);
        _currentNode = result;
        GameController.Get.Terminal.SetCurrentPath(_currentNode.path, false);
    }

    private void Python(List<string> args) {
        string redirector = "";
        //look for redirector, only worry about one rn LOL, i really should've did this like OS
        if (args.Count > 3) {
            //TODO implement a REAL command struct LOL
            if (_guided) {IncorrectCMDReceived?.Invoke();}
            return;
        } else if (args.Count == 2) {
            terminal.DisplayError(ErrorMessageType.InvalidArguments, args[2]);
            if (_guided) {IncorrectCMDReceived?.Invoke();}
            return;
        } else if (args.Count == 3) {
            if (args[1] == "<" || args[1] == ">") {
                redirector = args[1];
            } else {
                terminal.DisplayError(ErrorMessageType.InvalidArguments, args[1]);
            }
        } 
        
        string path = args[0];
        INode result = GetINode(path);
        if (result == null) { 
            terminal.DisplayError(ErrorMessageType.PathNotFound, path);
        } else if (!result.file) {
            terminal.DisplayError(ErrorMessageType.DirectoryNotFile, result.name);
        } else if (result.name.Substring(result.name.Length - 3, 3) != ".py"){
            terminal.DisplayError(ErrorMessageType.NotValidFile, result.name);
        } else {
            if (redirector.Length > 0) {
                //check redirect path
                string redirectPath = args[2];
                INode getter = GetINode(redirectPath);
                if (getter == null) {
                    terminal.DisplayError(ErrorMessageType.PathNotFound, redirectPath); if (_guided) {IncorrectCMDReceived?.Invoke();}
                    return;
                } else if (!getter.file) {
                    terminal.DisplayError(ErrorMessageType.DirectoryNotFile, getter.name); if (_guided) {IncorrectCMDReceived?.Invoke();}
                    return;
                } else if (getter.name.Substring(getter.name.Length - 4, 4) != ".txt") {
                    terminal.DisplayError(ErrorMessageType.NotValidFile, getter.name); if (_guided) {IncorrectCMDReceived?.Invoke();}
                    return;
                } else {
                    //it's a valid path
                    redirector += getter.name;
                }
            }

            if (_guided && GameController.Get.Dialogue.CheckIfCorrect(CMDType.PYTHON3, result.name, redirector)) {
                CorrectCMDReceived?.Invoke();
            }
            //for now do nothing lol if it's not guided
        }
        if (_guided) {IncorrectCMDReceived?.Invoke();}
    }

    private INode GetINode(string path) {
        bool isAbsolute = path[0] == '\\';
        INode startingNode = isAbsolute ? _root : _currentNode;
        if (isAbsolute) {
            path = path.Substring(1, path.Length - 1); //remove that first backslash
        }
        string[] dirs = path.Split('\\');
        INode result = CDHelper(startingNode, startingNode, dirs[dirs.Length - 1], dirs, 0);
        return result;
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

    public void AutocompletePath(string path) {
        bool isAbsolute = path[0] == '\\';
        INode startingNode = isAbsolute ? _root : _currentNode;
        if (isAbsolute) {
            path = path.Substring(1, path.Length - 1); //remove that first backslash
        }
        string[] dirs = path.Split("\\");
        string[] poppedDirs = dirs.Take(dirs.Length - 1).ToArray();
        INode result = dirs.Length > 1 ? CDHelper(startingNode, startingNode, dirs[dirs.Length - 2], poppedDirs, 0) : startingNode; //i feel like this might cause a bug in the future
        if (result == null) {
            //do nothing
            return;
        }

        Trie curTrie = result.trie;
        List<string> ls = curTrie.GetWordsWithPrefix(dirs[dirs.Length - 1]);
        if (ls.Count == 0) {
            //do nothing
        } else if (ls.Count == 1) {
            //auto complete!
            //add n - 1 dirs to the list and then append the ls[0] and make the string from that bro, big brain
            List<string> autocompletedirs = new List<string>();
            for (int i = 0; i < dirs.Length - 1; i++) {
                autocompletedirs.Add(dirs[i]);
            }
            autocompletedirs.Add(ls[0]);
            string autocompletedpath = "";
            foreach (string s in autocompletedirs) {
                autocompletedpath += s + '\\';
            }
            autocompletedpath = autocompletedpath.Substring(0, autocompletedpath.Length - 1); //AHAHAHAHA FENCE POST
            terminal.TextTabAutoComplete(autocompletedpath);
        } else {
            //we have many results, print them all out, then go to a new line with the same command!
            terminal.ShowPossibleAutoComplete(ls);
        }
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
        moneyPy.Init("money.py", true);
        pyScripts.AddChild(moneyPy);

        INode reporttxt = new INode();
        reporttxt.Init("report.txt", true);
        pyScripts.AddChild(reporttxt);

        INode daystxt = new INode();
        daystxt.Init("days.txt", true);
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


