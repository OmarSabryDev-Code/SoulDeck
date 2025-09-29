using System.Collections.Generic;
using UnityEngine;

public class DeckSpawner : MonoBehaviour
{
    [Header("Card Prefab")]
    public GameObject cardUIPrefab; // The prefab used for cards

    [Header("Deck (ScriptableObjects)")]
    public List<Card> startingDeck; // Assign all your card ScriptableObjects here

    [Header("References")]
    public HandManager handManager; // Drag your HandManager here in the Inspector

    [Header("Settings")]
    public int maxHandSize = 7; // Maximum number of cards allowed in hand
    public int startingHandSize = 4;  // Cards to draw at game start



    /// <summary>
    /// Draws a single random card from the ScriptableObject deck.
    /// Call this method from your UI Button's OnClick().
    /// </summary>
    public void Start()
    {
        // Draw starting hand
        for (int i = 0; i < startingHandSize; i++)
        {
            DrawCard();
        }
    }
    public void DrawCard()
    {



        // Safety checks
        if (handManager == null)
        {
            Debug.LogWarning("HandManager reference is missing!");
            return;
        }

        if (startingDeck == null || startingDeck.Count == 0)
        {
            Debug.LogWarning("No cards in startingDeck to draw from!");
            return;
        }

        // Check hand limit
        if (handManager.GetHandCount() >= maxHandSize)
        {
            Debug.Log("Hand is full! Cannot draw more cards.");
            return;
        }

        // Pick a random card from the ScriptableObjects deck
        Card randomCard = startingDeck[Random.Range(0, startingDeck.Count)];

        // Instantiate the card prefab at the HandManager's spawn point
        GameObject cardObj = Instantiate(cardUIPrefab, handManager.spawnPoint.position, Quaternion.identity);

        // Assign the ScriptableObject data to the prefab
        CardUI cardUI = cardObj.GetComponent<CardUI>();
        if (cardUI != null)
        {
            cardUI.UpdateCardUI(randomCard);
            cardUI.handManager = handManager;   // ensures the spawned card calls the correct HandManager.PlayCard
            cardUI.owner = CardUI.CardOwner.Player; // if you use owner flags

        }
        else
        {
            Debug.LogError("Card prefab is missing CardUI component!");
        }

        // Let HandManager handle layout, rotation, animations, and touch
        handManager.AddCard(cardObj);

    }
    
}
