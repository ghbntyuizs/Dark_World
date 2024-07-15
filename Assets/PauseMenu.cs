using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu; 

  
    public void Pause()
    {
        pauseMenu.SetActive(true); 
        Time.timeScale = 0;
    }


    public void Home()
    {
        Time.timeScale = 1; 
        SceneManager.LoadScene("MenuController"); 
    }

 
    public void Resume()
    {
        pauseMenu.SetActive(false); 
        Time.timeScale = 1; 
    }

    public void Restart()
    {
        Time.timeScale = 1; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); 
    }
}
