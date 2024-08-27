using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Get {get; private set;}
    public FileSystem Filesys;
    public TerminalTextHandler Terminal;
    public FileExplorerManager FileExplorer;
    public Transform PopupContainer;

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
