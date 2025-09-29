using UnityEngine;

public enum CardType
{
    Attack,
    Heal,
    Defense,
    Steal,
    Protection,
    Freeze,     // new
    Mirror,      // new
    Special     // for Joker effects
}


public abstract class Card : ScriptableObject
{
    [Header("Basic Info")]
    public string cardName = "New Card";
    [TextArea(2,4)] public string description;
    public Sprite cardIcon;
    public int manaCost = 1;
    public int value = 0;


    [Header("Type")]
    public CardType cardType;

    // called when the card is played
    public abstract void PlayCard(Player caster, Player opponent = null);

    // used by UI to show details
    public virtual string GetCardInfo()
    {
        return $"{cardName}: {description}";
    }
}
