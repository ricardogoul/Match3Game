using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Match3.Grid;

namespace Match3.UI
{
    public class MenuController : MonoBehaviour
    {
        [SerializeField]
        private Animator _menuAnim;
        [SerializeField]
        private Animator _innerMenuAnim;
        [SerializeField]
        private Animator _levelClearedAnim;
        [SerializeField]
        private Animator _failedLevelAnim;

        [SerializeField]
        private Text _nextLevelButtonText;
        [SerializeField]
        private Text _nextLevelMessage;
        [SerializeField]
        private Text _failedAtLevelText;

        [SerializeField]
        private GameManager _gameManager;
        [SerializeField]
        private ScoreManager _scoreManager;
        private GridGenerator _gridGenerator;

        void Start()
        {
            _gridGenerator = GetComponentInParent<GridGenerator>();
        }

        public void StartButton()
        {
            if (_menuAnim != null && _innerMenuAnim != null)
            {
                _menuAnim.SetTrigger("PlayGame");
                _innerMenuAnim.SetTrigger("PlayGame");

                _gameManager.RestartGame();
                _gridGenerator.CurrentState = GridGenerator.GameState.move;
                _gameManager.PlayingGame = true;
                _nextLevelButtonText.text = "Level " + (_scoreManager.CurrentLevel + 1).ToString();
                _nextLevelMessage.text = "Level " + _scoreManager.CurrentLevel.ToString() + " Cleared!";
            }
        }

        public void NextLevelButton()
        {
            _levelClearedAnim.SetTrigger("NextLevel");
            _gameManager.StartNextLevel();
            Invoke("UpdateTexts", 1);
        }

        private void UpdateTexts()
        {
            _nextLevelButtonText.text = "Level " + (_scoreManager.CurrentLevel + 1).ToString();
            _nextLevelMessage.text = "Level " + _scoreManager.CurrentLevel.ToString() + " Cleared!";
        }

        public void MenuButton()
        {
            _levelClearedAnim.SetTrigger("NextLevel");            
            Invoke("CallMenu", 1f);
            _gameManager.RestartGame();

        }

        public void FailMenuButton()
        {
            _failedLevelAnim.SetTrigger("CallMenu");
            Invoke("CallMenu", 1f);
            _gameManager.RestartGame();
        }

        public void FailedRound()
        {
            _failedAtLevelText.text = "You lost at level " + _scoreManager.CurrentLevel + ".";
            _failedLevelAnim.SetTrigger("LevelCleared");
        }

        public void ClearedLevel()
        {
            _levelClearedAnim.SetTrigger("LevelCleared");
        }
        
        private void CallMenu()
        {
            _menuAnim.SetTrigger("OpenMenu");
            _innerMenuAnim.SetTrigger("OpenMenu");
        }
    }
}
