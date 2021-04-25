using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LD48
{
    public class ClickSounds : MonoBehaviour
    {
        public AudioClip selectSound;
        public AudioClip buildSound;
        public AudioClip bulldozeSound;

        private AudioSource audioSource;

        private void Start()
        {
            this.audioSource = this.GetComponent<AudioSource>();
        }

        public void PlaySelectSound()
        {
            this.PlaySound(this.selectSound, 0.2f);
        }

        public void PlayBuildSound()
        {
            this.PlaySound(this.buildSound, 0.2f);
        }

        public void PlayBulldozeSound()
        {
            this.PlaySound(this.bulldozeSound, 0.1f);
        }


        private void PlaySound(AudioClip clip, float volumne)
        {
            this.audioSource.PlayOneShot(clip, volumne);
        }

        private void PlaySound(AudioClip clip)
        {
            this.audioSource.PlayOneShot(clip);
        }
    }
}
