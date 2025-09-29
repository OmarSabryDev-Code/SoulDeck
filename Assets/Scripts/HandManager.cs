using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class HandManager : MonoBehaviour
{
    [Header("Hand Setup")]
    public Transform handArea;          // The PlayerHandArea panel
    public Transform spawnPoint;        // Off-screen spawn point
    public float spacing = 120f;        // Horizontal distance between cards
    public float fanAngle = -10f;        // Max angle for fanning
    public float animationTime = 0.5f;  // Animation speed
    public Transform playArea; // Assign in Inspector
    public Player targetPlayer; // assign in Inspector for now
    public PlayerUI playerUI; // drag in inspector
    public Player opponentPlayer; // assign in Inspector for now
    public PlayerUI opponentUI; // drag in inspector
    public TurnManager turnManager; // drag & drop in Inspector
    



    private List<GameObject> cardsInHand = new List<GameObject>();

    // Call this when a new card is drawn
    public void AddCard(GameObject cardOrPrefab)
    {
        GameObject newCard;

        // If the passed GameObject is already in the scene (instantiated), use it directly.
        if (cardOrPrefab != null && cardOrPrefab.scene.IsValid())
        {
            newCard = cardOrPrefab;
            // ensure it's parented into the hand area
            newCard.transform.SetParent(handArea, false);
        }
        else
        {
            // otherwise instantiate a new one from the prefab
            newCard = Instantiate(cardOrPrefab, spawnPoint.position, Quaternion.identity, handArea);
        }

        // Add to list
        cardsInHand.Add(newCard);

        // Ensure CardUI knows its hand manager (important for later interactions)
        CardUI ui = newCard.GetComponent<CardUI>();
        if (ui != null)
        {
            ui.handManager = this;
            // optional: set owner flags if you use them
            // ui.owner = CardUI.CardOwner.Player; // set as needed in inspector if you track ownership
        }

        // Rearrange
        UpdateCardPositions();
    }

    // Rearrange with fanning
    public void UpdateCardPositions()
    {
        int cardCount = cardsInHand.Count;
        if (cardCount == 0) return;

        // Calculate dynamic spacing based on screen width
        float totalWidth = Screen.width * 0.8f; // use 80% of screen width
        float spacing = cardCount > 1 ? totalWidth / (cardCount - 1) : 0;

        // Center offset
        float startX = -((cardCount - 1) * spacing) / 2f;

        // Angle offset (so cards fan)
        float startAngle = -((cardCount - 1) * fanAngle) / 2f;

        for (int i = 0; i < cardCount; i++)
        {
            GameObject card = cardsInHand[i];

            // Target position
            Vector3 targetPos = new Vector3(startX + (i * spacing), 0, 0);

            // Target rotation
            float targetRotZ = startAngle + (i * fanAngle);

            // Animate move
            card.transform.DOLocalMove(targetPos, animationTime).SetEase(Ease.OutBack);

            // Animate rotation
            card.transform.DOLocalRotate(new Vector3(0, 0, targetRotZ), animationTime).SetEase(Ease.OutBack);
        }
    }
    public void PlayCard(GameObject cardObject)
{
    if (!turnManager.IsPlayerTurn())
    {
        Debug.Log("❌ Not your turn!");
        return;
    }

    if (turnManager.HasPlayedThisTurn())
    {
        Debug.Log("❌ Already played a card this turn!");
        return;
    }

    // 1) Ensure card is actually in hand
    if (!cardsInHand.Contains(cardObject)) return;

    // 2) Quick sanity: get CardUI & cardData
    CardUI cardUI = cardObject.GetComponent<CardUI>();
    if (cardUI == null || cardUI.cardData == null)
    {
        Debug.LogWarning("PlayCard: missing CardUI or cardData.");
        return;
    }

    // 3) Identify whose turn it is
    Player caster = turnManager.GetCurrentPlayer();
    Player opponent = (caster == targetPlayer) ? opponentPlayer : targetPlayer;

    // 4) Check mana BEFORE removing/animating
    if (caster.currentMana < cardUI.cardData.manaCost)
    {
        Debug.Log($"{caster.playerName} does not have enough mana to play {cardUI.cardData.cardName}!");
        return;
    }

    // 5) IMMEDIATELY mark that a card is being played (prevents spam)
    turnManager.MarkCardBeingPlayed();

    // 6) Remove from hand list and move to play area
    cardsInHand.Remove(cardObject);
    cardObject.transform.SetParent(playArea, true);

    // 7) Animate move then apply effect in OnComplete
    cardObject.transform.DOMove(playArea.position, animationTime)
        .SetEase(Ease.InOutQuad)
        .OnComplete(() =>
        {
            // Disable raycasts so the card can't be clicked again
            CanvasGroup cg = cardObject.GetComponent<CanvasGroup>();
            if (cg != null)
            {
                cg.interactable = false;
                cg.blocksRaycasts = false;
            }

            // 🔹 Debug BEFORE mana subtraction
            Debug.Log($"[Mana Debug] {caster.playerName} mana BEFORE = {caster.currentMana}, cost = {cardUI.cardData.manaCost}");

            // Pay mana
            caster.currentMana -= cardUI.cardData.manaCost;

            // 🔹 Debug AFTER mana subtraction
            Debug.Log($"[Mana Debug] {caster.playerName} mana AFTER = {caster.currentMana}");

            caster.playerUI?.Refresh();

            // APPLY CARD EFFECT - this must happen BEFORE ending the turn!
            cardUI.cardData.PlayCard(caster, opponent);

            // Refresh both UIs
            caster.playerUI?.Refresh();
            opponent.playerUI?.Refresh();

            // 🔹 CRITICAL: End turn AFTER card effect is applied
            turnManager.EndTurnAfterCardEffect();
        });

    // Re-arrange/fan remaining cards
    UpdateCardPositions();
    
    // 🚨 REMOVED: Don't call this here! It needs to be in OnComplete()
    // turnManager.PlayerPlayedCard();
}


/// <summary>
    /// Steal a random card from THIS hand and move it into targetHand.
    /// Returns the stolen GameObject (null if none).
    /// revealToTarget: if true, reveal the card face-up for the receiving hand (useful when the player steals).
    /// </summary>
    public GameObject StealRandomCardTo(HandManager targetHand, bool revealToTarget = true)
    {
        Debug.Log("[STEAL] StealRandomCardTo called");

        if (targetHand == null)
        {
            Debug.LogWarning("[STEAL] targetHand is null!");
            return null;
        }

        if (cardsInHand.Count == 0)
        {
            Debug.Log("[STEAL] No cards to steal.");
            return null;
        }

        // Pick a random card
        int idx = Random.Range(0, cardsInHand.Count);
        GameObject stolenCard = cardsInHand[idx];

        // Remove from source list immediately
        cardsInHand.RemoveAt(idx);

        // Attempt to set hidden/revealed state on CardUI (if that method exists)
        CardUI stolenUI = stolenCard.GetComponent<CardUI>();
        if (stolenUI != null)
        {
            // reveal to the receiver only if requested
            // Assume CardUI has SetHidden(bool) — you already call SetHidden elsewhere
            stolenUI.SetHidden(!revealToTarget);
        }

        // Detach temporarily so world position is preserved and the card can fly across UI
        stolenCard.transform.SetParent(null, true);

        // Animate flying to the target hand area
        stolenCard.transform.DOMove(targetHand.handArea.position, 0.7f)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() =>
            {
                // Parent to the target hand area, add to its list and update layouts
                stolenCard.transform.SetParent(targetHand.handArea, false);
                targetHand.cardsInHand.Add(stolenCard);

                targetHand.UpdateCardPositions();
                UpdateCardPositions();

                Debug.Log("[STEAL] Steal animation complete.");
            });

        return stolenCard;
    }

    /// <summary>
    /// Return a specific card from this hand back to targetHand (reverse flow).
    /// If cardToReturn is null it will do nothing.
    /// </summary>
    public void GiveCardBackTo(HandManager targetHand, GameObject cardToReturn)
    {
        if (targetHand == null || cardToReturn == null) return;

        // Remove from this list if present
        if (cardsInHand.Contains(cardToReturn))
            cardsInHand.Remove(cardToReturn);

        // detach and animate back
        cardToReturn.transform.SetParent(null, true);

        cardToReturn.transform.DOMove(targetHand.handArea.position, 0.7f)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() =>
            {
                cardToReturn.transform.SetParent(targetHand.handArea, false);
                targetHand.cardsInHand.Add(cardToReturn);
                targetHand.UpdateCardPositions();
                UpdateCardPositions();
                Debug.Log("[STEAL] Card returned to original owner.");
            });
    }
    public void AddCardDirect(GameObject card)
{
    // Used when another manager (like OpponentHandManager) gives us a card
    card.transform.SetParent(handArea, true);
    cardsInHand.Add(card);
    UpdateCardPositions();
}







    public void MoveToPlayArea(GameObject card)
    {
        if (playArea != null)
        {
            card.transform.SetParent(playArea, true);
            card.transform.position = playArea.position;
        }
    }
    // HandManager.cs
    public int GetHandCount()
    {
        return cardsInHand.Count;
    }
    public GameObject GetCardAt(int index)
    {
        if (index >= 0 && index < cardsInHand.Count)
            return cardsInHand[index];
        return null;
    }
    public List<GameObject> GetAllCards()
    {
        return new List<GameObject>(cardsInHand);
    }
public void RemoveCard(GameObject card)
{
    if (cardsInHand.Contains(card))
    {
        cardsInHand.Remove(card);
    }
}





}
