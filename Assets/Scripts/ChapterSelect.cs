using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEditor.Rendering;
using UnityEngine.SceneManagement;

public class ChapterSelect : MonoBehaviour
{
    private const string PATH_THING = "ChapterSelect> ";
    private const string LS_TEXT = "ls";
    private const float LETTER_DELAY = 0.2f;
    [SerializeField] private TextMeshProUGUI _terminalText; 
    [SerializeField] private List<Button> _chapterButtons;
    [SerializeField] private List<RectTransform> _chapterLerps;
    private Vector2 _startingPos;
    private bool _animate = false;
    public float animSpeed = 5f;
    private float _animLerp = 0;

    // Start is called before the first frame update
    void Start()
    {
        _terminalText.text = PATH_THING;
        _startingPos = _chapterButtons[0].GetComponent<RectTransform>().anchoredPosition;

        for (int i = 0; i < _chapterButtons.Count; i++) {
            int id = i;
            _chapterButtons[i].onClick.AddListener(() => LoadChapter(id));
        }

        StartCoroutine("ShowLS");
    }

    void Update()
    {
        if (_animate) {
            AnimateButtons();
        }
    }

    private void LoadChapter(int chID) {
        SceneManager.LoadSceneAsync("CH" + chID);
    }

    private IEnumerator ShowLS() {
        yield return new WaitForSeconds(0.1f);
        _terminalText.text = _terminalText.text.Insert(PATH_THING.Length, "_");
        yield return new WaitForSeconds(LETTER_DELAY);
        for (int i = 0; i < LS_TEXT.Length; i++) {
            _terminalText.text = _terminalText.text.Insert(i + PATH_THING.Length, "" + LS_TEXT[i]);
            yield return new WaitForSeconds(LETTER_DELAY);
        }
        _terminalText.text = _terminalText.text.Substring(0, _terminalText.text.Length - 1);
        yield return new WaitForSeconds(0.1f);
        _animate = true;
    }

    private void AnimateButtons() {
        for (int i = 0; i < _chapterButtons.Count; i++) {
            var tgtPos = _chapterLerps[i].anchoredPosition;
            var lerpPos = Vector2.Lerp(_startingPos, tgtPos, _animLerp);
            _chapterButtons[i].GetComponent<RectTransform>().anchoredPosition = lerpPos;
        }


        _animLerp += animSpeed * Time.deltaTime;
        if (_animLerp > 1) {
            _animate = false;
        }
    }
}
