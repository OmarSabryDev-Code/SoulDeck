using UnityEngine;

[CreateAssetMenu(menuName = "Cards/JokerCard")]
public class JokerCard : Card
{
    private void OnEnable() => cardType = CardType.Special;

    public override void PlayCard(Player caster, Player opponent = null)
    {
        int roll = Random.Range(0, 4); // 0=attack, 1=heal, 2=freeze, 3=mana
        string resultMessage = "🃏 Joker did nothing...";

        switch (roll)
        {
            case 0: // Attack
                if (opponent != null)
                {
                    opponent.TakeDamage(value);
                    resultMessage = $"🃏 Joker → ATTACK (-{value} HP)";
                }
                break;

            case 1: // Heal
                caster.Heal(value);
                resultMessage = $"🃏 Joker → HEAL (+{value} HP)";
                break;

            case 2: // Freeze
                if (opponent != null)
                {
                    opponent.isFrozen = true; // ensure this bool exists in Player.cs
                    resultMessage = $"🃏 Joker → FREEZE ❄️ {opponent.playerName}";
                }
                break;

            case 3: // Mana
                caster.GainMana(2);
                resultMessage = $"🃏 Joker → +2 MANA!";
                break;
    
        }

        // 🔹 Show result in caster’s UI (PlayerUI already has ShowJokerResult)
        caster.playerUI?.ShowJokerResult(resultMessage);

        // 🔹 Refresh both UIs instantly
        caster.playerUI?.Refresh();
        opponent?.playerUI?.Refresh();

        // 🔹 Force Unity redraw
        Canvas.ForceUpdateCanvases();

        Debug.Log($"{caster.playerName} played Joker: {resultMessage}");

    }

}

