using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cards/AdrenalineRush")]
public class AdrenalineRushCard : Card
{
    public override void PlayCard(Player caster, Player opponent)
    {
        caster.currentMana += 3;
        caster.TakeDamage(2);

        Debug.Log($"{caster.playerName} used Adrenaline Rush: +3 Mana, -2 HP!");
        caster.playerUI?.Refresh();
    }
}

