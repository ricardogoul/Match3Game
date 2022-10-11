using System;
using UnityEngine;

[RequireComponent(typeof(TimerPresenter))]
public class Timer : MonoBehaviour
{
    #region Variables

    public float RemainingTime { get; private set; }
    [SerializeField]
    private float roundTimeInSeconds;

    private TimerPresenter timerPresenter;

    #endregion

    #region Callbacks

    private void OnValidate()
    {
        timerPresenter = GetComponent<TimerPresenter>();
    }

    private void FixedUpdate()
    {
        IncreaseTime();
    }

    #endregion

    #region Functions

    private void IncreaseTime()
    {
        RemainingTime -= Time.deltaTime;
        timerPresenter.UpdateTimerTextEvent?.Invoke(Mathf.Round(RemainingTime).ToString());
    }

    private void TimerSetup()
    {
        RemainingTime = roundTimeInSeconds;
    }
    
    public void ResetTimer()
    {
        RemainingTime = roundTimeInSeconds + 1;
    }

    #endregion
}
