using UnityEngine;

[CreateAssetMenu(menuName = "Cards/EnergyDrainCard")]
public class EnergyDrainCard : Card
{
    private void OnEnable() => cardType = CardType.Special; // or Mana/Utility, up to you

    public override void PlayCard(Player caster, Player opponent = null)
    {
        if (opponent == null)
        {
            Debug.LogWarning("❌ No opponent to drain mana from!");
            return;
        }

        // Make sure opponent has at least 1 mana
        if (opponent.currentMana > 0)
        {
            opponent.currentMana -= 3;
            caster.GainMana(3);
            Debug.Log($"{caster.playerName} drained 1 mana from {opponent.playerName}!");
        }
        else
        {
            Debug.Log($"{opponent.playerName} has no mana to drain!");
        }

        // Refresh UI
        caster.playerUI?.Refresh();
        opponent.playerUI?.Refresh();
    }
}
