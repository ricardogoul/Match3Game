using System;
using TMPro;
using UnityEngine;

public class TimerPresenter : MonoBehaviour
{
    #region Variables

    [SerializeField]
    private TextMeshProUGUI timerText;

    #endregion

    #region Callbacks

    private void OnEnable()
    {
        UpdateTimerTextEvent += UpdateTimerText;
    }

    private void OnDisable()
    {
        UpdateTimerTextEvent -= UpdateTimerText;
    }

    #endregion

    #region Functions

    private void UpdateTimerText(string text)
    {
        timerText.text = text;
    }

    #endregion

    #region Events

    public Action<string> UpdateTimerTextEvent { get; set; }

    #endregion
}
