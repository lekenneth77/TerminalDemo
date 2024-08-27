using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Get {get; private set;}
    public FileSystem Filesys;
    public TerminalTextHandler Terminal;

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
