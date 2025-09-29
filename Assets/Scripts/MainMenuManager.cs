using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Called when "Play" button is clicked
    public void PlayGame()
    {
        // Replace "GameScene" with your actual game scene name
        SceneManager.LoadScene("GameScene");
    }

    // Called when "Exit" button is clicked
    public void QuitGame()
    {
        Debug.Log("Quit button pressed. Exiting game...");
        Application.Quit();

        // Note: In the Editor, Application.Quit() does nothing,
        // so the Debug.Log is useful for testing
    }
}
