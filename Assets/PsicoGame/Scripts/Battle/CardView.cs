using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardView : MonoBehaviour
{
    public Image art;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI effectsText;
    private CardSO _card;
    private BattleController _controller;

    public void Bind(CardSO card, BattleController controller)
    {
        _card = card; _controller = controller;
        titleText.text = card.DisplayName;
        // effectsText opcional: puedes construir un string con +/? resumidos
        effectsText.text = BuildSummary(card);
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    void OnClick() => _controller.PlayCard(_card);

    string BuildSummary(CardSO c)
    {
        // Simple y útil para testing
        // Ej: "+Heal 20 | -SelfDmg 10"
        System.Text.StringBuilder sb = new();
        foreach (var e in c.PositiveEffects) sb.Append($"+{e.Type} {e.Magnitude}  ");
        foreach (var e in c.NegativeEffects) sb.Append($"-{e.Type} {e.Magnitude}  ");
        return sb.ToString();
    }
}
