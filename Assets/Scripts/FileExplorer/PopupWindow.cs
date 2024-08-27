using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PopupWindow : MonoBehaviour
{
    [SerializeField] private Button _close;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<RectTransform>().localScale = Vector2.zero;
        _close.onClick.AddListener(Dismiss);
    }

    public void Show() {
        var seq = DOTween.Sequence();
        seq.Append(GetComponent<RectTransform>().DOScale(1f, 0.5f));
        seq.Play();
    }

    public void Dismiss() {
        Destroy(gameObject);
    }
}
