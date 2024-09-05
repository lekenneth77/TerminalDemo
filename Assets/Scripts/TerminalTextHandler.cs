using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using System;
using Mono.Cecil;

public enum ErrorMessageType {
    UnrecognizedCommand,
    PathNotFound,
    InvalidNumberOfArgs,
    FileNotDirectory,
    DirectoryNotFile,
    NotValidFile,
    InvalidArguments
}

public enum CMDType {
    CD,
    PWD,
    LS,
    CLEAR,
    PYTHON3,
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
    }

    public void Init(string path) {
        _text.text = path + "> |";
        _currentPath = _text.text;
        ResizeTextbox();
        ic = new InputController();
        ic.Keyboard.AddCallbacks(this);
        ic.Keyboard.Enable();
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
        ic.Keyboard.RemoveCallbacks(this);
        ic.Keyboard.Disable();
        ic.Keyboard.KeyboardPress.Dispose();
        ic.Disable();
        ic.Dispose();
    }

    public void SetVacuumView(bool view) {
        _vacuum.SetActive(view);
    }


    public void OnKeyboardPress(InputAction.CallbackContext context)
    {
        if (!this || _vacuum.activeInHierarchy || GameController.Get.Paused) {return;}
     
        if (context.performed) {
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
                for (int i = 0; i < args.Count; i++) { //get rid of empty args
                    if (args[i].Length == 0) {
                        args.RemoveAt(i);
                        --i;
                    }
                }
                GameController.Get.Filesys.ReceiveCommand(cmd, args);
                _command = "";
            } else if (Input.GetKeyDown(KeyCode.Tab)) {
                //try to auto complete the last word, space seperated
                string lastWord = GetLastWord(_command);
                Debug.Log("Last Word: " + lastWord);
                GameController.Get.Filesys.AutocompletePath(lastWord);
            } else if (Input.inputString != "") {
                char key = Input.inputString[0];
                _text.text = PopBack(_text.text);
                _text.text += key;
                _text.text = PushBack(_text.text, "|"); // we should really change this to, idk INSERT?!!?!!
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

            case ErrorMessageType.FileNotDirectory:
            msg = cause + " is a file, not a directory";
            DisplayMessage(msg);
            break;

            case ErrorMessageType.InvalidArguments:
            msg = "Invalid argument: " + cause;
            DisplayMessage(msg);
            break;

            case ErrorMessageType.DirectoryNotFile:
            msg = cause + " is a directory, not a file";
            DisplayMessage(msg);
            break;

            case ErrorMessageType.NotValidFile:
            msg = cause + " is not a valid file for the command.";
            DisplayMessage(msg);
            break;

            default:
            DisplayMessage("What in the world did you just type into the terminal?");
            break;
        }
    }

    public void SetCurrentPath(string p, bool newLine = true) {
        _currentPath = p + "> |";
        if (newLine) _text.text += "\n" + _currentPath;
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

    public void TextTabAutoComplete(string autocompletedstring) {
        //remove last word from current command and replace it with the autocompletedstring
        //this makes a huge assumption that the last word is like, an actual word and not just a bunch of nothing or spaces

        string lastWord = GetLastWord(_command);
        _command = _command.Substring(0, _command.Length - lastWord.Length);
        _command += autocompletedstring;
        _text.text = PopBack(_text.text); //remove |
        _text.text = _text.text.Substring(0, _text.text.Length - lastWord.Length) + autocompletedstring + "|";
    }

    public void ShowPossibleAutoComplete(List<string> possibilities) {
        string lastLine = GetLastLine(_text.text);
        _text.text = PopBack(_text.text); //remove |
        _text.text += "\n";
        foreach (string str in possibilities) {
            _text.text += str + " ";
        }
        _text.text += "\n";
        _text.text += lastLine;
    }

    private string PopBack(string str) {
        return str.Substring(0, str.Length - 1);
    }

    private string PushBack(string str, string add) {
        return str.Insert(str.Length, add);
    }

    private string GetLastWord(string sentence) {
        int index = sentence.Length - 1;
        //start at a real character
        while (index >= 0 && sentence[index] == ' ') {
            index--;
        }
        if (index < 0){return "";}
        //cd doc
        int length = 0;
        while (index >= 0 && sentence[index] != ' ') {
            index--;
            length++;
        }
        ++index;

        return sentence.Substring(index, length);
    }

    private string GetLastLine(string paragraph) {
        int index = paragraph.Length - 1;
        if (paragraph[index] == '\n') {return "";}

        int length = 1;
        while (index > 0 && paragraph[index] != '\n') {
            index--;
            length++;
        }

        return paragraph.Substring(index, length);
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
