using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FileExplorerManager : MonoBehaviour
{
    public FileExplorerScreen CurrentScreen;
    public List<Button> ShortcutButtons;
    public List<FileExplorerScreen> ShortcutScreens;
    [SerializeField] private GameObject _vacuum;
    
    public void Awake() {
        for (int i = 0; i < ShortcutButtons.Count; i++) {
            int id = i;
            var screen = ShortcutScreens[id];
            ShortcutButtons[i].onClick.AddListener(() => ShowNewScreen(screen));
        }
        CurrentScreen.gameObject.SetActive(true);
    }

    public void SetVacuumView(bool view) {
        _vacuum.SetActive(view);
    }

    public void ShowNewScreen(FileExplorerScreen newScreen) {
        CurrentScreen.gameObject.SetActive(false);
        newScreen.gameObject.SetActive(true);
        CurrentScreen = newScreen;
    }
}
