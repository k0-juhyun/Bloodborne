using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class textEffect : MonoBehaviour
{
    public TMP_Text textMeshPro;
    public float fadeOutDuration = 2.0f; // ���̵� �ƿ��� �ɸ��� �ð�

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

        textMeshPro.gameObject.SetActive(false); // �ؽ�Ʈ�� ��Ȱ��ȭ�Ͽ� ������ ������� �մϴ�.
    }
}
