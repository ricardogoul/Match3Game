using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Match3.UI;
using Match3.Sounds;

namespace Match3.Grid
{
    public class GameManager : MonoBehaviour
    {
        private bool _checkRoundFinished;
        private bool _playingGame;

        [SerializeField]
        private SoundManager _soundManager;        
        [SerializeField]
        private ScoreManager _scoreManager;
        [SerializeField]
        private MenuController _menuController;
        private GridGenerator _gridGenerator;

        void Start()
        {
            _gridGenerator = GetComponentInParent<GridGenerator>();

            _soundManager.PlayBackgroundMusic();
        }

        void Update()
        {

        }

        private void FixedUpdate()
        {
            if (_playingGame)
            {
                if (_scoreManager.RemainingTime <= 0 && !_checkRoundFinished)
                {
                    _checkRoundFinished = true;
                    FailedRound();
                    _playingGame = false;
                }

                if (_scoreManager.ScoreGoalReached && !_checkRoundFinished)
                {
                    _checkRoundFinished = true;
                    RoundFinished();
                    RoundCleared();
                    _playingGame = false;
                }
            }
        }

        private void RoundFinished()
        {
            _soundManager.StopBackgroundMusic();            

            //if (!_gridGenerator.GemsMatchedOnGrid())
            //{
            Invoke("FreezeGame", 1);
            _menuController.ClearedLevel();            
            //}
            //else
            //{
            //    while (_gridGenerator.GemsMatchedOnGrid())
            //    {

            //    }
            //    Invoke("FreezeGame", 1);
            //    _menuController.ClearedLevel();
            //}
        }

        private void FailedRound()
        {
            Invoke("FreezeGame", 1);
            _menuController.FailedRound();
        }

        private void RoundCleared()
        {
            _soundManager.PlayClearLevelSound();
        }

        private void FreezeGame()
        {
            _gridGenerator.CurrentState = GridGenerator.GameState.cantMove;
            Time.timeScale = 0;
        }

        public void UnFreezeGame()
        {
            Time.timeScale = 1;
        }

        public void RestartGame()
        {
            UnFreezeGame();
            _checkRoundFinished = false;
            _scoreManager.SetFirstLevel();
            _soundManager.PlayBackgroundMusic();
        }

        public void StartNextLevel()
        {
            UnFreezeGame();
            _gridGenerator.CurrentState = GridGenerator.GameState.move;
            _checkRoundFinished = false;
            _gridGenerator.ShuffleGems();
            _scoreManager.SetNextLevel();
            _soundManager.PlayBackgroundMusic();
            _playingGame = true;
        }

        public bool PlayingGame
        {
            get { return _playingGame; }
            set { _playingGame = value; }
        }
    }
}
