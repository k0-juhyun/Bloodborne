using UnityEngine;

public class Specialeffect2 : MonoBehaviour
{
    public Transform bossEye; // ���� �� ��ġ
    private RectTransform canvasRect; // ĵ���� RectTransform
    private RectTransform particleRect; // ��ƼŬ �ý����� RectTransform

    private void Start()
    {
        canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        particleRect = GetComponent<RectTransform>();
    }

    private void Update()
    {
        // ���� �� ��ġ�� ���� ��ǥ���� ĵ���� ���� ��ġ�� ��ȯ
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, bossEye.position);
        Vector2 anchoredPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, null, out anchoredPos);

        // ��ƼŬ �ý����� ��ġ�� �ε巴�� ������Ʈ
        particleRect.anchoredPosition = Vector2.Lerp(particleRect.anchoredPosition, anchoredPos, Time.deltaTime * 10f);
    }
}
