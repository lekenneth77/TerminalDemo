using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
public enum TerminalType {
    Windows, //command prompt
    Mac,
}

public class GameController : MonoBehaviour
{
    public static GameController Get {get; private set;}
    public FileSystem Filesys;
    public TerminalTextHandler Terminal;
    public FileExplorerManager FileExplorer;
    public Transform PopupContainer;
    public DialogueHandler Dialogue;
    public TerminalType TerminalType = TerminalType.Windows;
    public CanvasGroup titleFadeIn;
    private const float TITLE_ANIM_DURATION = 1f;
    private const float TITLE_HOW_LONG = 2f;
    public int CurrentCH = 0;

    public bool Debug = false;
    public bool Paused = false;
    public bool AnimDone = false;

    void Awake()
    {
        if (Get) {
            Destroy(gameObject);
            return;
        } else {
            Get = this;
        }
        
        if (TerminalTypeSingleton.Get) {
            TerminalType = TerminalTypeSingleton.Get.terminalType;
        }
    }

    void Start()
    {
        if (Debug) {
            Dialogue.StartDialogue();
            return;
        }
        
        titleFadeIn.alpha = 0;
        var sequence = DOTween.Sequence();
        if (LoadingScreen.Get) {
            sequence.AppendCallback(() => LoadingScreen.Get.Dismiss());
        }
        sequence.AppendInterval(1.5f);
        sequence.Append(titleFadeIn.DOFade(1, TITLE_ANIM_DURATION));
        sequence.AppendInterval(TITLE_HOW_LONG);
        sequence.Append(titleFadeIn.DOFade(0, TITLE_ANIM_DURATION));
        sequence.AppendInterval(0.5f);
        sequence.AppendCallback(() => titleFadeIn.gameObject.SetActive(false));
        sequence.AppendCallback(Dialogue.StartDialogue);
        sequence.OnComplete(() => AnimDone = true);
        sequence.Play();

    }

    
}
