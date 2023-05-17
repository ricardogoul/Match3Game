using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ScorePresenter : MonoBehaviour
{
    #region Variables

    [SerializeField]
    private TextMeshProUGUI scoreText;
    [SerializeField]
    private TextMeshProUGUI currentLevelText;
    
    [SerializeField]
    private Image scoreBar;

    private bool scoreBarFull;

    #endregion

    #region Callbacks

    private void OnEnable()
    {
        UpdateScoreUIEvent += UpdateScoreUI;
        ScoreUISetupEvent += ScoreUISetup;
    }

    private void OnDisable()
    {
        UpdateScoreUIEvent -= UpdateScoreUI;
        ScoreUISetupEvent -= ScoreUISetup;
    }

    #endregion

    #region Functions

    private void UpdateScoreUI(string text, float fillAmount)
    {
        scoreBar.fillAmount = fillAmount;
        scoreText.text = text;
    }

    private void ScoreUISetup()
    {
        scoreBar.fillAmount = 0;
        scoreText.text = "0";
        currentLevelText.text = "L: " + ServiceLocator.GetGameManager().CurrentLevel;
    }

    #endregion
    
    #region Events

    public Action<string, float> UpdateScoreUIEvent { get; set; }
    public Action ScoreUISetupEvent { get; set; }

    #endregion
}
