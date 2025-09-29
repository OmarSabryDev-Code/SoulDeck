using UnityEngine;

[CreateAssetMenu(menuName = "Cards/DefenseCard")]
public class DefenseCard : Card
{
    [Tooltip("How many turns the block lasts")]
    public int defenseTurns = 1;

    [Tooltip("How much block is granted (damage absorbed each hit until used)")]
    public int blockAmount = 0;

    private void OnEnable() => cardType = CardType.Defense;

    public override void PlayCard(Player caster, Player opponent = null)
    {
        caster.SetDefense(defenseTurns, blockAmount);
        Debug.Log($"{caster.playerName} used {cardName}: +{blockAmount} block for {defenseTurns} turns.");
    }
}
