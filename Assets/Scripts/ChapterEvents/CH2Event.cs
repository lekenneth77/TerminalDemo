using System.Collections;
using UnityEngine;
using TMPro;


public class CH2Event : MonoBehaviour
{
    [SerializeField] private FileSystem fs;
    [SerializeField] private TextMeshProUGUI _titleText;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("LOL");
        _titleText.text = GameController.Get.TerminalType == TerminalType.Mac ? "Navigating the Terminal - Part 1: pwd, ls" : "Navigating the Terminal - Part 1: cd, dir";
    }

    private IEnumerator LOL() {
        yield return null;
        fs.ForceCD("\\Users\\baggu\\Documents");
    }

}
