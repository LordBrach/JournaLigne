using System;
using UnityEngine;
using TMPro;

namespace _Project.Scripts.Runtime.Options
{
    public class OptionsSystem : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown m_qualityDropdown;

        private void Start()
        {
            if (m_qualityDropdown)
            {
                m_qualityDropdown.value = QualitySettings.GetQualityLevel();
                m_qualityDropdown.RefreshShownValue();
            }
        }

        public void SetQuality(int qualityIndex)
        {
            QualitySettings.SetQualityLevel(qualityIndex);
        }
    }
}
