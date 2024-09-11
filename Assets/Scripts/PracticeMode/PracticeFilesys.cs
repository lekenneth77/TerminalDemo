using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PracticeFilesys : MonoBehaviour
{
    private INode _root;
    private const int MAX_INODES_IN_DIR = 2;
    private List<INode> dirs;
    private List<INode> pythonScripts;
    private List<INode> textFiles;

    void Awake()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    public INode GetRoot() {
        return _root;
    }

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
    "Animations",
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
    "Materials",
    "StreamingAssets",
    "Logs",
    "Temp",
    "Configurations",
    "Exports",
    "Import",
    "Settings",
    "Themes",
    "Profiles",
    "Demos",
    "Tests",
    "Utilities",
    "Tools",
    "Manuals",
    "References",
    "Datasets",
    "Notes",
    "Snapshots",
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
    "analysis.py",
    "server.py",
    "client.py",
    "script.py",
    "module.py",
    "function.py",
    "model.py",
    "views.py",
    "controller.py",
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
    "instructions.txt",
    "backup.txt",
    "config.txt",
    "script.txt",
    "details.txt",
    "results.txt",
    "history.txt"
};


}
