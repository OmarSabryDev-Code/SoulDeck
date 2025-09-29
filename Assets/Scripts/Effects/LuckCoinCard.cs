using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cards/LuckyCoin")]
public class LuckyCoinCard : Card
{
    public override void PlayCard(Player caster, Player opponent)
    {
        if (Random.value < 0.5f)
        {
            caster.Heal(caster.maxHealth); // full heal
            Debug.Log($"{caster.playerName} flipped Lucky Coin: FULL HEAL!");
        }
        else
        {
            caster.TakeDamage(5);
            Debug.Log($"{caster.playerName} flipped Lucky Coin: Ouch! -5 HP.");
        }

        caster.playerUI?.Refresh();
    }
}

