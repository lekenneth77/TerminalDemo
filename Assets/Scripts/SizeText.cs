using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SizeText : MonoBehaviour
{
    public RectTransform thingBeingSized;

    // Start is called before the first frame update
    void Start()
    {
        var text = GetComponent<TextMeshProUGUI>();
        text.ForceMeshUpdate();
        float textWidth = text.GetRenderedValues(false).x;
        float textHeight = thingBeingSized.sizeDelta.y;
        thingBeingSized.sizeDelta = new Vector2(textWidth, textHeight);
    }

    
}
