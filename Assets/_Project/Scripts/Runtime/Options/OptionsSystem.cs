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
        //Audio
        [SerializeField] private AudioMixer m_mainAudioMixer;
        
        [SerializeField] private string m_masterVolumeParamName;
        [SerializeField] private string m_musicVolumeParamName;
        [SerializeField] private string m_sfxVolumeParamName;

        [SerializeField] private Slider m_masterVolumeSlider;
        [SerializeField] private Slider m_musicVolumeSlider;
        [SerializeField] private Slider m_sfxVolumeSlider;
        
        //Quality
        [SerializeField] private TMP_Dropdown m_qualityDropdown;
        
        //Language
        
        // METHODS //
        
        private void Start()
        {
            if (m_qualityDropdown)
            {
                m_qualityDropdown.value = QualitySettings.GetQualityLevel();
                m_qualityDropdown.RefreshShownValue();
            }

            if (m_masterVolumeSlider)
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
            }
        }

        #region SoundOptions
        public void SetMasterVolume(float newValue)
        {
            m_mainAudioMixer.SetFloat(m_masterVolumeParamName, newValue);
        }

        public void SetMusicVolume(float newValue)
        {
            m_mainAudioMixer.SetFloat(m_musicVolumeParamName, newValue);
        }
        
        public void SetSFXVolume(float newValue)
        {
            m_mainAudioMixer.SetFloat(m_sfxVolumeParamName, newValue);
        }
        #endregion
        
        #region QualityOptions
        public void SetQuality(int qualityIndex)
        {
            QualitySettings.SetQualityLevel(qualityIndex);
        }
        #endregion
        
        #region LanguagesOptions

        public void SetLanguage(string languageKey)
        {
            if (LocalizationManager.Instance)
            {
                LocalizationManager.Instance.SetLanguage(languageKey);
            }
            
        }
        
        #endregion
    }
}
