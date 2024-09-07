using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class ChapterSelect : MonoBehaviour
{
    private const string PATH_THING = "ChapterSelect> ";
    private const string LS_TEXT = "ls";
    private const string DIR_TEXT = "dir";
    private const float LETTER_DELAY = 0.2f;
    [SerializeField] private TextMeshProUGUI _terminalText; 
    [SerializeField] private List<Button> _chapterButtons;
    [SerializeField] private List<RectTransform> _chapterLerps;
    [SerializeField] private GameObject _vacuum;
    [SerializeField] private Button _titleScreenButton;
    [SerializeField] private TerminalTypeSelector _typeSelector;
    private Vector2 _startingPos;
    private bool _animate = false;
    public float animSpeed = 5f;
    private float _animLerp = 0;
    private bool _doneAnimation = false;
    private bool _goingTitleScreen = false;

    // Start is called before the first frame update
    void Start()
    {
        _terminalText.text = PATH_THING;
        _startingPos = _chapterButtons[0].GetComponent<RectTransform>().anchoredPosition;
        _doneAnimation = false;

        if (BackgroundColor.Get) {
            Destroy(BackgroundColor.Get.gameObject);
        }

        for (int i = 0; i < _chapterButtons.Count; i++) {
            int id = i;
            _chapterButtons[i].onClick.AddListener(() => LoadChapter(id));
        }
        TerminalTypeSingleton.Get.ChangedType += UpdateListCommand;
        if (LoadingScreen.Get.dismissed) {
            StartCoroutine("ShowLS");
        } else {
            var seq = DOTween.Sequence();
            seq.AppendCallback(() => LoadingScreen.Get.Dismiss());
            seq.AppendInterval(0.25f);
            seq.AppendCallback(() => StartCoroutine("ShowLS"));
            seq.Play();
        }

        _titleScreenButton.onClick.AddListener(GoToTitleScreen);
        CanvasGroup titleScreenBtnGrp = _titleScreenButton.GetComponent<CanvasGroup>();
        titleScreenBtnGrp.alpha = 0;
        var sequence = DOTween.Sequence();
        sequence.AppendInterval(0.5f);
        sequence.Append(titleScreenBtnGrp.DOFade(1, 1f));
        sequence.Play();
    }

    void OnDestroy() {
        TerminalTypeSingleton.Get.ChangedType -= UpdateListCommand;
    }

    void Update()
    {
        if (_animate) {
            AnimateButtons(!_goingTitleScreen);
        }
    }

    private void GoToTitleScreen() {
        if (!_doneAnimation) {return;}
        _goingTitleScreen = true;
        
        StartCoroutine("GoTitleScreenAnimation");
    }

    private IEnumerator GoTitleScreenAnimation() {
        _terminalText.text = PATH_THING;
        yield return new WaitForSeconds(0.1f);
        _terminalText.text = _terminalText.text.Insert(PATH_THING.Length, "_");
        yield return new WaitForSeconds(LETTER_DELAY);
        string text = "cd ..";
        for (int i = 0; i < text.Length; i++) {
            _terminalText.text = _terminalText.text.Insert(i + PATH_THING.Length, "" + text[i]);
            yield return new WaitForSeconds(LETTER_DELAY);
        }
        _terminalText.text = _terminalText.text.Substring(0, _terminalText.text.Length - 1);
        yield return new WaitForSeconds(0.1f);
        CanvasGroup titleScreenBtnGrp = _titleScreenButton.GetComponent<CanvasGroup>();
        CanvasGroup selectorGrp = _typeSelector.GetComponent<CanvasGroup>();
        titleScreenBtnGrp.alpha = 1;
        selectorGrp.alpha = 1;
        var sequence = DOTween.Sequence();
        sequence.Append(titleScreenBtnGrp.DOFade(0, 0.25f));
        sequence.Join(selectorGrp.DOFade(0, 0.25f));
        sequence.AppendInterval(0.3f);
        sequence.OnComplete(() => {SceneManager.LoadSceneAsync("TitleScreen");});
        sequence.Play();
        _animate = true;
    }

    private void UpdateListCommand() {
        var type = TerminalTypeSingleton.Get.terminalType;
        _terminalText.text = PATH_THING + (type == TerminalType.Mac ? LS_TEXT : DIR_TEXT);
    }

    private void LoadChapter(int chID) {
        if (_goingTitleScreen) {return;}
        _vacuum.SetActive(true);
        string name = "CH" + chID;
        LoadingScreen.Get.AnimateIn(name);
    }

    private IEnumerator ShowLS() {
        yield return new WaitForSeconds(0.1f);
        _terminalText.text = _terminalText.text.Insert(PATH_THING.Length, "_");
        yield return new WaitForSeconds(LETTER_DELAY);
        string text = TerminalTypeSingleton.Get.terminalType == TerminalType.Windows ? DIR_TEXT : LS_TEXT;
        for (int i = 0; i < text.Length; i++) {
            _terminalText.text = _terminalText.text.Insert(i + PATH_THING.Length, "" + text[i]);
            yield return new WaitForSeconds(LETTER_DELAY);
        }
        _terminalText.text = _terminalText.text.Substring(0, _terminalText.text.Length - 1);
        yield return new WaitForSeconds(0.1f);
        _animate = true;
        _doneAnimation = true;
    }

    private void AnimateButtons(bool goingToTgt) {
        if (goingToTgt) {
            for (int i = 0; i < _chapterButtons.Count; i++) {
                var tgtPos = _chapterLerps[i].anchoredPosition;
                var lerpPos = Vector2.Lerp(_startingPos, tgtPos, _animLerp);
                _chapterButtons[i].GetComponent<RectTransform>().anchoredPosition = lerpPos;
            }
        } else {
            for (int i = 0; i < _chapterButtons.Count; i++) {
                var tgtPos = _chapterLerps[i].anchoredPosition;
                var lerpPos = Vector2.Lerp(tgtPos, _startingPos, _animLerp);
                _chapterButtons[i].GetComponent<RectTransform>().anchoredPosition = lerpPos;
            }
        }

        _animLerp += animSpeed * Time.deltaTime;
        if (_animLerp > 1) {
            _animate = false;
            _animLerp = 0;

            if (!goingToTgt) {
                //
            }
        }
    }
}
