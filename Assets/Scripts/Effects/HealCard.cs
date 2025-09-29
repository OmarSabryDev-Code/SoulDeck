using UnityEngine;

[CreateAssetMenu(menuName = "Cards/HealCard")]
public class HealCard : Card
{
    private void OnEnable() => cardType = CardType.Heal;

    public override void PlayCard(Player caster, Player opponent = null)
    {
        if (opponent.hasMirror)
        {
            Debug.Log($"🪞 {opponent.playerName}'s MIRROR reflected {caster.playerName}'s heal!");
            opponent.hasMirror = false;
            opponent.Heal(value);  // Opponent gets healed instead
            return;
        }

        caster.Heal(value);
        Debug.Log($"{caster.playerName} used {cardName} and healed {value} HP.");
    }
}
