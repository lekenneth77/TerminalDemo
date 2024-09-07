using DG.Tweening;
using UnityEngine;

public class CheckmarkAnim : MonoBehaviour
{
    [SerializeField] private CanvasGroup _checkMarkGrp;
    [SerializeField] private RectTransform _checkMarkRect;
    private const float ANIM_DURATION = 2f;

    // Start is called before the first frame update
    void Start()
    {
        _checkMarkGrp.alpha = 1;
        _checkMarkRect.localScale = Vector2.zero;
    }

    public void AnimateCheckmark() {
        _checkMarkGrp.alpha = 1;
        _checkMarkRect.localScale = Vector2.zero;
        var seq = DOTween.Sequence();
        seq.Append(_checkMarkGrp.DOFade(0, ANIM_DURATION));
        seq.Join(_checkMarkRect.DOScale(1, ANIM_DURATION));
        seq.AppendCallback(() => {_checkMarkRect.localScale = Vector2.zero; _checkMarkGrp.alpha = 1;});
        seq.Play();
    }

}
