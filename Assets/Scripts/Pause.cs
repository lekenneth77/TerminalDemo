using UnityEngine;

public class Pause : MonoBehaviour
{
    private bool isShowing = false;
    void Start() {
        GetComponent<CanvasGroup>().alpha = 0;

    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (isShowing) {
                GetComponent<CanvasGroup>().alpha = 0;
                Time.timeScale = 1;
                GameController.Get.Paused = false;
                GameController.Get.FileExplorer.SetVacuumView(false);
            } else {
                GetComponent<CanvasGroup>().alpha = 1;
                Time.timeScale = 0;
                GameController.Get.Paused = true;
                GameController.Get.FileExplorer.SetVacuumView(true);
            }
            isShowing = !isShowing;
        }
    }
}
