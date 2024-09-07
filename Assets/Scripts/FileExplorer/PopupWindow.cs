using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PopupWindow : MonoBehaviour
{
    [SerializeField] private Button _close;
    public event Action OnDismiss;
    // Start is called before the first frame update
    void Start()
    {
        
        GetComponent<RectTransform>().localScale = Vector2.zero;
        _close.onClick.AddListener(Dismiss);
    }

    public void Show() {
        
        var seq = DOTween.Sequence();
        seq.Append(GetComponent<RectTransform>().DOScale(1f, 0.5f));
        if (GetComponent<WindowsCMDPromptPopup>()) {
            seq.AppendCallback(GetComponent<WindowsCMDPromptPopup>().SetBar);
        }
        if (GetComponent<WindowsCMDPromptPopup>()) { seq.OnUpdate(() => GetComponent<WindowsCMDPromptPopup>().SetBar());}
        seq.Play();
    }

    public void Dismiss() {
        OnDismiss?.Invoke();
        Destroy(gameObject);
    }
}
