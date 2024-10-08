using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TerminalTypeSelector : MonoBehaviour
{
    [SerializeField] private Button _windowsButton;
    [SerializeField] private Button _macButton;
    [SerializeField] private Sprite _unselectedSprite;
    [SerializeField] private Sprite _selectedSprite;
    [SerializeField] private PopupWindow _windowsPopup;
    [SerializeField] private Transform _popupContainer;
    [SerializeField] private GameObject _vacuum;
    public static bool SeenWindowsPopup = false;

    // Start is called before the first frame update
    void Start()
    {
        _windowsButton.onClick.AddListener(() => {
            UpdateAllButtons(0);
            if (!SeenWindowsPopup) {
                SeenWindowsPopup = true;
                GameObject popup = Instantiate(_windowsPopup.gameObject, _popupContainer);
                _vacuum.SetActive(true);
                popup.GetComponent<PopupWindow>().OnDismiss += DisableVacuum;
                popup.GetComponent<PopupWindow>().Show();
            }
        });
        _macButton.onClick.AddListener(() => UpdateAllButtons(1));

        GetComponent<CanvasGroup>().alpha = 0;

        UpdateAllButtons((int)TerminalTypeSingleton.Get.terminalType);
        var seq = DOTween.Sequence();
        seq.AppendInterval(0.5f);
        seq.Append(GetComponent<CanvasGroup>().DOFade(1, 1f));
        seq.Play();
    }

    private void DisableVacuum() {
        _vacuum.SetActive(false);
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
