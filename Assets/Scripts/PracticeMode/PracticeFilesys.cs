using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class PracticeFilesys : MonoBehaviour
{
    private INode _root;
    private const int MAX_INODES_IN_DIR = 2;
    private List<INode> dirs;
    private List<INode> pythonScripts;
    private List<INode> textFiles;

    private INode _currentNode;
    private PracticeTerminal _terminal;
    private HashSet<int> usedDirsI = new HashSet<int>();


    void Awake()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        _terminal = Practice.Get.Terminal;
    }

    public INode GetRoot() {
        return _root;
    }

    public INode GetRandomDir() {
        int randomDirI = Random.Range(0, dirs.Count);
        while (!usedDirsI.Add(randomDirI)) {
            randomDirI = Random.Range(0, dirs.Count);
        }
        return dirs[randomDirI];
    }

    /*
    max vals:
    user: 3
    directories: 16
    total files: 10
    */

    public void CreateRandomFilesys(GameMode mode) {
        switch(mode) {
            case GameMode.EASYCD:
            GenerateFS(3, 12, 2, 4);
            break;

            case GameMode.HARDCD:
            break;

            case GameMode.REDIRECTORS:
            break;

            case GameMode.ALL:
            break;
        }
    }

    private void GenerateFS(int numUsers, int numDirs, int numPys, int numTexts) {
        _root = new INode();
        _root.Init("C:");

        HashSet<int> usedIndicies = new HashSet<int>();
        //users
        List<INode> userNodes = new List<INode>();
        for (int user = 0; user < numUsers; ++user) {
            int randomI = Random.Range(0, userNames.Length);
            while (!usedIndicies.Add(randomI)) {
                randomI = Random.Range(0, userNames.Length);
            }
            INode newUser = new INode();
            newUser.Init(userNames[randomI]);
            _root.AddChild(newUser);
            userNodes.Add(newUser);
        }

        usedIndicies.Clear();
        dirs = new List<INode>(userNodes);
        //dirs, choose a random directory to add to LOL
        for (int dir = 0; dir < numDirs; ++dir) {
            int randomDirNameI = Random.Range(0, directoryNames.Length);
            while(!usedIndicies.Add(randomDirNameI)) {
                randomDirNameI = Random.Range(0, directoryNames.Length);
            }
            string dirName = directoryNames[randomDirNameI];

            int randomDirI = Random.Range(0, dirs.Count);
            while (dirs[randomDirI].NumChildren() > MAX_INODES_IN_DIR) {
                randomDirI = Random.Range(0, dirs.Count);
            }
            INode parent = dirs[randomDirI];

            INode newDir = new INode();
            newDir.Init(dirName);
            parent.AddChild(newDir);
            dirs.Add(newDir);
        }

        usedIndicies.Clear();
        pythonScripts = new List<INode>();
        //add python
        for (int py = 0; py < numPys; ++py) {
            int randomPyNameI = Random.Range(0, pythonFileNames.Length);
            while (!usedIndicies.Add(randomPyNameI)) {
                randomPyNameI = Random.Range(0, pythonFileNames.Length);
            }
            string pyName = pythonFileNames[randomPyNameI];

            int randomDirI = Random.Range(0, dirs.Count);
            while (dirs[randomDirI].NumChildren() > MAX_INODES_IN_DIR) {
                randomDirI = Random.Range(0, dirs.Count);
            }
            INode parentDir = dirs[randomDirI];
            INode pyNode = new INode();
            pyNode.Init(pyName, true);
            parentDir.AddChild(pyNode);
            pythonScripts.Add(pyNode);
        }

        usedIndicies.Clear();
        textFiles = new List<INode>();
        //add textfiles
        for (int text = 0; text < numTexts; ++text) {
            int randomTextNameI = Random.Range(0, textFileNames.Length);
            while (!usedIndicies.Add(randomTextNameI)) {
                randomTextNameI = Random.Range(0, textFileNames.Length);
            }
            string textName = textFileNames[randomTextNameI];

            int randomDirI = Random.Range(0, dirs.Count);
            while (dirs[randomDirI].NumChildren() > MAX_INODES_IN_DIR) {
                randomDirI = Random.Range(0, dirs.Count);
            }
            INode parentNode = dirs[randomDirI];
            INode textNode = new INode();
            textNode.Init(textName, true);
            parentNode.AddChild(textNode);
            textFiles.Add(textNode);
        }

    }

    public void ReceiveCommand(CMDType command, List<string> args) {
        switch (command) {
            case CMDType.CD:
            if (args.Count != 1) {
                _terminal.DisplayError(ErrorMessageType.InvalidNumberOfArgs, "cd");
                break;
            }
            ChangeDirectory(args[0]);
            break;

            case CMDType.LS:
            if (args.Count != 0) {
                _terminal.DisplayError(ErrorMessageType.InvalidNumberOfArgs, "ls");
                break;
            }
            List();
            break;

            case CMDType.PWD:
            if (args.Count != 0) {
                _terminal.DisplayError(ErrorMessageType.InvalidNumberOfArgs, "pwd");
                break;
            }
            PrintWorkingDirectory();
            break;

            case CMDType.CLEAR:
            if (args.Count != 0) {
                _terminal.DisplayError(ErrorMessageType.InvalidNumberOfArgs, "clear");
                break;
            }

            _terminal.ClearTerminal();
            break;

            case CMDType.PYTHON3:
            if (args.Count == 0) {
                _terminal.DisplayError(ErrorMessageType.InvalidNumberOfArgs, "python3");
                break;
            }
            Python(args);
            break;
        }
    }

    private void ChangeDirectory(string path) {
        if (path.Length == 0) {
            _terminal.DisplayError(ErrorMessageType.PathNotFound, path);
            return;
        }

       INode result = GetINode(path);
        if (result == null) {
            _terminal.DisplayError(ErrorMessageType.PathNotFound, path);
        } else if (result.file) {
            _terminal.DisplayError(ErrorMessageType.FileNotDirectory, path);
        } else {
            _currentNode = result;
            _terminal.SetCurrentPath(_currentNode.path, true);
            return;
        }
    }

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

    private INode GetINode(string path) {
        TerminalType termType = GameController.Get.TerminalType;
        if (termType == TerminalType.Windows) {path = path.Replace('/', '\\');};
        char slash = termType == TerminalType.Windows ? '\\' : '/';
        bool isAbsolute = path[0] == slash;
        INode startingNode = isAbsolute ? _root : _currentNode;
        if (isAbsolute) {
            path = path.Substring(1, path.Length - 1); //remove that first backslash
        }
        string[] dirs = path.Split(slash);
        INode result = CDHelper(startingNode, startingNode, dirs[dirs.Length - 1], dirs, 0);
        return result;
    }

    private void List() {
        List<INode> children = _currentNode.children;
        string resultingList = "";
        foreach (INode node in children) {
            resultingList += node.name + "\n";
        }
        _terminal.DisplayMessage(resultingList);
    }

    //prints current working directory
    private void PrintWorkingDirectory() {
        _terminal.DisplayMessage("\n" + _currentNode.path + "\n");
    }

    public void ForceCD(string absolutePath) {
        absolutePath = absolutePath.Replace('/', '\\');
        absolutePath = absolutePath.Substring(1, absolutePath.Length - 1); //remove that first backslash
        string[] dirs = absolutePath.Split('\\');
        INode result = CDHelper(_root, _root, dirs[dirs.Length - 1], dirs, 0);
        _currentNode = result;
        _terminal.SetCurrentPath(_currentNode.path, false);
    }

    public void AutocompletePath(string path) {
        TerminalType termType = GameController.Get.TerminalType;
        char slash = termType == TerminalType.Windows ? '\\' : '/';
        bool isAbsolute = path[0] == slash;
        INode startingNode = isAbsolute ? _root : _currentNode;
        if (isAbsolute) {
            path = path.Substring(1, path.Length - 1); //remove that first backslash
        }
        string[] dirs = path.Split(slash);
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
                autocompletedpath += s + slash;
            }
            autocompletedpath = autocompletedpath.Substring(0, autocompletedpath.Length - 1); //AHAHAHAHA FENCE POST
            if (isAbsolute) {autocompletedpath = slash + autocompletedpath;}
            _terminal.TextTabAutoComplete(autocompletedpath);
        } else {
            //we have many results, print them all out, then go to a new line with the same command!
            _terminal.ShowPossibleAutoComplete(ls);
        }
    }

    private void Python(List<string> args) {
        string redirector = "";
        //look for redirector, only worry about one rn LOL, i really should've did this like OS
        if (args.Count > 3) {
            //TODO implement a REAL command struct LOL
            return;
        } else if (args.Count == 2) {
            _terminal.DisplayError(ErrorMessageType.InvalidArguments, args[2]);
            return;
        } else if (args.Count == 3) {
            if (args[1] == "<" || args[1] == ">") {
                redirector = args[1];
            } else {
                _terminal.DisplayError(ErrorMessageType.InvalidArguments, args[1]);
                return;
            }
        } 
        
        string path = args[0];
        INode result = GetINode(path);
        if (result == null) { 
            _terminal.DisplayError(ErrorMessageType.PathNotFound, path);
        } else if (!result.file) {
            _terminal.DisplayError(ErrorMessageType.DirectoryNotFile, result.name);
        } else if (result.name.Substring(result.name.Length - 3, 3) != ".py"){
            _terminal.DisplayError(ErrorMessageType.NotValidFile, result.name);
        } else {
            if (redirector.Length > 0) {
                //check redirect path
                string redirectPath = args[2];
                INode getter = GetINode(redirectPath);
                if (getter == null) {
                    _terminal.DisplayError(ErrorMessageType.PathNotFound, redirectPath); 
                    return;
                } else if (!getter.file) {
                    _terminal.DisplayError(ErrorMessageType.DirectoryNotFile, getter.name);
                    return;
                } else if (getter.name.Substring(getter.name.Length - 4, 4) != ".txt") {
                    _terminal.DisplayError(ErrorMessageType.NotValidFile, getter.name);
                    return;
                } else {
                    //it's a valid path
                    redirector += getter.name;
                }
            }

            //for now do nothing lol if it's not guided
        }
    }


    private string[] userNames = new string[]
{
    "baggu",
    "spoon",
    "party",
    "test",
    "myuser",
    "thisuser",
    "tomato",
    "potato",
    "polar",
    "milo",
    "super",
    "river"
};

    private string[] directoryNames = new string[]
{
    "Documents",
    "Pictures",
    "Music",
    "Videos",
    "Downloads",
    "Desktop",
    "Projects",
    "Games",
    "Work",
    "Personal",
    "Archives",
    "Backups",
    "Photos",
    "Scripts",
    "Anims",
    "Textures",
    "Models",
    "Source",
    "Assets",
    "Editor",
    "Builds",
    "Plugins",
    "Libraries",
    "Resources",
    "Shaders",
    "Prefabs",
    "Scenes",
    "Mats",
    "Assets",
    "Logs",
    "Temp",
    "Configs",
    "Exports",
    "Import",
    "Settings",
    "Themes",
    "Profiles",
    "Demos",
    "Tests",
    "Utils",
    "Tools",
    "Manuals",
    "Refs",
    "Datasets",
    "Notes",
    "Layouts",
    "Versions",
    "Packages",
    "Icons"
};

string[] pythonFileNames = new string[]
{
    "main.py",
    "config.py",
    "app.py",
    "test.py",
    "utils.py",
    "setup.py",
    "data.py",
    "light.py",
    "server.py",
    "client.py",
    "script.py",
    "module.py",
    "function.py",
    "model.py",
    "views.py",
    "con.py",
    "tasks.py",
    "helper.py",
    "logger.py",
    "constants.py"
};

string[] textFileNames = new string[]
{
    "notes.txt",
    "readme.txt",
    "log.txt",
    "summary.txt",
    "draft.txt",
    "outline.txt",
    "report.txt",
    "todo.txt",
    "ideas.txt",
    "tasks.txt",
    "memo.txt",
    "journal.txt",
    "data.txt",
    "test.txt",
    "backup.txt",
    "config.txt",
    "script.txt",
    "details.txt",
    "results.txt",
    "history.txt"
};


}
