using UnityEngine;

public class OpponentDeckSpawner : MonoBehaviour
{
    public GameObject cardPrefab;
    public OpponentHandManager opponentHandManager;

    public Card[] allCards;  // Assign opponent deck list in inspector
    public int startingHandSize = 5;

    private void Start()
    {
        for (int i = 0; i < startingHandSize; i++)
        {
            DrawCard();
        }
    }

    public void DrawCard()
    {
        Card randomCard = allCards[Random.Range(0, allCards.Length)];
        GameObject cardObj = Instantiate(cardPrefab, transform.position, Quaternion.identity);
        CardUI cardUI = cardObj.GetComponent<CardUI>();

        cardUI.UpdateCardUI(randomCard);
        cardUI.owner = CardUI.CardOwner.Opponent; // mark card as opponent card
        cardUI.handManager = null; // Opponent cards should NOT be clickable

        opponentHandManager.AddCard(cardObj);
    }
    public GameObject DrawCardPrefab()
    {
        if (allCards == null || allCards.Length == 0)
        {
            Debug.LogWarning("OpponentDeckSpawner.DrawCardPrefab: no cards in deck!");
            return null;
        }

        // Pick a random card from the deck
        Card randomCardData = allCards[Random.Range(0, allCards.Length)];

        // Instantiate the card prefab
        GameObject newCard = Instantiate(cardPrefab);

        // Assign card data to its UI component
        CardUI cardUI = newCard.GetComponent<CardUI>();
        if (cardUI != null)
        {
            cardUI.UpdateCardUI(randomCardData);
            cardUI.owner = CardUI.CardOwner.Opponent; // mark it as opponent card
            cardUI.handManager = null; // opponent cards are not clickable
            cardUI.SetHidden(true);    // hide initially
        }

        return newCard;
    }
}
