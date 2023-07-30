using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImgFadeIn : MonoBehaviour
{
    public Image image;
    public float fadeInDuration = 2.0f; // ���̵� �ƿ��� �ɸ��� �ð�

    private void Start()
    {
        StartCoroutine(FadeInImage());
    }

    private System.Collections.IEnumerator FadeInImage()
    {
        float elapsedTime = 0f;
        Color originalColor = image.color;

        while (elapsedTime < fadeInDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 100f, elapsedTime / fadeInDuration);
            image.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }
    }
}
