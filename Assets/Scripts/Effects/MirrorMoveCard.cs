using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Cards/MirrorMove")]
public class MirrorMoveCard : Card
{
    private void OnEnable() => cardType = CardType.Protection; // or new Mirror type if you want

    public override void PlayCard(Player caster, Player opponent)
    {
        caster.hasMirror = true;   // not mirrorNext
        Debug.Log($"{caster.playerName} prepared Mirror Move! Next attack/heal will bounce back.");
    }
}


