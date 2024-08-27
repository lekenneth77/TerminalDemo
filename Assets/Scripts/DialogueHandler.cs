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
}

public class DialogueHandler : MonoBehaviour
{
    [SerializeField] private CanvasGroup _container;
    [SerializeField] private TextMeshProUGUI _textField;
    [SerializeField] private List<Dialogue> _dialogues;
    [SerializeField] private Button _advanceButton;
    private int _currDialogueIndex = 0;
    private float LETTER_DELAY = 0.05f;
    private float AUTO_DELAY = 1f;
    private const float FADE_IN = 1f;
    private const float FADE_OUT = 1f;
    


    // Start is called before the first frame update
    void Start()
    {
        _advanceButton.onClick.AddListener(OnAdvanceButtonClicked);
        _advanceButton.gameObject.SetActive(false);
        _textField.text = "";
        _currDialogueIndex = 0;
        StartDialogue();
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
            StartCoroutine("DisplayCurrentDialogue");
        } else {
            DismissDialogue();
        }
    }

    private IEnumerator DisplayCurrentDialogue()
    {
        _textField.text = "";
        Dialogue currDialogue = _dialogues[_currDialogueIndex];
        string currDialogueText = _dialogues[_currDialogueIndex].text;
        for (int i = 0; i < currDialogueText.Length; i++) {
            _textField.text += currDialogueText[i];
            yield return new WaitForSeconds(LETTER_DELAY);
        }        
        
        _currDialogueIndex++;
        switch (currDialogue.type) {
            case AdvanceDialogueType.Auto:
            yield return new WaitForSeconds(AUTO_DELAY);
            NextDialogue();
            break;
            case AdvanceDialogueType.Button:
            _advanceButton.gameObject.SetActive(true);
            break;
            case AdvanceDialogueType.Event:
            break;
            default:
            break;
        }

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
}
