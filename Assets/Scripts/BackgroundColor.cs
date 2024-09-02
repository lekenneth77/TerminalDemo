using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;


public class BackgroundColor : MonoBehaviour
{
    public static BackgroundColor Get;
    [SerializeField] private Image bgImage; 
    void Awake() {
        if (Get) {
            Get.ChangeColors(bgImage.color);
            Destroy(gameObject);
            return;
        } else {
            Get = this;
        }

        DontDestroyOnLoad(gameObject);
    }

    public void ChangeColors(Color tgt) {
        bgImage.DOColor(tgt, 3.0f);
    }
}
