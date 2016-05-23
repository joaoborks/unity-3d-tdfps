using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class Credits : MonoBehaviour
{
    public Image fader;

    float fadeTimer = 1f;
    bool ending;

    void Update()
    {
        if (!ending && Input.GetButtonDown("Skip"))
            EndCredits();
    }

    void EndCredits()
    {
        ending = true;
        StartCoroutine(FadeCredits());
    }

    IEnumerator FadeCredits()
    {
        float time = 0;
        while (time < fadeTimer)
        {
            time += Time.deltaTime;
            fader.color = Color.black * time / fadeTimer;
            yield return null;
        }
        SceneManager.LoadScene(0);
    }
}
