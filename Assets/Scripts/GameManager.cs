using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Match3.UI;
using Match3.Sounds;

namespace Match3.Grid
{
    public class GameManager : MonoBehaviour
    {
        public int CurrentLevel { get; private set; }
        
        private bool checkRoundFinished;
        private bool playingGame;

        [SerializeField]
        private Score score;
        [SerializeField]
        private Timer timer;
        [SerializeField]
        private MenuController menuController;
        [SerializeField]
        private GridGenerator gridGenerator;

        private void Awake()
        {
            ServiceLocator.Provide(this);
        }

        private void FixedUpdate()
        {
            if (!playingGame) return;
            
            if (timer.RemainingTime <= 0 && !checkRoundFinished)
            {
                checkRoundFinished = true;
                FailedRound();
                playingGame = false;
            }

            if (score.ScoreGoalReached && !checkRoundFinished)
            {
                checkRoundFinished = true;
                RoundFinished();
                RoundCleared();
                playingGame = false;
            }
        }

        public void StartGame()
        {
            SetGameUp();
            score.SetFirstLevel();
        }
        
        public void SetGameUp()
        {
            UnfreezeGame();
            playingGame = true;
            checkRoundFinished = false;
            gridGenerator.CurrentState = GridGenerator.GameState.move;
            ServiceLocator.GetSoundManager().PlayBackgroundMusic();
            timer.TimerSetup();
        }

        private void UnfreezeGame()
        {
            Time.timeScale = 1;
        }
        
        public void StartNextLevel()
        {
            SetGameUp();
            gridGenerator.ShuffleGems();
            CurrentLevel++;
            score.SetNextLevel();
        }
        
        private void RoundFinished()
        {
            ServiceLocator.GetSoundManager().StopBackgroundMusic();            

            Invoke(nameof(FreezeGame), 1);
            menuController.ClearedLevel();
        }

        private void FailedRound()
        {
            Invoke(nameof(FreezeGame), 1);
            menuController.FailedRound();
        }

        private void RoundCleared()
        {
            ServiceLocator.GetSoundManager().PlayClearLevelSound();
        }

        private void FreezeGame()
        {
            gridGenerator.CurrentState = GridGenerator.GameState.cantMove;
            Time.timeScale = 0;
        }

        public bool PlayingGame
        {
            get { return playingGame; }
            set { playingGame = value; }
        }
    }
}
