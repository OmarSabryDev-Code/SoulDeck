using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class PlayerUI : MonoBehaviour
{
    public TMP_Text playerNameText;
    public TMP_Text healthText;
    public TMP_Text manaText;

    public Player player;
    [SerializeField] private TextMeshProUGUI jokerText; // Drag a UI Text in Inspector



    void Start()
    {
        Refresh();
        jokerText.gameObject.SetActive(false); // hide at start
    }
    
    

    public void Refresh()
    {
        // safety
        if (player == null)
        {
            Debug.LogWarning($"[PlayerUI] Refresh called but player is NULL on PlayerUI GameObject '{gameObject.name}'.");
            return;
        }

        // show debug info (this will tell us if Refresh is called and whether fields are assigned)
        Debug.Log($"[PlayerUI] Refresh() on PlayerUI '{gameObject.name}' for Player '{player.playerName}'. " +
                  $"HP={player.currentHealth}/{player.maxHealth}, Mana={player.currentMana}/{player.maxMana}. " +
                  $"healthTextAssigned={(healthText != null)}, manaTextAssigned={(manaText != null)}, nameTextAssigned={(playerNameText != null)}");

        // Update visible text
        if (playerNameText != null) playerNameText.text = player.playerName;
        if (healthText != null) healthText.text = $"HP: {player.currentHealth}/{player.maxHealth}";
        if (manaText != null) manaText.text = $"Mana: {player.currentMana}/{player.maxMana}";

        // Force Unity to rebuild/refresh UI immediately (helps if Canvas rebuild timing is the issue)
        Canvas.ForceUpdateCanvases();
    }
 public void ShowJokerResult(string message)
    {
        if (jokerText == null) return;

        jokerText.text = message;
        jokerText.gameObject.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(2f);
        jokerText.gameObject.SetActive(false);
    }

}
