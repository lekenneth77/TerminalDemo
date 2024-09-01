using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;


[Serializable]
public struct Dialogue {
    public bool isEvent;
     [TextArea(3, 6)]
    public string text;
    public Sprite portrait;
    public string hintText;
    public CMDType cmd;
    public string tgtPath;
    public string startingPath;
}

public class DialogueHandler : MonoBehaviour
{
    [SerializeField] private CanvasGroup _container;
    [SerializeField] private TextMeshProUGUI _textField;
    [SerializeField] private List<Dialogue> _dialogues;
    [SerializeField] private Button _advanceButton;
    [SerializeField] private Button _dialogueTextContainer;
    [SerializeField] private Image _portraitImg;
    // [SerializeField] private Toggle _autoToggle;
    private int _currDialogueIndex = 0;
    private float LETTER_DELAY = 0.03f;
    private float AUTO_DELAY = 1f;
    private const int FAILS_MAX = 2;
    private bool auto = false;
    private CMDType _tgtCmd;
    private string _tgtPath;
    private int numFails = 0;
    TerminalTextHandler terminal;
    FileSystem filesys;
    public event Action<int> OnDialogueAdvance;
    private Dictionary<char, string> specialCharMap = new Dictionary<char, string>();
    private const string COLOR_TAG_ENDER = "</color>";
    private bool _skipText = false;
    
    // Start is called before the first frame update
    void Start()
    {
        terminal = GameController.Get.Terminal; 
        filesys = GameController.Get.Filesys;
        filesys.CorrectCMDReceived += DisplayCorrectDialogue;
        filesys.IncorrectCMDReceived += DisplayIncorrectDialogue;

        _dialogueTextContainer.onClick.AddListener(() => _skipText = true);

        _advanceButton.onClick.AddListener(OnAdvanceButtonClicked);
        _advanceButton.gameObject.SetActive(false);
        _textField.text = "";
        
        specialCharMap.Add('[', "<color=blue>");
        specialCharMap.Add('(', "<color=red>");
        specialCharMap.Add('{', "<color=orange>");
        
        _currDialogueIndex = 0;

    }
    public void OnDestroy() {
        filesys.CorrectCMDReceived -= DisplayCorrectDialogue;
        filesys.IncorrectCMDReceived -= DisplayIncorrectDialogue;
    }

    public void StartDialogue()
    {
        NextDialogue();        
    }

    public void NextDialogue()
    {
        StopAllCoroutines();
        if (_currDialogueIndex < _dialogues.Count) {
            OnDialogueAdvance?.Invoke(_currDialogueIndex);
            StartCoroutine("DisplayCurrentDialogue");
        } else {
            SceneManager.LoadSceneAsync("CH" + (GameController.Get.CurrentCH + 1));
        }
    }

    private IEnumerator DisplayCurrentDialogue()
    {
        _skipText = false;
        _textField.text = "";
        Dialogue currDialogue = _dialogues[_currDialogueIndex];
        _portraitImg.sprite = currDialogue.portrait;

        if (currDialogue.isEvent && numFails == 0) {
            terminal.SetVacuumView(true);
            if (currDialogue.startingPath.Length > 0) {
                Debug.Log("Forced Starting Path!");
                filesys.ForceCD(currDialogue.startingPath);
                terminal.ClearTerminal();
            }
        }

        string currDialogueText = _dialogues[_currDialogueIndex].text;
        if (numFails >= FAILS_MAX && currDialogue.hintText.Length > 0) {
            currDialogueText += "\n" + currDialogue.hintText;
        }
        
        bool specialText = false;
        for (int i = 0; i < currDialogueText.Length; i++) {
            char curChar = currDialogueText[i];
            if (specialCharMap.ContainsKey(curChar)) {
                _textField.text += specialCharMap[curChar];
                specialText = true;
                _textField.text += COLOR_TAG_ENDER;
            } else if (curChar == ']' || curChar == ')' || curChar == '}') {
                specialText = false;
            } else {
                if (specialText) {
                    _textField.text = _textField.text.Substring(0, _textField.text.Length - COLOR_TAG_ENDER.Length);
                }
                
                _textField.text += currDialogueText[i];
                
                if (specialText) {
                    _textField.text += COLOR_TAG_ENDER;
                }
            }
            
            float delay = _skipText ? 0 : LETTER_DELAY;
            yield return new WaitForSeconds(delay);
        }        

        if (currDialogue.isEvent) {
            terminal.SetVacuumView(false);
            filesys.SetGuidedMode(true);
            _tgtCmd = currDialogue.cmd;
            _tgtPath = currDialogue.tgtPath;
        } else if (auto) {
            _currDialogueIndex++;
            yield return new WaitForSeconds(AUTO_DELAY);
            NextDialogue();
        } else {
            _currDialogueIndex++;
            _advanceButton.gameObject.SetActive(true);
        }
    }

    private void DisplayCorrectDialogue() {
        filesys.SetGuidedMode(false);
        terminal.SetVacuumView(true);
        StopAllCoroutines();
        numFails = 0;
        _currDialogueIndex++;
        StartCoroutine("DisplayCurrentDialogue");
    }

    private void DisplayIncorrectDialogue() {
        numFails++;
        terminal.GoToNextLine();
        StopAllCoroutines();
        StartCoroutine("DisplayIncorrectHelper");
    }

    private IEnumerator DisplayIncorrectHelper() {
        _textField.text = "";
        string incorrText = "That wasn't correct, ";
        switch (_tgtCmd) {
            case CMDType.CD:
            incorrText += "it should be cd, space, and then the path, make sure your path is correct!";
            break;

            case CMDType.PWD:
            incorrText += "it should just be pwd, and that's it! No spaces or words after!";
            break;

            case CMDType.LS:
            incorrText += "it should just be ls, and that's it! No spaces or words after!";
            break;
        }
        for (int i = 0; i < incorrText.Length; i++) {
            _textField.text += incorrText[i];
            yield return new WaitForSeconds(LETTER_DELAY / 2f);
        }   
        yield return new WaitForSeconds(1f);
        _advanceButton.gameObject.SetActive(true);
    }

    private void OnAdvanceButtonClicked() {
        NextDialogue();
        _advanceButton.interactable = false;
        _advanceButton.interactable = true;

        _advanceButton.gameObject.SetActive(false);
    }

    // public void DismissDialogue()
    // {
    //     _container.alpha = 1;
    //     var sequence = DOTween.Sequence();
    //     sequence.Append(_container.DOFade(0, FADE_OUT));
    //     sequence.AppendInterval(0.25f);
    //     sequence.AppendCallback(() => _container.gameObject.SetActive(false));
    //     sequence.Play(); 
    // }

    public bool CheckIfCorrect(CMDType type, string path) {
        if (type == _tgtCmd && path == _tgtPath) {
            return true;
        } else {
            return false;
        }
    }
}
