using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CH1Event : MonoBehaviour
{
    [SerializeField] private CanvasGroup _terminalGrp;
    [SerializeField] private CanvasGroup _fileExplorerGrp;

    // Start is called before the first frame update
    void Start()
    {
        GameController.Get.Dialogue.OnDialogueAdvance += HandleDialogueAdvance;
    }

    void OnDestroy() {
        GameController.Get.Dialogue.OnDialogueAdvance -= HandleDialogueAdvance;
    }

    private void HandleDialogueAdvance(int id) {
        if (id == 2) {
            var seq = DOTween.Sequence();
            seq.Append(_terminalGrp.DOFade(1, 1f));
            seq.Play();
        } else if (id == 6) {
            var seq = DOTween.Sequence();
            seq.Append(_fileExplorerGrp.DOFade(1, 1f));
            seq.Play();
        }
    }

    
}
