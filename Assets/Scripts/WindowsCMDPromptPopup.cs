using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WindowsCMDPromptPopup : MonoBehaviour
{
    [SerializeField] private Scrollbar bar;
    // Start is called before the first frame update

    public void SetBar() {
        bar.value = 1;
    }
}
