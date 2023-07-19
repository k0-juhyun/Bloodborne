using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImgFadeOut : MonoBehaviour
{
    public Image image;
    public float fadeOutDuration = 2.0f; // 페이드 아웃에 걸리는 시간

    private void Start()
    {
        StartCoroutine(FadeOutImage());
    }

    private System.Collections.IEnumerator FadeOutImage()
    {
        float elapsedTime = 0f;
        Color originalColor = image.color;
        float startAlpha = originalColor.a; // 현재 알파값 저장

        while (elapsedTime < fadeOutDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, 0f, elapsedTime / fadeOutDuration);
            image.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        image.gameObject.SetActive(false); // 이미지를 비활성화하여 완전히 사라지게 합니다.
    }
}
