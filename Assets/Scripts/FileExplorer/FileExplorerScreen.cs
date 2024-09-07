using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FileExplorerScreen : MonoBehaviour
{
    public GameObject prevScreen;
    public List<Button> Buttons;
    public Button backButton;
    private int _currentlyActiveButton = -1;
    private Color _transparentColor = new Color(1, 1, 1, 0);
    private Color _highlightColor = new Color(0, 0.85f, 0.85f, 0.5f);
    [SerializeField] private TextMeshProUGUI _pathText;
    // Start is called before the first frame update
    void Awake()
    {
        _currentlyActiveButton = -1;
        for (int i = 0; i < Buttons.Count; i++) {
            int id = i;
            Buttons[i].onClick.AddListener(() => HandleButtonClick(id));
        }
        backButton.onClick.AddListener(HandleBackButton);
        if (!prevScreen) {
            backButton.enabled = false;
        }
    }

    void Start()
    {
        if (GameController.Get.TerminalType == TerminalType.Mac) {
            _pathText.text = _pathText.text.Replace("\\", "/");
        } else {
            _pathText.text = _pathText.text.Replace("/", "\\");
        }
    }

    public void OnEnable() {
        if (_currentlyActiveButton != -1) {Buttons[_currentlyActiveButton].GetComponent<Image>().color = _transparentColor;}
        _currentlyActiveButton = -1;

        if (!prevScreen) {
            backButton.enabled = false;
        }
    }

    private void HandleButtonClick(int id) {
        if (_currentlyActiveButton == id) {
            //activate that button!
            Buttons[id].GetComponent<Image>().color = _transparentColor;
            if (Buttons[id].GetComponent<File>()) {
                Buttons[id].GetComponent<File>().ShowPopup();
            } else if (Buttons[id].GetComponent<Directory>()) {
                var newScreen = Buttons[id].GetComponent<Directory>().GetFileExplorerScreen();
                GameController.Get.FileExplorer.ShowNewScreen(newScreen.GetComponent<FileExplorerScreen>());
            }
            _currentlyActiveButton = -1;
        } else {
            if (_currentlyActiveButton != -1) {Buttons[_currentlyActiveButton].GetComponent<Image>().color = _transparentColor;}
            _currentlyActiveButton = id;
            Buttons[id].GetComponent<Image>().color = _highlightColor;
        }
    }

    public void HandleBackButton() {
        if (!prevScreen) {return;}
        GameController.Get.FileExplorer.ShowNewScreen(prevScreen.GetComponent<FileExplorerScreen>());
        if (_currentlyActiveButton != -1) {Buttons[_currentlyActiveButton].GetComponent<Image>().color = _transparentColor;}
        _currentlyActiveButton = -1;
    }
}
