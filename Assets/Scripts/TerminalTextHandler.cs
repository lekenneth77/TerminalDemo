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
    private string _commandCopy = ""; //USED TO SAVE STATE FOR DOWN ARROW KEY
    private bool _savedTypingCommand = false;
    private InputController ic;
    private List<string> _upCommands = new List<string>();
    private int _upIndex = 0;
    public bool isInit = false;
    //TODO ADD A LIMIT TO COMMAND AND LIST/STACK PUSHES

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
        isInit = true;
    }

    public void EnableKeyboard() {
        ic.Keyboard.Enable();
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    public void SetVacuumView(bool view) {
        _vacuum.SetActive(view);
        if (view) {ic.Keyboard.Disable();}
        else {ic.Keyboard.Enable();}
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
                    ResizeTextbox();
                }
            } else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) {
                if (_upCommands.Count > 100) {_upCommands.Clear();}
                _upCommands.Add(_command); //push command
                _upIndex = _upCommands.Count;
                _savedTypingCommand = false;
                EnterCommand();
                _command = "";
            } else if (Input.GetKeyDown(KeyCode.Tab)) {
                //try to auto complete the last word, space seperated
                string lastWord = GetLastWord(_command);
                GameController.Get.Filesys.AutocompletePath(lastWord);
            } else if (Input.GetKeyDown(KeyCode.UpArrow)) {
                if (_upIndex > 0) --_upIndex;
                if (_upIndex >= 0 && _upIndex < _upCommands.Count) {
                    if (_upIndex == _upCommands.Count - 1 && !_savedTypingCommand) {
                        //save the current typing command
                        _savedTypingCommand = true;
                        _commandCopy = _command;
                    }
                    //replace current line
                    string lastLine = GetLastLine(_text.text);
                    _text.text = _text.text.Substring(0, _text.text.Length - lastLine.Length);
                    _text.text += PopBack(_currentPath) + PushBack(_upCommands[_upIndex], "|");
                    _command = _upCommands[_upIndex];
                    ResizeTextbox();
                }

            } else if (Input.GetKeyDown(KeyCode.DownArrow)) {
                if (_upIndex < _upCommands.Count) ++_upIndex;
                if (_upIndex < _upCommands.Count) {
                    //replace current line
                    string lastLine = GetLastLine(_text.text);
                    _text.text = _text.text.Substring(0, _text.text.Length - lastLine.Length);
                    _text.text += PopBack(_currentPath) + PushBack(_upCommands[_upIndex], "|");
                    _command = _upCommands[_upIndex];
                    ResizeTextbox();
                } else {
                    //go back down to the typing line, need to use the saved copy if we've done that
                    string lastLine = GetLastLine(_text.text);
                    _text.text = _text.text.Substring(0, _text.text.Length - lastLine.Length);
                    if(_savedTypingCommand) _command = _commandCopy;
                    _text.text += PopBack(_currentPath) + PushBack(_command, "|");
                    ResizeTextbox();
                }

            } else if (Input.inputString != "") {
                char key = Input.inputString[0];
                _text.text = PopBack(_text.text);
                _text.text += key;
                _text.text = PushBack(_text.text, "|"); // we should really change this to, idk INSERT?!!?!!
                _command += key;
                _savedTypingCommand = false;
                _upIndex = _upCommands.Count;
                ResizeTextbox();
            } 
        }

    }

    public void EnterCommand() {
        _text.text = PopBack(_text.text);
        string cmd;
        List<string> args = ParseCommand(_command, out cmd);
        for (int i = 0; i < args.Count; i++) { //get rid of empty args
            if (args[i].Length == 0) {
                args.RemoveAt(i);
                --i;
            }
        }
        CMDType type = TranslateCMDType(cmd, args.Count);
        if (type == CMDType.UNKNOWN) {
            if (GameController.Get.Filesys._guided && GameController.Get.Dialogue.CheckIfCorrect(CMDType.UNKNOWN, cmd)) {
                GameController.Get.Filesys.AlertCorrectCMD();
            } else {
                GameController.Get.Filesys.AlertIncorrectCMD();
                return;
            }
            DisplayError(ErrorMessageType.UnrecognizedCommand, cmd);
        } else {
            GameController.Get.Filesys.ReceiveCommand(type, args);
        }
    }

    public void DisplayError(ErrorMessageType msgType, string cause, bool doNewLine = true) {
        string msg = "";
        switch (msgType) {
            case ErrorMessageType.UnrecognizedCommand:
            msg = cause + " is not a recognized command";
            DisplayMessage(msg, doNewLine);
            break;

            case ErrorMessageType.PathNotFound:
            msg = "Cannot find path " + cause + " because it does not exist";
            DisplayMessage(msg, doNewLine);
            break; 

            case ErrorMessageType.InvalidNumberOfArgs:
            msg = "Invalid Number of args for this command: " + cause;
            DisplayMessage(msg, doNewLine);
            break;

            case ErrorMessageType.FileNotDirectory:
            msg = cause + " is a file, not a directory";
            DisplayMessage(msg, doNewLine);
            break;

            case ErrorMessageType.InvalidArguments:
            msg = "Invalid argument: " + cause;
            DisplayMessage(msg, doNewLine);
            break;

            case ErrorMessageType.DirectoryNotFile:
            msg = cause + " is a directory, not a file";
            DisplayMessage(msg, doNewLine);
            break;

            case ErrorMessageType.NotValidFile:
            msg = cause + " is not a valid file for the command.";
            DisplayMessage(msg, doNewLine);
            break;

            default:
            DisplayMessage("What in the world did you just type into the terminal?", doNewLine);
            break;
        }
    }

    public void SetCurrentPath(string p, bool newLine = true) {
        _currentPath = p + "> |";
        if (newLine) {_text.text += "\n" + _currentPath;}
        else {_text.text = _currentPath;}
        ResizeTextbox();
    }

    public void DisplayMessage(string message, bool doNewLine = true) {
        _text.text += '\n';
        if (doNewLine) {
            _text.text += message + '\n';
            _text.text += _currentPath;
        } else {
            _text.text += message;
        }
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
        ResizeTextbox();
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
        ResizeTextbox();
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

        int length = 0;
        while (index >= 0 && paragraph[index] != '\n') {
            index--;
            length++;
        }
        ++index;

        return paragraph.Substring(index, length);
    }

    private List<string> ParseCommand(string str, out string cmd) {
        List<string> ls = new List<string>(str.Split(' '));
        cmd = ls[0];
        ls.RemoveAt(0);
        return ls;
    }

    private void ResizeTextbox() {
        if (_text.text.Length > 1000) {
            ClearTerminal();
            return;
        }
        _text.ForceMeshUpdate();
        float textHeight = _text.GetRenderedValues(false).y;
        float textWidth = _text.transform.parent.GetComponent<RectTransform>().sizeDelta.x;
        _text.transform.parent.GetComponent<RectTransform>().sizeDelta = new Vector2(textWidth, textHeight);
        _scrollRect.normalizedPosition = Vector2.zero;
    }

    public CMDType TranslateCMDType(string cmd, int numArgs) {
        TerminalType type = GameController.Get.TerminalType;
        if (type == TerminalType.Mac) {
            switch(cmd) {
                case "cd":
                return CMDType.CD;
                
                case "ls":
                return CMDType.LS;

                case "pwd":
                return CMDType.PWD;

                case "clear":
                return CMDType.CLEAR;

                case "python3":
                return CMDType.PYTHON3;

                default:
                return CMDType.UNKNOWN;
            }
        } else { //COMMAND PROMPT
            switch(cmd) {
                case "cd":
                return numArgs == 0 ? CMDType.PWD : CMDType.CD; //THIS WON'T WORK IF YOU'RE DOING CD > TEXTFILE LOLOL
                
                case "dir":
                return CMDType.LS;

                case "cls":
                return CMDType.CLEAR;

                case "python3":
                return CMDType.PYTHON3;

                default:
                return CMDType.UNKNOWN;
            }
        }
    }

}
