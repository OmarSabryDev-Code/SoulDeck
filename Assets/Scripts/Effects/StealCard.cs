using UnityEngine;

[CreateAssetMenu(menuName = "Cards/StealCard")]
public class StealCard : Card
{
    [Tooltip("If true, steals card(s) from opponent's hand. Otherwise steals mana.")]
    public bool stealCard = false;

    private void OnEnable() => cardType = CardType.Steal;

    public override void PlayCard(Player caster, Player opponent = null)
    {
        if (opponent == null) return;

        if (stealCard)
        {
            // Get caster + opponent hand managers
            HandManager casterHand = caster.GetComponent<HandManager>();
            HandManager opponentHand = opponent.GetComponent<HandManager>();
            OpponentHandManager opponentAIHand = opponent.GetComponent<OpponentHandManager>();

            if (opponentHand != null && casterHand != null)
            {
                // Human vs Human (both use HandManager)
                GameObject stolen = opponentHand.StealRandomCardTo(casterHand);
                AnnounceSteal(caster, opponent, stolen);
            }
            else if (opponentAIHand != null && casterHand != null)
            {
                // Human steals from AI
                GameObject stolen = opponentAIHand.StealRandomCardTo(casterHand);
                AnnounceSteal(caster, opponent, stolen);
            }
            else
            {
                Debug.LogWarning("No valid hand managers found for steal effect!");
            }
        }
        else
        {
            // 🔹 Mana steal
            int stealAmount = Mathf.Min(value, opponent.currentMana);
            opponent.currentMana -= stealAmount;
            caster.currentMana += stealAmount;

            caster.playerUI?.Refresh();
            opponent.playerUI?.Refresh();

            Debug.Log($"{caster.playerName} stole {stealAmount} mana from {opponent.playerName}.");
        }
    }

    private void AnnounceSteal(Player caster, Player opponent, GameObject stolen)
    {
        if (stolen != null)
        {
            Debug.Log($"{caster.playerName} stole a card from {opponent.playerName}!");
        }
        else
        {
            Debug.Log($"{opponent.playerName} had no cards to steal.");
        }

        caster.playerUI?.Refresh();
        opponent.playerUI?.Refresh();
    }
}
