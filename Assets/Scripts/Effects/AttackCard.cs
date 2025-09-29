using UnityEngine;

[CreateAssetMenu(menuName = "Cards/AttackCard")]
public class AttackCard : Card
{
    private void OnEnable() => cardType = CardType.Attack;

    public override void PlayCard(Player caster, Player opponent = null)
    {
          if (opponent.hasMirror)
        {
            Debug.Log($"🪞 {opponent.playerName}'s MIRROR reflected {caster.playerName}'s attack!");
            opponent.hasMirror = false;
            caster.TakeDamage(value);
            return;
        }
        if (opponent == null)
        {
            Debug.LogWarning($"[{cardName}] no opponent to attack.");
            return;
        }

        opponent.TakeDamage(value);
        Debug.Log($"{caster.playerName} used {cardName} dealing {value} damage to {opponent.playerName}.");
        
    }
    
}
