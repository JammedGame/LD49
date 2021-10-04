using UnityEngine;
public class IntroPanel : MonoBehaviour
{

    public GameObject Panel;

    void Start()
    {
        Pause();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            if (Panel != null)
            {
                bool isActive = Panel.activeSelf;

                Debug.Log("active:" + isActive);

                Panel.SetActive(!isActive);
                Debug.Log("PauseMenu.GameIsPaused:" + PauseMenu.GameIsPaused);

                if (PauseMenu.GameIsPaused)
                {
                    Debug.Log("Res:");
                    Resume();
                    Debug.Log("Res succ:");
                }
                else
                {
                    Debug.Log("Pause:");
                    Pause();
                    Debug.Log("Pause succ:");

                }
            }
        }
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        PauseMenu.GameIsPaused = false;
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        PauseMenu.GameIsPaused = true;
    }
}