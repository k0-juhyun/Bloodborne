using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImgFadeOut : MonoBehaviour
{
    public Image image;
    public float fadeOutDuration = 2.0f; // ���̵� �ƿ��� �ɸ��� �ð�

    private void Start()
    {
        StartCoroutine(FadeOutImage());
    }

    private System.Collections.IEnumerator FadeOutImage()
    {
        float elapsedTime = 0f;
        Color originalColor = image.color;
        float startAlpha = originalColor.a; // ���� ���İ� ����

        while (elapsedTime < fadeOutDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, 0f, elapsedTime / fadeOutDuration);
            image.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        image.gameObject.SetActive(false); // �̹����� ��Ȱ��ȭ�Ͽ� ������ ������� �մϴ�.
    }
}
