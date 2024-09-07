using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDialog : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("test");
    }

    IEnumerator test() {
        yield return null;
        GameController.Get.Terminal.EnableKeyboard();
    }

}
