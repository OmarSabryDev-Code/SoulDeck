using UnityEngine;

[CreateAssetMenu(menuName = "Cards/ProtectionCard")]
public class ProtectionCard : Card
{
    [Tooltip("How much shield (absorbs damage until depleted)")]
    public int shieldAmount = 0;

    [Tooltip("How many turns the shield persists")]
    public int shieldTurns = 1;

    private void OnEnable() => cardType = CardType.Protection;

    public override void PlayCard(Player caster, Player opponent = null)
    {
        caster.SetProtection(shieldAmount, shieldTurns);
        Debug.Log($"{caster.playerName} used {cardName}: +{shieldAmount} shield for {shieldTurns} turns.");
    }
}
