using UnityEngine;
using UnityEngine.Audio;

namespace _Project.Scripts.Runtime.Core.AudioSystem
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private AudioMixer m_currentMixer;
        
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

        public void PlaySound2D(AudioClip audioClip, AudioMixerGroup audioMixerGroup = null)
        {
            GameObject tGo = new GameObject("One shot audio 2D");
            tGo.transform.position = transform.position;
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
    }
}
