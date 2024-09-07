using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Pause : MonoBehaviour
{
    public Button pButton;
    public Button goToChpterSeelct;
    private bool isShowing = false;
    void Start() {
        GetComponent<CanvasGroup>().alpha = 0;
        goToChpterSeelct.gameObject.SetActive(false);
        pButton.onClick.AddListener(TogglePause);
        goToChpterSeelct.onClick.AddListener(GoToChpterSelect);
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            TogglePause();
        }
    }

    private void GoToChpterSelect() {
        Time.timeScale = 1;
        LoadingScreen.Get.AnimateIn("ChapterSelect");
    }

    private void TogglePause() {
        if (!GameController.Get || !GameController.Get.AnimDone) {return;}
        if (isShowing) {
            GetComponent<CanvasGroup>().alpha = 0;
            goToChpterSeelct.gameObject.SetActive(false);
            Time.timeScale = 1;
            GameController.Get.Paused = false;
            GameController.Get.FileExplorer.SetVacuumView(false);
        } else {
            GetComponent<CanvasGroup>().alpha = 1;
            Time.timeScale = 0;
            goToChpterSeelct.gameObject.SetActive(true);
            GameController.Get.Paused = true;
            GameController.Get.FileExplorer.SetVacuumView(true);
        }
        isShowing = !isShowing;
    }
}
