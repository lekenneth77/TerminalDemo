using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FileExplorerManager : MonoBehaviour
{
    public FileExplorerScreen CurrentScreen;

    public void ShowNewScreen(FileExplorerScreen newScreen) {
        CurrentScreen.gameObject.SetActive(false);
        newScreen.gameObject.SetActive(true);
        CurrentScreen = newScreen;
    }
}
