using UnityEngine;

public class HandPresenter : MonoBehaviour
{
    public GameObject cardViewPrefab;
    public Transform content;
    public BattleController controller;

    void Start()
    {
        controller.Events.OnHandChanged += Rebuild;
        Rebuild();
    }

    void OnDisable()
    {
        if (controller?.Events != null) controller.Events.OnHandChanged -= Rebuild;
    }

    void Rebuild()
    {
        foreach (Transform c in content) Destroy(c.gameObject);
        foreach (var card in controller.Hand.Hand)
        {
            var go = Instantiate(cardViewPrefab, content);
            var view = go.GetComponent<CardView>();
            view.Bind(card, controller);

            // Botón deshabilitado si no se puede jugar
            var btn = go.GetComponent<UnityEngine.UI.Button>();
            btn.interactable = !controller.InputLocked/* && controller.FSM.State == BattleState.PlayerAction*/;
        }
    }
}

