using UnityEngine;
using TMPro;

public class DeckInfoView : MonoBehaviour
{
    public BattleController controller;
    public TextMeshProUGUI drawText, discardText;

    void OnEnable()
    {
        controller.Events.OnHandChanged += Refresh;
        controller.Events.OnLog += _ => Refresh();
        Refresh();
    }
    void OnDisable()
    {
        if (controller?.Events != null)
        {
            controller.Events.OnHandChanged -= Refresh;
            controller.Events.OnLog -= _ => Refresh();
        }
    }

    void Refresh()
    {
        drawText.text = $"Mazo: {controller.Deck.Count}";
        discardText.text = $"Descarte: {controller.Discard.Discard.Count}";
    }
}