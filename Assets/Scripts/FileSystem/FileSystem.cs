using System.Collections.Generic;
using UnityEngine;

public struct INode {
    public string name;
    public string path;
    public List<INode> children;
}

public class FileSystem : MonoBehaviour
{
    private INode _root = new INode();
    private INode _currentNode;

    void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
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
            if (args.Count != 1) {
                GameController.Get.Terminal.DisplayError(ErrorMessageType.InvalidNumberOfArgs, command);
                break;
            }
            List();
            break;

            case "pwd":
            if (args.Count != 1) {
                GameController.Get.Terminal.DisplayError(ErrorMessageType.InvalidNumberOfArgs, command);
                break;
            }
            PrintWorkingDirectory();
            break;

            default:
            GameController.Get.Terminal.DisplayError(ErrorMessageType.UnrecognizedCommand, command);
            break;
        }
    }

    //changes directory to given path
    private void ChangeDirectory(string path) {

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



}


