using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;

namespace _Project.Scripts.Runtime.Core.AudioSystem
{
    
    public class AudioManager : MonoBehaviour
    {
        [Header("General")]
        [SerializeField] private AudioMixer m_currentMixer;
        
        [Header("Music")] 
        [SerializeField] private AudioSource m_musicSource;
        
        [Space(10)]
        [SerializeField, Tooltip("In seconds")] private float m_fadeOutSpeed = 1;
        [SerializeField, Tooltip("In seconds")] private float m_fadeInSpeed = 1;

        private Coroutine m_switchMusicCoroutine;
        private Coroutine m_killMusicCoroutine;
        
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

        private void Awake()
        {
            InitSingleton();
        }
        
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

            audioSource.loop = false;
            
            audioSource.Play();
            Destroy(tGo, audioClip.length);
        }

        #region Music

        public void PlayMusic(AudioClip audioClip)
        {
            if(audioClip == null) return;
            
            StopKillMusic();
            StopSwitchMusic();

            m_switchMusicCoroutine = StartCoroutine(SwitchMusic(audioClip));
        }
        
        public void StopMusic()
        {
            StopKillMusic();
            StopSwitchMusic();
            
            m_killMusicCoroutine = StartCoroutine(KillMusic());
        }

        private IEnumerator SwitchMusic(AudioClip newAudioClip)
        {
            yield return KillMusic();
            
            m_musicSource.volume = 0;
            m_musicSource.clip = newAudioClip;
            
            m_musicSource.Play();
            
            if(m_fadeInSpeed != 0)
            {
                while (m_musicSource.volume < 1)
                {
                    m_musicSource.volume += Time.deltaTime / m_fadeInSpeed;
                    
                    yield return null;
                }
            }
            else
            {
                m_musicSource.volume = 1;
            }
            
            StopSwitchMusic();
        }

        private void StopSwitchMusic()
        {
            if (m_switchMusicCoroutine == null) return;
            
            StopCoroutine(m_switchMusicCoroutine);
            m_switchMusicCoroutine = null;
        }
        
        private IEnumerator KillMusic()
        {
            if(m_musicSource.isPlaying)
            {
                if (m_fadeOutSpeed != 0)
                {
                    while (m_musicSource.volume > 0)
                    {
                    
                        m_musicSource.volume -=  Time.deltaTime / m_fadeOutSpeed;
                        yield return null;
                    }
                }
                
                m_musicSource.Stop();
            }
            
            StopKillMusic();
        }

        private void StopKillMusic()
        {
            if (m_killMusicCoroutine == null) return;
            
            StopCoroutine(m_killMusicCoroutine);
            m_killMusicCoroutine = null;
        }

        #endregion
        
    }
}
