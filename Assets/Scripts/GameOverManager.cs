using UnityEngine;

public class GameOverManager : MonoBehaviour
{
    public GameObject gameOverPanel; // Assign a UI Panel in Unity
    public Player player;
    public Player opponent;

    void Update()
    {
        CheckGameOver();
    }

    void CheckGameOver()
    {
        if (player.currentHealth <= 0 || player.currentMana <= 0)
        {
            ShowGameOver("You Lose!");

        }
        else if (opponent.currentHealth <= 0 || opponent.currentMana <= 0)
        {
            ShowGameOver("You Win!");
        }
    }

    void ShowGameOver(string message)
    {
        gameOverPanel.SetActive(true);
        gameOverPanel.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = message;

        //Time.timeScale = 0f; // pause game
    }
    
}
