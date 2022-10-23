using UnityEngine;

namespace Match3.UI
{
    [RequireComponent(typeof(ScorePresenter))]
    public class Score : MonoBehaviour
    {
        #region Variables

        public bool ScoreGoalReached { get; private set; }
        
        [SerializeField]
        private float baseScoreGoal;

        private float score;
        private float scoreGoal;

        private ScorePresenter scorePresenter;

        #endregion

        #region Callbacks
        
        private void OnEnable()
        {
            HandleIncreaseScoreDelegate += IncreaseScore;
        }

        private void OnDisable()
        {
            HandleIncreaseScoreDelegate -= IncreaseScore;
        }

        private void OnValidate()
        {
            scorePresenter = GetComponent<ScorePresenter>();
        }

        private void Start()
        {
            ScoreSetup();
        }

        #endregion

        #region Functions

        private void ScoreSetup()
        {
            scoreGoal = baseScoreGoal;
        }

        private void IncreaseScore(int amountToIncrease)
        {
            score += amountToIncrease;
            scorePresenter.UpdateScoreUIEvent?.Invoke(score.ToString(), score / scoreGoal);
            CheckIfScoreGoalReached();
        }
        
        private void CheckIfScoreGoalReached()
        {
            ScoreGoalReached = score >= scoreGoal;
        }

        private void ResetScore()
        {
            score = 0;
            ScoreGoalReached = false;
        }

        public void SetNextLevel()
        {
            ResetScore();
            scorePresenter.ScoreUISetupEvent?.Invoke();
            scoreGoal *= 1.1f;
        }

        public void SetFirstLevel()
        {
            ResetScore();
            scorePresenter.ScoreUISetupEvent?.Invoke();
            scoreGoal = baseScoreGoal;
        }
        
        #endregion

        #region Events/Delegates
        
        public static IncreaseScoreDelegate HandleIncreaseScoreDelegate;
        public delegate void IncreaseScoreDelegate(int num);

        #endregion
    }
}
