using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;


public class TitleScreen : MonoBehaviour
{

    private const float LETTER_DELAY = 0.2f;
    private const float FADE_IN = 1f;
    private const string TITLE_TEXT = "Terminal_Demo";
    private const string CD_CHPTER = "cd ChapterSelect";
    public static bool HasSeenTitleScreen;
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TextMeshProUGUI _goToChapterSelectText;
    [SerializeField] private Button _startButton;
    [SerializeField] private Animator _animation;
    [SerializeField] private CanvasGroup _numbersGroup;

    // Start is called before the first frame update

    void Awake() {
        _startButton.GetComponent<CanvasGroup>().alpha = 0;
        _startButton.gameObject.SetActive(false);
        _titleText.text = HasSeenTitleScreen ? "Terminal_Demo" : "";
    }
    void Start()
    {
        if (HasSeenTitleScreen) {
            ShowButtons();
        } else {
            StartCoroutine("TypeTitleOut");
        }
    }

    private IEnumerator TypeTitleOut() {
        yield return new WaitForSeconds(0.5f);
        _titleText.text = "_";
        yield return new WaitForSeconds(LETTER_DELAY);
        for (int i = 0; i < TITLE_TEXT.Length; i++) {
            _titleText.text = _titleText.text.Insert(i, "" + TITLE_TEXT[i]);
            yield return new WaitForSeconds(LETTER_DELAY);
        }
        _titleText.text = _titleText.text.Substring(0, _titleText.text.Length - 1);
        yield return new WaitForSeconds(0.5f);
        
        _animation.Play("titleanim");
    }

    private void ShowButtons() {
        _startButton.gameObject.SetActive(true);
        _animation.Play("numbers");
        var seq = DOTween.Sequence();
        seq.Append(_startButton.GetComponent<CanvasGroup>().DOFade(1, FADE_IN));
        seq.Join(_numbersGroup.DOFade(1, FADE_IN * 1.5f));
        seq.Play();
    }

    public void StartGame() {
        StartCoroutine("GoToChapterSelect");
    }

    private IEnumerator GoToChapterSelect() {
        yield return new WaitForSeconds(0.2f);
        _goToChapterSelectText.text = "> ";
        yield return new WaitForSeconds(LETTER_DELAY / 2f);
        _goToChapterSelectText.text = _goToChapterSelectText.text.Insert(2, "_");
        yield return new WaitForSeconds(LETTER_DELAY / 2f);
        for (int i = 0; i < CD_CHPTER.Length; i++) {
            _goToChapterSelectText.text = _goToChapterSelectText.text.Insert(i + 2, "" + CD_CHPTER[i]);
            yield return new WaitForSeconds(LETTER_DELAY / 2f);
        }
        _goToChapterSelectText.text = _goToChapterSelectText.text.Substring(0, _goToChapterSelectText.text.Length - 1);
        yield return new WaitForSeconds(0.5f);
        _goToChapterSelectText.text = "> ";
        yield return new WaitForSeconds(0.1f);
        //load chapterselect
    }

}
