using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using DG.Tweening;


[Serializable]
public struct Dialogue {
    public bool isEvent;
     [TextArea(3, 6)]
    public string text; //windows command prompt
     [TextArea(3, 6)]
    public string macLinuxText; //mac/linux text
    public Sprite portrait;
    public string hintText;
    public CMDType cmd;
    public string startingPath;
    public string tgtINodeName;
    public string redirectTgt;
}

public class DialogueHandler : MonoBehaviour
{
    [SerializeField] private CanvasGroup _mainContainer;
    [SerializeField] private CanvasGroup _container;
    [SerializeField] private TextMeshProUGUI _textField;
    [SerializeField] private List<Dialogue> _dialogues;
    [SerializeField] private Button _advanceButton;
    [SerializeField] private Button _dialogueTextContainer;
    [SerializeField] private Image _portraitImg;
    [SerializeField] private Image _dialogueImg;
    // [SerializeField] private Toggle _autoToggle;
    private int _currDialogueIndex = 0;
    private float LETTER_DELAY = 0.03f;
    private float AUTO_DELAY = 1f;
    private const int FAILS_MAX = 3;
    private bool auto = false;
    private CMDType _tgtCmd;
    private string _tgtINode;
    private string _redirINode;
    private int numFails = 0;
    TerminalTextHandler terminal;
    FileSystem filesys;
    public event Action<int> OnDialogueAdvance;
    public event Action PythonSuccess;
    private Dictionary<char, string> specialCharMap = new Dictionary<char, string>();
    private const string COLOR_TAG_ENDER = "</color>";
    private bool _skipText = false;
    public CheckmarkAnim CheckAnim;
    public Sprite crabPortrait;
    public Sprite bagguPortrait;
    public Sprite redGradient;
    public Sprite yellowGradient;


    
    // Start is called before the first frame update
    void Start()
    {
        terminal = GameController.Get.Terminal; 
        filesys = GameController.Get.Filesys;
        filesys.CorrectCMDReceived += DisplayCorrectDialogue;
        filesys.IncorrectCMDReceived += DisplayIncorrectDialogue;

        _dialogueTextContainer.onClick.AddListener(() => {
            _skipText = true;
            _dialogueTextContainer.enabled = false;
        });

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
        if (GameController.Get.CurrentCH == 0) {
            ShowDialogue();
        } else {
            NextDialogue();        
        }
    }

    public void NextDialogue()
    {
        StopAllCoroutines();
        if (_currDialogueIndex < _dialogues.Count) {
            OnDialogueAdvance?.Invoke(_currDialogueIndex);
            StartCoroutine("DisplayCurrentDialogue");
        } else {
            int nextCH = GameController.Get.CurrentCH + 1;
            if (nextCH > 6) {
                DismissDialogue();
                //go to title screen?
            } else {
                SceneManager.LoadSceneAsync("CH" + nextCH);
            }
        }
    }

    private IEnumerator DisplayCurrentDialogue()
    {
        yield return null;
        _textField.text = "";
        Dialogue currDialogue = _dialogues[_currDialogueIndex];
        _portraitImg.sprite = currDialogue.portrait;
        _dialogueImg.sprite = currDialogue.portrait == bagguPortrait ? yellowGradient : redGradient;

        if (currDialogue.isEvent && numFails == 0) {
            terminal.SetVacuumView(true);
            if (currDialogue.startingPath.Length > 0) {
                filesys.ForceCD(currDialogue.startingPath);
                terminal.ClearTerminal();
            }
        }
        _skipText = false;
        _dialogueTextContainer.enabled = true;
        string currDialogueText = GameController.Get.TerminalType == TerminalType.Mac && currDialogue.macLinuxText.Length != 0 ? 
            currDialogue.macLinuxText : currDialogue.text;

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
        EventSystem.current.SetSelectedGameObject(terminal.gameObject);


        if (currDialogue.isEvent) {
            terminal.SetVacuumView(false);
            filesys.SetGuidedMode(true);
            _tgtCmd = currDialogue.cmd;
            _tgtINode = currDialogue.tgtINodeName;
            _redirINode = currDialogue.redirectTgt;
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
        _advanceButton.gameObject.SetActive(false);
        CheckAnim.AnimateCheckmark();
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
            if (GameController.Get.TerminalType == TerminalType.Mac) {
                incorrText += "it should just be pwd, and that's it! No spaces or words after!";
            } else {
                incorrText += "it should just be cd, and that's it! No spaces or words after!";
            }
            break;

            case CMDType.LS:
            if (GameController.Get.TerminalType == TerminalType.Mac) {
                incorrText += "it should just be ls, and that's it! No spaces or words after!";
            } else {
                incorrText += "it should just be dir, and that's it! No spaces or words after!";
            }
            break;

            default:
            incorrText += "type it just like how I'm telling you to.";
            break;
        }
        for (int i = 0; i < incorrText.Length; i++) {
            _textField.text += incorrText[i];
            yield return new WaitForSeconds(0);
        }  

        Dialogue currDialogue = _dialogues[_currDialogueIndex];
        if (numFails >= FAILS_MAX && currDialogue.hintText.Length > 0) {
            _textField.text += " ";
            string currDialogueText = currDialogue.hintText;
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
                
                yield return new WaitForSeconds(0);
            }        
        }
        
        yield return new WaitForSeconds(1f);
        _advanceButton.gameObject.SetActive(true);
    }

    private void OnAdvanceButtonClicked() {
        if (GameController.Get.Paused) {return;}
        NextDialogue();
        _advanceButton.interactable = false;
        _advanceButton.interactable = true;

        _advanceButton.gameObject.SetActive(false);
    }

    public void DismissDialogue()
    {
        _mainContainer.alpha = 1;
        var sequence = DOTween.Sequence();
        sequence.Append(_mainContainer.DOFade(0, 2f));
        sequence.AppendInterval(0.25f);
        sequence.AppendCallback(() => _mainContainer.gameObject.SetActive(false));
        sequence.OnComplete(() => {LoadingScreen.Get.AnimateIn("ChapterSelect");});
        sequence.Play(); 
    }

    public void ShowDialogue() 
    {
        _mainContainer.alpha = 0;
        var seq = DOTween.Sequence();
        seq.Append(_mainContainer.DOFade(1, 1.5f));
        seq.OnComplete(NextDialogue);
        seq.Play();
    }

    public bool CheckIfCorrect(CMDType type, string inodeName, string redirectTgtName = "") {
        if (type == _tgtCmd && inodeName == _tgtINode) {
            if (type == CMDType.PYTHON3) {
                PythonSuccess?.Invoke();
            }
            return redirectTgtName == _redirINode;
        } else {
            return false;
        }
    }
}
