using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Match3.UI
{
    public class MenuController : MonoBehaviour
    {
        [SerializeField]
        private Animator menuAnim;
        [SerializeField]
        private Animator innerMenuAnim;
        [SerializeField]
        private Animator levelClearedAnim;
        [SerializeField]
        private Animator failedLevelAnim;

        [SerializeField]
        private TextMeshProUGUI nextLevelButtonText;
        [SerializeField]
        private TextMeshProUGUI nextLevelMessage;
        [SerializeField]
        private TextMeshProUGUI failedAtLevelText;

        private static readonly int PlayGame = Animator.StringToHash("PlayGame");

        private void Start()
        {
            ServiceLocator.GetSoundManager().PlayBackgroundMusic();
        }

        public void StartButton()
        {
            if (menuAnim == null || innerMenuAnim == null) return;
            
            menuAnim.SetTrigger(PlayGame);
            innerMenuAnim.SetTrigger(PlayGame);

            ServiceLocator.GetGameManager().StartGame();
            nextLevelButtonText.text = "Level " + (ServiceLocator.GetGameManager().CurrentLevel + 1).ToString();
            nextLevelMessage.text = "Level " + ServiceLocator.GetGameManager().CurrentLevel.ToString() + " Cleared!";
        }

        public void NextLevelButton()
        {
            levelClearedAnim.SetTrigger("NextLevel");
            ServiceLocator.GetGameManager().StartNextLevel();
            Invoke("UpdateTexts", 1);
        }

        private void UpdateTexts()
        {
            nextLevelButtonText.text = "Level " + (ServiceLocator.GetGameManager().CurrentLevel + 1).ToString();
            nextLevelMessage.text = "Level " + ServiceLocator.GetGameManager().CurrentLevel.ToString() + " Cleared!";
        }

        public void MenuButton()
        {
            levelClearedAnim.SetTrigger("NextLevel");            
            Invoke("CallMenu", 1f);
            ServiceLocator.GetGameManager().SetGameUp();

        }

        public void FailMenuButton()
        {
            failedLevelAnim.SetTrigger("CallMenu");
            Invoke("CallMenu", 1f);
            ServiceLocator.GetGameManager().SetGameUp();
        }

        public void FailedRound()
        {
            failedAtLevelText.text = "You lost at level " + ServiceLocator.GetGameManager().CurrentLevel + ".";
            failedLevelAnim.SetTrigger("LevelCleared");
        }

        public void ClearedLevel()
        {
            levelClearedAnim.SetTrigger("LevelCleared");
        }
        
        private void CallMenu()
        {
            menuAnim.SetTrigger("OpenMenu");
            innerMenuAnim.SetTrigger("OpenMenu");
        }
    }
}
