using System;
using UnityEngine;
using TMPro;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace _Project.Scripts.Runtime.Options
{
    public class OptionsSystem : MonoBehaviour
    {
        // FIELDS //
        [Header("Audio Options")]
        //Audio
        [SerializeField] private AudioMixer m_mainAudioMixer;
        
        [SerializeField] private string m_masterVolumeParamName;
        [SerializeField] private string m_musicVolumeParamName;
        [SerializeField] private string m_sfxVolumeParamName;

        [SerializeField] private Slider m_masterVolumeSlider;
        [SerializeField] private Slider m_musicVolumeSlider;
        [SerializeField] private Slider m_sfxVolumeSlider;
        
        [Header("Quality Options")]
        //Quality
        [SerializeField] private TMP_Dropdown m_qualityDropdown;
        
        //Language
        [Header("Language Options")]
        [SerializeField] private string m_englishLanguageKey;
        [SerializeField] private string m_frenchLanguageKey;
        [SerializeField] private LanguageButtonUI m_languageButtonUI;
        private bool m_isEnglishOn = true;
        
        // METHODS //
        
        private void Start()
        {
            if (m_qualityDropdown)
            {
                m_qualityDropdown.value = QualitySettings.GetQualityLevel();
                m_qualityDropdown.RefreshShownValue();
            }

/*            if (m_masterVolumeSlider)
            {
                m_mainAudioMixer.GetFloat(m_masterVolumeParamName, out float masterVolumeValue);
                m_masterVolumeSlider.value = masterVolumeValue;
            }

            if (m_musicVolumeSlider)
            {
                m_mainAudioMixer.GetFloat(m_musicVolumeParamName, out float musicVolumeValue);
                m_musicVolumeSlider.value = musicVolumeValue;
            }

            if (m_sfxVolumeSlider)
            {
                m_mainAudioMixer.GetFloat(m_sfxVolumeParamName, out float sfxVolumeValue);
                m_sfxVolumeSlider.value = sfxVolumeValue;
            }*/
            
            SwitchToEnglish();
        }

        #region SoundOptions
        public void SetMasterVolume(float newValue)
        {
            m_mainAudioMixer.SetFloat(m_masterVolumeParamName, newValue - 80);
        }

        public void SetMusicVolume(float newValue)
        {
            m_mainAudioMixer.SetFloat(m_musicVolumeParamName, newValue - 80);
        }
        
        public void SetSFXVolume(float newValue)
        {
            m_mainAudioMixer.SetFloat(m_sfxVolumeParamName, newValue - 80);
        }
        #endregion
        
        #region QualityOptions
        public void SetQuality(int qualityIndex)
        {
            QualitySettings.SetQualityLevel(qualityIndex);
        }
        #endregion
        
        #region LanguagesOptions

        public void SwitchLanguage()
        {
            if (m_isEnglishOn)
            {
                SwitchToFrench();
            }
            else
            {
                SwitchToEnglish();
            }
        }

        private void SwitchToEnglish()
        {
            m_isEnglishOn = true;
            SetLanguage(m_englishLanguageKey);
            if(m_languageButtonUI) m_languageButtonUI.SwitchToEnglish();
        }
        
        private void SwitchToFrench()
        {
            m_isEnglishOn = false;
            SetLanguage(m_frenchLanguageKey);
            if(m_languageButtonUI) m_languageButtonUI.SwitchToFrench();
        }
        
        private void SetLanguage(string languageKey)
        {
            if (LocalizationManager.Instance)
            {
                LocalizationManager.Instance.SetLanguage(languageKey);
            }
        }
        
        #endregion
    }
}
