using UnityEngine;
using TMPro;
using DG.Tweening;
using System.Collections;

public class TurnManager : MonoBehaviour
{
    public Player player;       // Your Player
    public Player opponent;     // Opponent
    public OpponentAI opponentAI; // auto-assigned in Start()
    private bool hasPlayedCard = false;

    private bool isPlayerTurn = true;

    [Header("UI")]
    public TextMeshProUGUI turnText;

    private void Start()
    {
        // Auto-get OpponentAI from opponent
        if (opponent != null)
        {
            opponentAI = opponent.GetComponent<OpponentAI>();
            if (opponentAI == null)
                Debug.LogError("❌ OpponentAI script missing on Opponent!");
        }
        else
        {
            Debug.LogError("❌ Opponent not assigned in TurnManager!");
        }

        StartPlayerTurn();
    }

    public bool IsPlayerTurn()
    {
        return isPlayerTurn;
    }

    public void PlayerPlayedCard()
    {
        if (hasPlayedCard) return; // Already played a card this turn

        hasPlayedCard = true;
        // Player has played → end turn immediately
        EndPlayerTurn();
    }

    // New method: Mark card as played without ending turn (prevents spam)
    public void MarkCardBeingPlayed()
    {
        hasPlayedCard = true;
    }

    // New method: End turn after card effect is complete
    public void EndTurnAfterCardEffect()
    {
        // Card effect is done, now end the turn
        EndPlayerTurn();
    }

    private void EndPlayerTurn()
    {
        isPlayerTurn = false;
        Debug.Log("🔄 Player ended turn");

        // 🔹 CHECK FREEZE IMMEDIATELY after player's turn ends
        if (opponent.skipNextTurn)
        {
            Debug.Log("❄️ Opponent was frozen this turn - skipping immediately!");
            opponent.skipNextTurn = false; // Reset flag
            
            // Show freeze effect briefly, then return to player
            StartCoroutine(ShowFreezeEffectAndContinue());
            return;
        }

        // If opponent is not frozen, proceed normally
        if (turnText != null)
            turnText.text = "Opponent's Turn";

        hasPlayedCard = false; // Reset for opponent
        StartCoroutine(StartOpponentTurn());
    }

    private IEnumerator ShowFreezeEffectAndContinue()
    {
        // Show freeze effect
        if (turnText != null)
        {
            turnText.DOFade(0, 0);
            turnText.text = "Opponent Frozen!";
            turnText.DOFade(1, 0.5f).SetEase(Ease.OutQuad);
            turnText.transform.DOScale(1.2f, 0.5f).SetLoops(2, LoopType.Yoyo);
        }
        
        yield return new WaitForSeconds(1.5f);
        
        // Go back to player turn
        StartPlayerTurn();
    }

    private void StartPlayerTurn()
    {
        // Check if player is frozen
        if (player.skipNextTurn)
        {
            Debug.Log("❄️ Player's turn was skipped (Freeze effect)!");
            player.skipNextTurn = false; // Reset flag
            EndPlayerTurn();             // Immediately pass turn
            return;
        }

        isPlayerTurn = true;
        hasPlayedCard = false;
        Debug.Log("▶️ Player turn started");

        player?.DrawCard();

        if (turnText != null)
        {
            turnText.DOFade(0, 0);
            turnText.text = "Your Turn";
            turnText.DOFade(1, 0.5f).SetEase(Ease.OutQuad);
            turnText.transform.DOScale(1.2f, 0.5f).SetLoops(2, LoopType.Yoyo);
        }
    }

    private IEnumerator StartOpponentTurn()
    {
        // If we reach this method, opponent is definitely not frozen
        // (freeze check was done in EndPlayerTurn)
        Debug.Log("🤖 Opponent turn started (not frozen)");

        opponent?.DrawCard();

        if (turnText != null)
        {
            turnText.DOFade(0, 0);
            turnText.text = "Opponent's Turn";
            turnText.DOFade(1, 0.5f).SetEase(Ease.OutQuad);
            turnText.transform.DOScale(1.2f, 0.5f).SetLoops(2, LoopType.Yoyo);
        }

        if (opponentAI != null)
        {
            yield return StartCoroutine(opponentAI.TakeTurn(() => { }));
        }

        StartPlayerTurn();
    }

    public Player GetCurrentPlayer()
    {
        return isPlayerTurn ? player : opponent;
    }
    
    public bool HasPlayedThisTurn()
    {
        return hasPlayedCard;
    }

    public void MarkCardPlayed()
    {
        hasPlayedCard = true;
    }
}