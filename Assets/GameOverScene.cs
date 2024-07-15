using UnityEngine;
using UnityEngine.SceneManagement;


public class GameOverScene : MonoBehaviour
{
    public GameObject gameOverUI;

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }
    public void gameOver()
    {
        gameOverUI.SetActive(true);
    }
}
