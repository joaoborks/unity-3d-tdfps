using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField]
    CanvasGroup[] groups;

    int activeGroup;

    void Update()
    {
        if (activeGroup == 0 && Input.GetButtonDown("Pause"))
            TogglePause();
    }

    void TogglePause()
    {
        CanvasGroup group = groups[activeGroup];
        if (group.alpha == 0)
        {
            group.alpha = 1;
            group.interactable = true;
            group.blocksRaycasts = true;
            Time.timeScale = 0;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            group.alpha = 0;
            group.interactable = false;
            group.blocksRaycasts = false;
            Time.timeScale = 1;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void SetActive(int index)
    {
        activeGroup = index;
        TogglePause();
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1;
    }

    public void ToMenu()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 1;
    }

    public void ToCredits()
    {
        SceneManager.LoadScene(2);
        Time.timeScale = 1;
    }

    public void Quit()
    {
        Application.Quit();
    }
}