using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TerminalTypeSelector : MonoBehaviour
{
    [SerializeField] private Button _windowsButton;
    [SerializeField] private Button _macButton;
    [SerializeField] private Button _linuxButton;
    [SerializeField] private Sprite _unselectedSprite;
    [SerializeField] private Sprite _selectedSprite;

    // Start is called before the first frame update
    void Start()
    {
        _windowsButton.onClick.AddListener(() => UpdateAllButtons(TerminalType.Windows));
        _macButton.onClick.AddListener(() => UpdateAllButtons(TerminalType.Mac));
        _linuxButton.onClick.AddListener(() => UpdateAllButtons(TerminalType.Linux));

        GetComponent<CanvasGroup>().alpha = 0;
        UpdateAllButtons(TerminalType.Windows);
        var seq = DOTween.Sequence();
        seq.AppendInterval(0.5f);
        seq.Append(GetComponent<CanvasGroup>().DOFade(1, 1f));
        seq.Play();
    }

    private void UpdateAllButtons(TerminalType type) {
        switch (type) {
            case TerminalType.Windows:
                ChangeButtonImage(_windowsButton, _selectedSprite);
                ChangeButtonImage(_macButton, _unselectedSprite);
                ChangeButtonImage(_linuxButton, _unselectedSprite);
            break;

            case TerminalType.Mac:
                ChangeButtonImage(_windowsButton, _unselectedSprite);
                ChangeButtonImage(_macButton, _selectedSprite);
                ChangeButtonImage(_linuxButton, _unselectedSprite);
            break;

            case TerminalType.Linux:
                ChangeButtonImage(_windowsButton, _unselectedSprite);
                ChangeButtonImage(_macButton, _unselectedSprite);
                ChangeButtonImage(_linuxButton, _selectedSprite);
            break;
        }
    }

    private void ChangeButtonImage(Button btn, Sprite spr) {
        btn.GetComponent<Image>().sprite = spr;
    }
}
