using UnityEngine;

[CreateAssetMenu(fileName = "NewCard", menuName = "Card Game/Card")]
public class CardData : ScriptableObject
{
    public string cardName;
    public CardType cardType;

    [Header("Stats")]
    public int cost = 1;       // Mana cost
    public int power = 2;      // Damage or heal amount

    [Header("Defense")]
    public int defenseTurns = 1; // Only used if cardType == Defense

    [Header("UI")]
    public Sprite artwork;
    public Sprite icon;
}
