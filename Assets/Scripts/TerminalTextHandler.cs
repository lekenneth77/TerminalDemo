using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using System;

public enum ErrorMessageType {
    UnrecognizedCommand,
    PathNotFound,
    InvalidNumberOfArgs
}

public enum CMDType {
    CD,
    PWD,
    LS,
    CLEAR,
    UNKNOWN
}

public class TerminalTextHandler : MonoBehaviour, InputController.IKeyboardActions
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private ScrollRect _scrollRect;
    [SerializeField] private GameObject _vacuum;
    private string _currentPath = "";
    private string _command = "";
    private InputController ic;

    
    void Awake()
    {
        _text.text = "";
        ic = new InputController();
        ic.Keyboard.AddCallbacks(this);
    }

    public void Init(string path) {
        _text.text = path + "> |";
        _currentPath = _text.text;
        ResizeTextbox();
        ic.Keyboard.Enable();
    }

    public void OnDestroy()
    {
        ic.Dispose();
    }

    public void SetVacuumView(bool view) {
        _vacuum.SetActive(view);
    }

    public void OnKeyboardPress(InputAction.CallbackContext context)
    {
        if (Input.inputString == "" || _vacuum.activeInHierarchy) {return;}
     
        if (context.performed) {
            char key = Input.inputString[0];
            if (Input.GetKeyDown(KeyCode.Backspace)) {
                if (_command.Length > 0) {
                    _text.text = PopBack(_text.text);
                    _text.text = PopBack(_text.text);
                    _text.text = PushBack(_text.text, "|");
                    _command = PopBack(_command);
                }
            } else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) {
                _text.text = PopBack(_text.text);
                string cmd;
                List<string> args = ParseCommand(_command, out cmd);
                GameController.Get.Filesys.ReceiveCommand(cmd, args);
                _command = "";
            } else {
                _text.text = PopBack(_text.text);
                _text.text += key;
                _text.text = PushBack(_text.text, "|");
                _command += key;
            }
        }

    }

    public void DisplayError(ErrorMessageType msgType, string cause) {
        string msg = "";
        switch (msgType) {
            case ErrorMessageType.UnrecognizedCommand:
            msg = cause + " is not a recognized command";
            DisplayMessage(msg);
            break;

            case ErrorMessageType.PathNotFound:
            msg = "Cannot find path " + cause + " because it does not exist";
            DisplayMessage(msg);
            break; 

            case ErrorMessageType.InvalidNumberOfArgs:
            msg = "Invalid Number of args for this command: " + cause;
            DisplayMessage(msg);
            break;

            default:
            DisplayMessage("What in the world did you just type into the terminal?");
            break;
        }
    }

    public void SetCurrentPath(string p) {
        _currentPath = p + "> |";
        _text.text += "\n" + _currentPath;
        ResizeTextbox();
    }

    public void DisplayMessage(string message) {
        _text.text += '\n';
        _text.text += message + '\n';
        _text.text += _currentPath;
        ResizeTextbox();
    }

    public void ClearTerminal() {
        _text.text = _currentPath;
        ResizeTextbox();
    }

    public void GoToNextLine() {
        _command = "";
        _text.text += "\n" + _currentPath;
        ResizeTextbox();
    }

    private string PopBack(string str) {
        return str.Substring(0, str.Length - 1);
    }

    private string PushBack(string str, string add) {
        return str.Insert(str.Length, add);
    }

    private string GetFirstWord(string sentence) {
        int index = 0;
        while (index < sentence.Length && sentence[index] != ' ') {
            index++;
        }

        return sentence.Substring(0, index);
    }

    private List<string> ParseCommand(string str, out string cmd) {
        //convert all forward slashes to backslashes
        str = str.Replace('/', '\\');
        List<string> ls = new List<string>(str.Split(' '));
        cmd = ls[0];
        ls.RemoveAt(0);
        return ls;
    }

    private void ResizeTextbox() {
        _text.ForceMeshUpdate();
        float textHeight = _text.GetRenderedValues(false).y;
        float textWidth = _text.transform.parent.GetComponent<RectTransform>().sizeDelta.x;
        _text.transform.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(textWidth, textHeight);
        _scrollRect.normalizedPosition = Vector2.zero;
    }

    public CMDType TranslateCMDType(string cmd) {
        TerminalType type = GameController.Get.TerminalType;
        if (type == TerminalType.Windows || type == TerminalType.Mac) {
            switch(cmd) {
                case "cd":
                return CMDType.CD;
                
                case "ls":
                return CMDType.LS;

                case "pwd":
                return CMDType.PWD;

                case "clear":
                return CMDType.CLEAR;

                default:
                return CMDType.UNKNOWN;
            }
        } else {
            switch(cmd) {
                case "cd":
                return CMDType.CD;
                
                case "dir":
                return CMDType.LS;

                //TODO NEED TO HANDLE THE STUPID THING WITH COMMAND PROMPT WHERE
                //CD EMPTY IS PWD

                // case "cd":
                // return CMDType.PWD;

                case "cls":
                return CMDType.CLEAR;

                default:
                return CMDType.UNKNOWN;
            }
        }
    }

}
