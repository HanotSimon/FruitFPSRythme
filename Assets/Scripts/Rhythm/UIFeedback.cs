using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIFeedback : MonoBehaviour
{
    public static UIFeedback Instance;

    public GameObject perfectImage;
    public GameObject goodImage;
    public GameObject missImage;

    private Coroutine currentRoutine;

    private void Start()
    {
        perfectImage.SetActive(false);
        goodImage.SetActive(false);
        missImage.SetActive(false);
    }

    private void Awake()
    {
        Instance = this;
    }

    public void ShowFeedback(BeatResult result)
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        perfectImage.SetActive(false);
        goodImage.SetActive(false);
        missImage.SetActive(false);

        switch (result)
        {
            case BeatResult.Perfect:
                currentRoutine = StartCoroutine(Show(perfectImage));
                break;

            case BeatResult.Good:
                currentRoutine = StartCoroutine(Show(goodImage));
                break;

            case BeatResult.Miss:
                currentRoutine = StartCoroutine(Show(missImage));
                break;
        }
    }

    IEnumerator Show(GameObject image)
    {
        Debug.Log("Showing feedback: " + image.name);
        image.SetActive(true);

        yield return new WaitForSeconds(1f);

        image.SetActive(false);
    }
}