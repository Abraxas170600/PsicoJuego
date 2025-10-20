using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CombatantPanelView : MonoBehaviour
{
    [Header("Wiring")]
    public Image portrait;
    public TextMeshProUGUI nameText;
    public Slider hpSlider;
    public TextMeshProUGUI hpText;
    public Transform statusContainer;
    public GameObject statusBadgePrefab;

    private Combatant _model;
    private EventBus _events;

    public void Bind(Combatant model, EventBus events)
    {
        _model = model; _events = events;
        nameText.text = model.Name;
        portrait.sprite = model.Portrait;

        hpSlider.maxValue = model.Stats.MaxHP;
        RefreshHp();
        RefreshStatuses();

        _events.OnHpChanged += OnHpChanged;
        _events.OnStatusesChanged += OnStatusesChanged;
        _events.OnStatusApplied += OnStatusApplied;
    }

    void OnDestroy()
    {
        if (_events != null) { _events.OnHpChanged -= OnHpChanged; _events.OnStatusesChanged -= OnStatusesChanged; }
    }

    void OnHpChanged(Combatant who) { if (who == _model) RefreshHp(); }
    void OnStatusesChanged(Combatant who) { if (who == _model) RefreshStatuses(); }
    void OnStatusApplied(Combatant who, StatusEffectSO _, int __)
    {
        if (who == _model) RefreshStatuses();
    }

    void RefreshHp()
    {
        hpSlider.value = _model.Stats.HP;
        hpText.text = $"{_model.Stats.HP}/{_model.Stats.MaxHP}";
    }

    void RefreshStatuses()
    {
        foreach (Transform c in statusContainer) Destroy(c.gameObject);
        foreach (var st in _model.Statuses)
        {
            var go = Instantiate(statusBadgePrefab, statusContainer);
            var badge = go.GetComponent<StatusBadgeView>();
            badge.SetData(st.Definition.DisplayName, st.RemainingTurns);
        }
    }
}

