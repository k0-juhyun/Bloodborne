using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Specialeffect2 : MonoBehaviour
{
    public Transform eyePosition;
    private RectTransform canvasRect; 

    // Start is called before the first frame update
    void Start()
    {
        canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
    }

    void Update()
    {
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, eyePosition.position);
        Vector2 anchoredPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, null, out anchoredPos);

        transform.localPosition = anchoredPos;
    }
}
