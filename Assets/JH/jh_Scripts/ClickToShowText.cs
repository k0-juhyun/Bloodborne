using UnityEngine;
using UnityEngine.UI;

public class ClickToShowText : MonoBehaviour
{
    public static bool nextScene; 
    public Text textObject;
    public Text talk;
    public string fullText = "Well done good hunter," + '.' +
        "the end of the night is coming."
        ;

    private string[] sentences;
    private int currentSentenceIndex = 0;

    void Start()
    {
        sentences = fullText.Split('.');
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            talk.gameObject.SetActive(false);
            ShowNextSentence();
        }
    }

    void ShowNextSentence()
    {
        if (currentSentenceIndex < sentences.Length)
        {
            textObject.text = sentences[currentSentenceIndex];
            currentSentenceIndex++;
        }
        else
        {
            currentSentenceIndex = 0;
            nextScene = true;
        }
    }
}
