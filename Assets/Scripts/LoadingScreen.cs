using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private RectTransform rect;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI loadingText;
    public static LoadingScreen Get;
    private const float FADE_SPEED = 1.5f;
    private const float LOADING_TEXT_SPEED = 0f;
    private const float PERIOD_SPEED = 0.5f;
    [HideInInspector] public bool dismissed = true;
    void Awake()
    {
        if (Get) {
            Destroy(gameObject);
            return;
        } else {
            Get = this;
        }

        dismissed = true;
        DontDestroyOnLoad(gameObject);
        gameObject.SetActive(false);
    }

    public void AnimateIn(string sceneName) {
        canvasGroup.alpha = 0;
        gameObject.SetActive(true);
        loadingText.text = "";
        var seq = DOTween.Sequence();
        seq.Append(canvasGroup.DOFade(1, FADE_SPEED));
        seq.AppendCallback(() => StartCoroutine(TextAnimate(sceneName)));
        seq.Play();
    }

    private IEnumerator TextAnimate(string sceneName) {
        yield return new WaitForSeconds(0.1f);
        string loading = "Loading";
        // for (int i = 0; i < loading.Length; i++) {
        //     loadingText.text += loading[i];
        //     yield return new WaitForSeconds(LOADING_TEXT_SPEED);
        // }
        // yield return new WaitForSeconds(0.1f);
        yield return new WaitForSeconds(PERIOD_SPEED);
        loadingText.text = loading;
        for (int i = 0; i < 3; i++) {
            yield return new WaitForSeconds(PERIOD_SPEED);
            loadingText.text += '.';
        }
        yield return new WaitForSeconds(PERIOD_SPEED);
        //load screen
        dismissed = false;
        SceneManager.LoadSceneAsync(sceneName);
    }

    public void Dismiss()
    {
        //dismiss screen
        loadingText.text = "";
        var seq = DOTween.Sequence();
        canvasGroup.alpha = 1;
        seq.Append(canvasGroup.DOFade(0, FADE_SPEED / 2f));
        seq.OnComplete(() => {
            gameObject.SetActive(false);
            dismissed = true;
        });
        seq.Play();
    }

}
