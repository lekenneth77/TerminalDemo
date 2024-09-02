using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CH4Event : MonoBehaviour
{

    private int scenario = 0;
    // Start is called before the first frame update
    void Start()
    {
        GameController.Get.Dialogue.PythonSuccess += HandlePython;
    }

    void OnDestroy()
    {
        GameController.Get.Dialogue.PythonSuccess -= HandlePython;
    }

    private void HandlePython() {
        switch (scenario) {
            case 0:
                GameController.Get.Terminal.DisplayMessage("You've made no money this year. You're broke.");
            break;

            case 1:
            break;

            case 2:
                GameController.Get.Terminal.DisplayMessage("0\n0\n0\n");
            break;
        }
        scenario++;
    }
}
