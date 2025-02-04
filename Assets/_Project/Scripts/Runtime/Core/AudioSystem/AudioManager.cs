using UnityEngine;
using UnityEngine.Audio;

namespace _Project.Scripts.Runtime.Core.AudioSystem
{
    public class AudioManager : MonoBehaviour
    {
        [Header("General")]
        [SerializeField] private AudioMixer m_currentMixer;

        [Header("Music")] [SerializeField] private AudioSource m_musicSource;
        
        #region Singleton

        private static AudioManager m_instance;
        public static AudioManager Instance => m_instance;

        private void InitSingleton()
        {
            if (m_instance != this && m_instance != null)
            {
                Destroy(this.gameObject);
            }
            else
            {
                m_instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
        }

        #endregion

        public void PlayOneShotSound2D(AudioClip audioClip, AudioMixerGroup audioMixerGroup = null)
        {
            if(audioClip == null) return;
            
            GameObject tGo = new GameObject("One shot audio 2D");
            tGo.transform.position = this.transform.position;
            tGo.transform.SetParent(this.transform);
            
            AudioSource audioSource = tGo.AddComponent<AudioSource>();

            audioSource.clip = audioClip;
            if (audioMixerGroup)
            {
                audioSource.outputAudioMixerGroup = audioMixerGroup;
            }
            else
            {
                audioSource.outputAudioMixerGroup = m_currentMixer.outputAudioMixerGroup;
            }
            
            audioSource.Play();
            Destroy(tGo, audioClip.length);
        }

        public void PlayMusic(AudioClip audioClip)
        {
            
        }
    }
}
