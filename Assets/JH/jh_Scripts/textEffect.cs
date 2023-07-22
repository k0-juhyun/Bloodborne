using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class textEffect : MonoBehaviour
{
    public TMP_Text textMeshPro;
    public float fadeOutDuration = 2.0f; // 페이드 아웃에 걸리는 시간

    private void Start()
    {
        StartCoroutine(FadeOutText());
    }

    private System.Collections.IEnumerator FadeOutText()
    {
        float elapsedTime = 0f;
        Color originalColor = textMeshPro.color;

        while (elapsedTime < fadeOutDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeOutDuration);
            textMeshPro.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        textMeshPro.gameObject.SetActive(false); // 텍스트를 비활성화하여 완전히 사라지게 합니다.
    }
}
