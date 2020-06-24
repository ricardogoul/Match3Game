using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Match3.UI
{
    public class ScoreManager : MonoBehaviour
    {
        [SerializeField]
        private Text _scoreText;
        [SerializeField]
        private Text _timerText;
        [SerializeField]
        private Text _currentLevelText;

        private int _currentLevel = 1;

        private float _score;
        [SerializeField]
        private float _baseScoreGoal;
        private float _scoreGoal;

        [Tooltip("Amount of seconds that the round should last;")]
        [SerializeField]
        private float _roundTime;
        [SerializeField]
        private float _remainingTime;

        [SerializeField]
        private Image _scoreBar;
        //[SerializeField]
        //private Image _timerBar;        

        //[SerializeField]
        //private Animator _anim;

        private bool _scoreBarFull;

        void Start()
        {
            _scoreGoal = _baseScoreGoal;
            _scoreBar.fillAmount = 0;
            _remainingTime = _roundTime;
            _currentLevelText.text = "L: " + _currentLevel;
        }

        void Update()
        {
            _scoreText.text = _score.ToString();            

            if (_score >= _scoreGoal)
            {
                _scoreBarFull = true;
            }

            //if (_scoreBarFull)
            //{
            //    _anim.SetBool("GoalReached", true);
            //}

            if (!_scoreBarFull)
            {
                _timerText.text = ((int)_remainingTime).ToString();
            }            
        }

        private void FixedUpdate()
        {
            _remainingTime -= Time.deltaTime;
        }

        public void IncreaseScore(int amountToIncrease)
        {
            _score += amountToIncrease;
            _scoreBar.fillAmount = _score / _scoreGoal;
        }

        public void ResetScore()
        {
            _score = 0;
            _scoreBarFull = false;
            _scoreBar.fillAmount = 0;
        }

        public void ResetRoundTimer()
        {
            _remainingTime = _roundTime + 1;
        }

        public void SetNextLevel()
        {
            _currentLevel++;
            _currentLevelText.text = "L: " + _currentLevel.ToString();
            _scoreGoal *= 1.1f;
            ResetScore();
            ResetRoundTimer();
        }

        public void SetFirstLevel()
        {
            _currentLevel = 1;
            _currentLevelText.text = "L: " + _currentLevel.ToString();
            _scoreGoal = _baseScoreGoal;
            ResetScore();
            ResetRoundTimer();
        }

        public float RemainingTime
        {
            get { return _remainingTime; }
            set { _remainingTime = value; }
        }

        public float Score
        {
            get { return _score; }
            set { _score = value; }
        }

        public float ScoreGoal
        {
            get { return _scoreGoal; }
            set { _scoreGoal = value; }
        }

        public bool ScoreGoalReached
        {
            get { return _scoreBarFull; }
            set { _scoreBarFull = value; }
        }

        public int CurrentLevel
        {
            get { return _currentLevel; }
        }
    }
}
