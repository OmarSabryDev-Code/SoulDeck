using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartButton : MonoBehaviour
{
    
    public void RestartGame()
    {
        // Reset time scale in case the game was paused
        //Time.timeScale = 1f;

        // Reload the currently active scene
        SceneManager.LoadScene("GameScene");
    }
}
