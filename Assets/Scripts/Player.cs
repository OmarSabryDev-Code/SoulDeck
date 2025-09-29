using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Player")]
    public string playerName = "Player";
    public int maxHealth = 20;
    public int currentHealth;

    [Header("Defense / Protection")]
    public int blockValue = 0;            // temporary block amount (absorbed before HP)
    public int blockTurnsRemaining = 0;
    public int shieldValue = 0;           // persistent shield amount (absorbs until depleted)
    public int shieldTurnsRemaining = 0;

    [Header("Steal Protection")]
    public int stealProtectionTurns = 0;

    [Header("Opponent")]
    public Player opponent; // assign in inspector or at runtime

    [Header("Resources")]
    public int maxMana = 30;
    public int currentMana;
    
    public PlayerUI playerUI; // assign in inspector
    public HandManager handManager;    // Reference to player's hand
    public DeckSpawner deckSpawner;    // Reference to player's deck
     public GameObject cardPrefab;      // assign in inspector
    [Header("Gameplay")]
    public bool isHuman = false; // set true in Inspector for the local player
    

public bool skipNextTurn = false;
    public bool mirrorNext = false;
 [Header("Status Effects")]
    public bool hasMirror = false;   // 🪞 reflects the next effect
    public bool isFrozen = false;    // ❄️ skip next turn (if you want Freeze later)
    



    void Start()
{
    currentMana = maxMana;   // start game full mana
    if (playerUI != null)
        playerUI.Refresh();
}

    

    void Awake()
    {
        // Auto-find UI if not assigned
    if (playerUI == null)
    {
        playerUI = GetComponentInChildren<PlayerUI>();
        if (playerUI == null)
            Debug.LogWarning($"{playerName} has no PlayerUI assigned!");
    }
        currentHealth = maxHealth;
        currentMana = Mathf.Min(maxMana, currentMana + 1); // or +X regen per turn

        if (playerUI == null)
            playerUI = GetComponent<PlayerUI>();

        if (playerUI != null)
            playerUI.Refresh();
    }
    public void DrawCard()
{
    if (deckSpawner != null)
    {
        deckSpawner.DrawCard(); // Use the proper DrawCard() from DeckSpawner
        Debug.Log("Player drew a card from ScriptableObject deck!");
    }
    else
    {
        Debug.LogError("Player DeckSpawner not assigned!");
    }
}

    public void TakeDamage(int amount)
    {
        int remaining = amount;

        if (blockValue > 0 && blockTurnsRemaining > 0)
        {
            int absorbed = Mathf.Min(blockValue, remaining);
            blockValue -= absorbed;
            remaining -= absorbed;
            Debug.Log($"{playerName} blocked {absorbed} damage (block left: {blockValue}).");
        }

        if (shieldValue > 0 && shieldTurnsRemaining > 0)
        {
            int absorbed = Mathf.Min(shieldValue, remaining);
            shieldValue -= absorbed;
            remaining -= absorbed;
            Debug.Log($"{playerName}'s shield absorbed {absorbed} damage (shield left: {shieldValue}).");
        }

        if (remaining > 0)
        {
            currentHealth -= remaining;
            currentHealth = Mathf.Max(0, currentHealth);
            Debug.Log($"{playerName} takes {remaining} damage. HP: {currentHealth}/{maxHealth}");
        }

        if (playerUI != null) playerUI.Refresh(); // 🔹 update UI
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        Debug.Log($"{playerName} healed {amount}. HP: {currentHealth}/{maxHealth}");

        if (playerUI != null) playerUI.Refresh(); // 🔹 update UI
    }

    public void SetDefense(int turns, int blockAmount = 0)
    {
        if (blockAmount > 0)
            blockValue += blockAmount;

        blockTurnsRemaining = Mathf.Max(blockTurnsRemaining, turns);

        if (playerUI != null) playerUI.Refresh(); // 🔹 update UI
    }

    public void SetProtection(int shieldAmount, int turns)
    {
        shieldValue += shieldAmount;
        shieldTurnsRemaining = Mathf.Max(shieldTurnsRemaining, turns);

        if (playerUI != null) playerUI.Refresh(); // 🔹 update UI
    }

    public void SetStealProtection(int turns)
    {
        stealProtectionTurns = Mathf.Max(stealProtectionTurns, turns);

        if (playerUI != null) playerUI.Refresh(); // 🔹 update UI
    }
    public void GainMana(int amount)
{
    currentMana = Mathf.Min(currentMana + amount, maxMana); // cap at maxMana
    playerUI?.Refresh();
}

    public void EndTurnTick()
    {
        if (blockTurnsRemaining > 0)
        {
            blockTurnsRemaining--;
            if (blockTurnsRemaining <= 0) blockValue = 0;
        }

        if (shieldTurnsRemaining > 0)
        {
            shieldTurnsRemaining--;
            if (shieldTurnsRemaining <= 0) shieldValue = 0;
        }

        if (stealProtectionTurns > 0)
            stealProtectionTurns--;

        if (playerUI != null) playerUI.Refresh(); // 🔹 update UI
    }

    public void StartTurn()
{
    currentMana = maxMana;   // reset to full
    if (playerUI != null)
        playerUI.Refresh();

    Debug.Log($"{playerName} turn started → Mana reset to {currentMana}/{maxMana}");
}


}
