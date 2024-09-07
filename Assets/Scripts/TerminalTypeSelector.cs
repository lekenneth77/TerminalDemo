using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TerminalTypeSelector : MonoBehaviour
{
    [SerializeField] private Button _windowsButton;
    [SerializeField] private Button _macButton;
    [SerializeField] private Sprite _unselectedSprite;
    [SerializeField] private Sprite _selectedSprite;

    // Start is called before the first frame update
    void Start()
    {
        _windowsButton.onClick.AddListener(() => UpdateAllButtons(0));
        _macButton.onClick.AddListener(() => UpdateAllButtons(1));

        GetComponent<CanvasGroup>().alpha = 0;

        UpdateAllButtons((int)TerminalTypeSingleton.Get.terminalType);
        var seq = DOTween.Sequence();
        seq.AppendInterval(0.5f);
        seq.Append(GetComponent<CanvasGroup>().DOFade(1, 1f));
        seq.Play();
    }

    private void UpdateAllButtons(int i) {
        switch (i) {
            case 0:
                TerminalTypeSingleton.Get.SetTerminalType(TerminalType.Windows);
                ChangeButtonImage(_windowsButton, _selectedSprite);
                ChangeButtonImage(_macButton, _unselectedSprite);
            break;

            case 1:
                TerminalTypeSingleton.Get.SetTerminalType(TerminalType.Mac);
                ChangeButtonImage(_windowsButton, _unselectedSprite);
                ChangeButtonImage(_macButton, _selectedSprite);
            break;

            case 2:
                TerminalTypeSingleton.Get.SetTerminalType(TerminalType.Mac);
                ChangeButtonImage(_windowsButton, _unselectedSprite);
                ChangeButtonImage(_macButton, _unselectedSprite);
            break;
        }
    }

    private void ChangeButtonImage(Button btn, Sprite spr) {
        btn.GetComponent<Image>().sprite = spr;
    }
}
