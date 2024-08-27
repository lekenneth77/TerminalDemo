using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class File : MonoBehaviour
{
    [SerializeField] private PopupWindow popup;

    public void ShowPopup()
    {
        var pop = Instantiate(popup, GameController.Get.PopupContainer);
        pop.GetComponent<PopupWindow>().Show();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

}
