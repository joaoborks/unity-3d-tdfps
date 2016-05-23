using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    static bool firstTime = true;

    Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
        string s = firstTime ? "logoFade" : "bgFade";
        FadeAnimation(s);
    }

    void DisableFirstTime()
    {
        firstTime = false;
    }

    void FadeAnimation(string triggerName)
    {
        anim.SetTrigger(triggerName);
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
