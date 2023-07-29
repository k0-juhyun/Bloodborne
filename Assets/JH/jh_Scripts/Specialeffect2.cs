using UnityEngine;

public class Specialeffect2 : MonoBehaviour
{
    public Transform bossEye; // 보스 눈 위치
    private RectTransform canvasRect; // 캔버스 RectTransform
    private RectTransform particleRect; // 파티클 시스템의 RectTransform

    private void Start()
    {
        canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        particleRect = GetComponent<RectTransform>();
    }

    private void Update()
    {
        // 보스 눈 위치를 월드 좌표에서 캔버스 상의 위치로 변환
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, bossEye.position);
        Vector2 anchoredPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, null, out anchoredPos);

        // 파티클 시스템의 위치를 부드럽게 업데이트
        particleRect.anchoredPosition = Vector2.Lerp(particleRect.anchoredPosition, anchoredPos, Time.deltaTime * 10f);
    }
}
