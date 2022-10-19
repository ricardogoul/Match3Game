using System;
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

        private void Start()
        {
            ServiceLocator.GetSoundManager().PlayBackgroundMusic();
        }

        public void StartButton()
        {
            if (_menuAnim != null && _innerMenuAnim != null)
            {
                _menuAnim.SetTrigger("PlayGame");
                _innerMenuAnim.SetTrigger("PlayGame");

                ServiceLocator.GetGameManager().StartGame();
                _nextLevelButtonText.text = "Level " + (ServiceLocator.GetGameManager().CurrentLevel + 1).ToString();
                _nextLevelMessage.text = "Level " + ServiceLocator.GetGameManager().CurrentLevel.ToString() + " Cleared!";
            }
        }

        public void NextLevelButton()
        {
            _levelClearedAnim.SetTrigger("NextLevel");
            ServiceLocator.GetGameManager().StartNextLevel();
            Invoke("UpdateTexts", 1);
        }

        private void UpdateTexts()
        {
            _nextLevelButtonText.text = "Level " + (ServiceLocator.GetGameManager().CurrentLevel + 1).ToString();
            _nextLevelMessage.text = "Level " + ServiceLocator.GetGameManager().CurrentLevel.ToString() + " Cleared!";
        }

        public void MenuButton()
        {
            _levelClearedAnim.SetTrigger("NextLevel");            
            Invoke("CallMenu", 1f);
            ServiceLocator.GetGameManager().SetGameUp();

        }

        public void FailMenuButton()
        {
            _failedLevelAnim.SetTrigger("CallMenu");
            Invoke("CallMenu", 1f);
            ServiceLocator.GetGameManager().SetGameUp();
        }

        public void FailedRound()
        {
            _failedAtLevelText.text = "You lost at level " + ServiceLocator.GetGameManager().CurrentLevel + ".";
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
