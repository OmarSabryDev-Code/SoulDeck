using System.Collections;
using UnityEngine;

public class OpponentAI : MonoBehaviour
{
    public OpponentDeckSpawner deckSpawner;       // Must be assigned in Inspector
    public OpponentHandManager opponentHandManager; // Must be assigned in Inspector

    /// <summary>
    /// Draws a single card from the deck and adds it to the opponent's hand.
    /// Works exactly like Player.DrawCard()
    /// </summary>
    public void DrawCard()
    {
        if (opponentHandManager == null)
        {
            Debug.LogError("OpponentAI.DrawCard failed: opponentHandManager not assigned!");
            return;
        }

        if (deckSpawner == null)
        {
            Debug.LogError("OpponentAI.DrawCard failed: deckSpawner not assigned!");
            return;
        }

        // Spawn a card from deck
        GameObject newCard = deckSpawner.DrawCardPrefab(); // Make sure this returns a valid card prefab
        if (newCard == null)
        {
            Debug.LogWarning("OpponentAI.DrawCard: deckSpawner returned null card prefab!");
            return;
        }

        // Add it to the hand (this handles layout & hidden visuals)
        opponentHandManager.AddCard(newCard);

        Debug.Log("🤖 Opponent drew a card!");
    }

    // Called by TurnManager
    public IEnumerator TakeTurn(System.Action onComplete)
    {
        if (opponentHandManager == null)
        {
            Debug.LogError("OpponentHandManager not assigned!");
            onComplete?.Invoke();
            yield break;
        }

        Debug.Log("🤖 Opponent turn started");

        // Draw a card automatically at the start of the turn
        DrawCard();

        yield return new WaitForSeconds(0.5f); // Small delay for realism

        if (opponentHandManager.opponentHand.Count > 0)
        {
            // Pick a random card
            GameObject cardToPlay = opponentHandManager.opponentHand[
                Random.Range(0, opponentHandManager.opponentHand.Count)
            ];

            bool cardPlayed = false;

            // Play card with callback
            opponentHandManager.PlayCard(cardToPlay, () =>
            {
                cardPlayed = true;
            });

            // Wait until card is actually played (animation + effect)
            yield return new WaitUntil(() => cardPlayed);
        }
        else
        {
            Debug.Log("Opponent has no cards to play");
        }

        // Notify TurnManager that AI finished
        onComplete?.Invoke();
    }
}
