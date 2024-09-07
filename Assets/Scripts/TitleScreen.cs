using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;


public class TitleScreen : MonoBehaviour
{

    private const float LETTER_DELAY = 0.2f;
    private const float FADE_IN = 1f;
    private const string TITLE_TEXT = "Terminal_Demo";
    private const string CD_CHPTER = "cd ChapterSelect";
    private const string CD_PRCTICE = "cd Practice";

    public static bool HasSeenTitleScreen;
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TextMeshProUGUI _goToChapterSelectText;
    [SerializeField] private Button _startButton;
    [SerializeField] private Button _practiceButton;
    [SerializeField] private Animator _animation;
    [SerializeField] private CanvasGroup _numbersGroup;
    private bool _transitioning = false;

    // Start is called before the first frame update

    void Awake() {
        _startButton.GetComponent<CanvasGroup>().alpha = 0;
        _practiceButton.GetComponent<CanvasGroup>().alpha = 0;
        _startButton.gameObject.SetActive(false);
        _practiceButton.gameObject.SetActive(false);
        _titleText.text = HasSeenTitleScreen ? "Terminal_Demo" : "";
        _transitioning = false;
    }
    void Start()
    {
        if (LoadingScreen.Get) {
            Destroy(LoadingScreen.Get.gameObject);
        }
        if (HasSeenTitleScreen) {
            ShowButtons();
        } else {
            HasSeenTitleScreen = true;
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
        // _practiceButton.gameObject.SetActive(true);
        _animation.Play("numbers");
        var seq = DOTween.Sequence();
        seq.Append(_startButton.GetComponent<CanvasGroup>().DOFade(1, FADE_IN));
        // seq.Join(_practiceButton.GetComponent<CanvasGroup>().DOFade(1, FADE_IN));
        seq.Join(_numbersGroup.DOFade(1, FADE_IN * 1.5f));
        seq.Play();
    }

    public void StartGame() {
        if (_transitioning) {return;}
        _transitioning = true;
        _numbersGroup.DOFade(0, 2f);
        StartCoroutine("GoToChapterSelect");
    }

    public void GoToPractice() {
        if (_transitioning) {return;}
        _transitioning = true;
        _numbersGroup.DOFade(0, 2f);
        StartCoroutine("PracticeAnim");
    }

    private IEnumerator GoToChapterSelect() {
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
        //load chapterselect
        SceneManager.LoadSceneAsync("ChapterSelect");
    }

    private IEnumerator PracticeAnim() {
        _goToChapterSelectText.text = "> ";
        yield return new WaitForSeconds(LETTER_DELAY / 2f);
        _goToChapterSelectText.text = _goToChapterSelectText.text.Insert(2, "_");
        yield return new WaitForSeconds(LETTER_DELAY / 2f);
        for (int i = 0; i < CD_PRCTICE.Length; i++) {
            _goToChapterSelectText.text = _goToChapterSelectText.text.Insert(i + 2, "" + CD_PRCTICE[i]);
            yield return new WaitForSeconds(LETTER_DELAY / 2f);
        }
        _goToChapterSelectText.text = _goToChapterSelectText.text.Substring(0, _goToChapterSelectText.text.Length - 1);
        yield return new WaitForSeconds(0.5f);
        //load chapterselect
        SceneManager.LoadSceneAsync("PracticeSelect");
    }

}
