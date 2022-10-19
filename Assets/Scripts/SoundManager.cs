using UnityEngine;

namespace Match3.Sounds
{
    public class SoundManager : MonoBehaviour
    {
        [SerializeField]
        private AudioSource explodeGemSound;
        [SerializeField]
        private AudioSource clearLevelSound;
        [SerializeField]
        private AudioSource swapGemsSound;
        [SerializeField]
        private AudioSource backGroundMusic;

        private void OnEnable()
        {
            ServiceLocator.Provide(this);
        }

        public void PlayExplodeGemSound()
        {
            explodeGemSound.Play();
        }

        public void PlayClearLevelSound()
        {
            clearLevelSound.Play();
        }

        public void PlaySwapGemsSound()
        {
            swapGemsSound.Play();
        }

        public void PlayBackgroundMusic()
        {
            backGroundMusic.Play();
        }

        public void StopBackgroundMusic()
        {
            backGroundMusic.Stop();
        }
    }
}
