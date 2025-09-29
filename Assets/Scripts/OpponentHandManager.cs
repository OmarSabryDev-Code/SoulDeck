using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class OpponentHandManager : MonoBehaviour
{
    public List<GameObject> opponentHand = new List<GameObject>();
    public Transform handArea;
    public Transform playArea;
    public Player opponentPlayer;
    public TurnManager turnManager;

    [Header("Hand Layout Settings")]
    public float spreadAngle = 10f;
    public float yOffset = 0f;

    public void AddCard(GameObject card)
    {
        // Hide opponent’s cards
        card.GetComponent<CardUI>().SetHidden(true);
        opponentHand.Add(card);
        card.transform.SetParent(handArea, false);
        UpdateHandLayout();
    }


    public void PlayCard(GameObject card, System.Action onComplete = null)
    {
        // 🔹 Prevent playing on player turn
        if (turnManager != null && turnManager.IsPlayerTurn())
        {
            onComplete?.Invoke();
            return;
        }

        CardUI cardUI = card.GetComponent<CardUI>();
        if (cardUI == null || cardUI.cardData == null)
        {
            Debug.LogWarning("CardUI or cardData missing!");
            onComplete?.Invoke();
            return;
        }

        Card cardData = cardUI.cardData;

        // Get caster/opponent properly
        Player caster = turnManager.GetCurrentPlayer();
        Player opponent = caster.opponent;

        // 🔹 Check mana
        if (caster.currentMana < cardData.manaCost)
        {
            Debug.Log($"{caster.playerName} tried to play {cardData.cardName} but not enough mana!");
            onComplete?.Invoke();
            return;
        }

        // Pay mana
        caster.currentMana -= cardData.manaCost;
        caster.playerUI?.Refresh();

        // Reveal card
        cardUI.SetHidden(false);

        // Remove from hand
        opponentHand.Remove(card);
        UpdateHandLayout();

        // Move card to playArea
        card.transform.SetParent(playArea, true);
        card.transform.DOMove(playArea.position, 0.5f).SetEase(Ease.InOutQuad)
            .OnComplete(() =>
            {
                // Apply actual card effect
                cardData.PlayCard(caster, opponent);

                Debug.Log($"Opponent played {cardData.cardName}");

                // Refresh both UIs
                caster.playerUI?.Refresh();
                opponent.playerUI?.Refresh();

                // Notify that play is finished
                onComplete?.Invoke();
            });
    }


    public void UpdateHandLayout()
    {
        int cardCount = opponentHand.Count;
        if (cardCount == 0) return;

        float totalWidth = Screen.width * 0.8f;
        float spacing = cardCount > 1 ? totalWidth / (cardCount - 1) : 0;
        float startX = -((cardCount - 1) * spacing) / 2f;
        float startAngle = -((cardCount - 1) * -spreadAngle) / 2f;

        for (int i = 0; i < cardCount; i++)
        {
            GameObject card = opponentHand[i];
            Vector3 targetPos = new Vector3(startX + (i * spacing), yOffset, 0);
            float targetRotZ = startAngle + (i * -spreadAngle);

            card.transform.DOLocalMove(targetPos, 0.5f).SetEase(Ease.OutBack);
            card.transform.DOLocalRotate(new Vector3(0, 0, targetRotZ), 0.5f).SetEase(Ease.OutBack);
        }
    }
    public GameObject StealRandomCardTo(HandManager targetHand)
{
    if (opponentHand.Count == 0) return null;

    // Pick random card
    GameObject stolenCard = opponentHand[Random.Range(0, opponentHand.Count)];
    opponentHand.Remove(stolenCard);

    // Add to target hand
    targetHand.AddCardDirect(stolenCard);  // 🔹 custom helper we'll add below

    // Animate movement to player’s hand area
    stolenCard.transform
        .DOMove(targetHand.handArea.position, 0.7f)
        .SetEase(Ease.InOutQuad)
        .OnComplete(() =>
        {
            targetHand.UpdateCardPositions();
            UpdateHandLayout();
        });

    return stolenCard;
}
    
    
    
}
