using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResizeToFitText : MonoBehaviour
{
    public RectTransform rectTransform
    {
        get
        {
            return transform as RectTransform;
        }
    }

    public bool shouldExpandHeight = true;

    private TextMeshProUGUI text_TMP;

    private Vector3 boundsSize = new Vector3(-1, -1, -1);

    private void OnEnable ()
    {
        if (text_TMP == null) text_TMP = GetComponent<TextMeshProUGUI>();
    }

    private void Update ()
    {
        if (boundsSize != text_TMP.bounds.size && text_TMP.bounds.size != Vector3.zero) Resize();
    }
    
    public void Resize ()
    {
        boundsSize = text_TMP.bounds.size;
        
        if (boundsSize == Vector3.zero)
        {
            boundsSize.y = new TMP_TextInfo(text_TMP).lineCount * (text_TMP.fontSize + text_TMP.lineSpacing);
        }
        
        if (shouldExpandHeight)
        {
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, boundsSize.y);
        }
        else
        {
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, boundsSize.z);
        }
    }
}
