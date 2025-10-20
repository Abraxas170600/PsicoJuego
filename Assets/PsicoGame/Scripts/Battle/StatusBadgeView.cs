using UnityEngine;
using TMPro;

public class StatusBadgeView : MonoBehaviour
{
    //public TextMeshProUGUI label;
    public TextMeshProUGUI turns;
    private string _statusName;

    public void SetData(string name, int remainingTurns)
    {
        //label.text = name;
        _statusName = name;
        turns.text = remainingTurns > 0 ? $"{remainingTurns}T" : "";
    }
}