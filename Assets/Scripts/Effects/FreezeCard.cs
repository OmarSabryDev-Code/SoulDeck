using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Freeze")]
public class FreezeCard : Card
{
    public override void PlayCard(Player caster, Player opponent)
    {
        opponent.skipNextTurn = true; // 🔹 add bool skipNextTurn in Player.cs
        Debug.Log($"{caster.playerName} froze {opponent.playerName}! They skip next turn.");
    }
}

