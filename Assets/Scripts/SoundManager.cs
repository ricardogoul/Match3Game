using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match3.Sounds
{
    public class SoundManager : MonoBehaviour
    {
        [SerializeField]
        private AudioSource _explodeGemSound;
        [SerializeField]
        private AudioSource _clearLevelSound;
        [SerializeField]
        private AudioSource _swapGemsSound;
        [SerializeField]
        private AudioSource _backGroundMusic;

        public void PlayExplodeGemSound()
        {
            _explodeGemSound.Play();
        }

        public void PlayClearLevelSound()
        {
            _clearLevelSound.Play();
        }

        public void PlaySwapGemsSound()
        {
            _swapGemsSound.Play();
        }

        public void PlayBackgroundMusic()
        {
            _backGroundMusic.Play();
        }

        public void StopBackgroundMusic()
        {
            _backGroundMusic.Stop();
        }
    }
}
