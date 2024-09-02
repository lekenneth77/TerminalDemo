using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class NoEnterButton : MonoBehaviour, ISubmitHandler
{
    public Button button;

    void Start()
    {
        if (button == null)
        {
            button = GetComponent<Button>();
        }
    }

    public void OnSubmit(BaseEventData eventData)
    {
        // Check if the Enter key was pressed
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            
        }
        else
        {
            // Allow normal button behavior for other input sources
            button.onClick.Invoke();
        }
    }
}
