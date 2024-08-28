using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum TerminalType {
    Windows, //powershell, WSL, bash
    Mac,
    CommandPrompt //the weird child
}
public class GameController : MonoBehaviour
{
    public static GameController Get {get; private set;}
    public FileSystem Filesys;
    public TerminalTextHandler Terminal;
    public FileExplorerManager FileExplorer;
    public Transform PopupContainer;
    public DialogueHandler Dialogue;
    public TerminalType TerminalType = TerminalType.Windows;

    void Awake()
    {
        if (Get) {
            Destroy(gameObject);
            return;
        } else {
            Get = this;
        }
    }
}
