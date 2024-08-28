using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;

public enum AdvanceDialogueType {
Auto,
Button,
Event
};

[Serializable]
public struct Dialogue {
    public AdvanceDialogueType type;
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
    [SerializeField] private Image _portraitImg;
    private int _currDialogueIndex = 0;
    private float LETTER_DELAY = 0.035f;
    private float AUTO_DELAY = 1f;
    private const float FADE_IN = 1f;
    private const float FADE_OUT = 1f;
    private const int FAILS_MAX = 2;
    private CMDType _tgtCmd;
    private string _tgtPath;
    private int numFails = 0;
    TerminalTextHandler terminal;
    FileSystem filesys;
    
    // Start is called before the first frame update
    void Start()
    {
        terminal = GameController.Get.Terminal; 
        filesys = GameController.Get.Filesys;
        filesys.CorrectCMDReceived += DisplayCorrectDialogue;
        filesys.IncorrectCMDReceived += DisplayIncorrectDialogue;

        _advanceButton.onClick.AddListener(OnAdvanceButtonClicked);
        _advanceButton.gameObject.SetActive(false);
        _textField.text = "";
        _currDialogueIndex = 0;
        StartDialogue();
    }

    public void OnDestroy() {
        filesys.CorrectCMDReceived -= DisplayCorrectDialogue;
        filesys.IncorrectCMDReceived -= DisplayIncorrectDialogue;
    }

    public void StartDialogue()
    {
        _container.alpha = 0;
        _container.gameObject.SetActive(true);
        var sequence = DOTween.Sequence();
        sequence.Append(_container.DOFade(1, FADE_IN));
        sequence.AppendInterval(0.25f);
        sequence.AppendCallback(NextDialogue);
        sequence.Play();        
    }

    public void NextDialogue()
    {
        if (_currDialogueIndex < _dialogues.Count) {
            StopAllCoroutines();
            StartCoroutine("DisplayCurrentDialogue");
        } else {
            DismissDialogue();
        }
    }

    private IEnumerator DisplayCurrentDialogue()
    {
        _textField.text = "";
        Dialogue currDialogue = _dialogues[_currDialogueIndex];
        _portraitImg.sprite = currDialogue.portrait;
        //TODO need to disable terminal when dialogue is typing
        if (currDialogue.type == AdvanceDialogueType.Event && numFails == 0) {
            terminal.SetVacuumView(true);
            if (currDialogue.startingPath.Length > 0) {
                filesys.ForceCD(currDialogue.startingPath);
                terminal.ClearTerminal();
            }
        }

        string currDialogueText = _dialogues[_currDialogueIndex].text;
        if (numFails >= FAILS_MAX && currDialogue.hintText.Length > 0) {
            currDialogueText += "\n" + currDialogue.hintText;
        }
        for (int i = 0; i < currDialogueText.Length; i++) {
            _textField.text += currDialogueText[i];
            yield return new WaitForSeconds(LETTER_DELAY);
        }        
        
        switch (currDialogue.type) {
            case AdvanceDialogueType.Auto:
            _currDialogueIndex++;
            yield return new WaitForSeconds(AUTO_DELAY);
            NextDialogue();
            break;
            case AdvanceDialogueType.Button:
            _currDialogueIndex++;
            _advanceButton.gameObject.SetActive(true);
            break;
            case AdvanceDialogueType.Event:
            terminal.SetVacuumView(false);
            filesys.SetGuidedMode(true);
            _tgtCmd = currDialogue.cmd;
            _tgtPath = currDialogue.tgtPath;
            break;
            default:
            break;
        }
    }

    private void DisplayCorrectDialogue() {
        filesys.SetGuidedMode(false);
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
        _advanceButton.gameObject.SetActive(false);
    }

    public void DismissDialogue()
    {
        _container.alpha = 1;
        var sequence = DOTween.Sequence();
        sequence.Append(_container.DOFade(0, FADE_OUT));
        sequence.AppendInterval(0.25f);
        sequence.AppendCallback(() => _container.gameObject.SetActive(false));
        sequence.Play(); 
    }

    public bool CheckIfCorrect(CMDType type, string path) {
        if (type == _tgtCmd && path == _tgtPath) {
            return true;
        } else {
            return false;
        }
    }
}
