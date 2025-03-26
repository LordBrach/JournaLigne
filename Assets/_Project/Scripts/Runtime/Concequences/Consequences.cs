using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ConsequenceImage
{
    public Sprite Image;
    public string Key;
}

public class Consequences : MonoBehaviour
{
    // Change to Scriptable Alessendro
    [SerializeField] private SO_DataFeedback influence;
    
    public float currentInfluence;
    public float currentMaxInfluence;
    
    [SerializeField] private Image imageComp;
    [SerializeField] private GameObject boxContainer;
    public List<ConsequenceImage> consequencesImages = new List<ConsequenceImage>();
    [SerializeField] private TextMeshProUGUI textComp;

    private Days _currentDay;
    public event Action<Days> OnConsequencesHide;
    public event Action<Days> OnConsequencesValidate;

    public void Initialize()
    {
        currentInfluence = influence.CurrentGraphValue;
        currentMaxInfluence = influence.maxInfluence;
    }

    public void ShowConsequences(Days day)
    {
        boxContainer.SetActive(true);
        _currentDay = day;
        
        currentInfluence = influence.CurrentGraphValue;
        currentMaxInfluence = influence.maxInfluence;
        GetConsequences(_currentDay);
    }

    public void ValidateConsequences()
    {
        OnConsequencesValidate?.Invoke(_currentDay);
    }
    
    public void HideConsequences()
    {
        boxContainer.SetActive(false);
        OnConsequencesHide?.Invoke(_currentDay);
    }

    Sprite GetConsequenceImage(string key)
    {
        foreach (var consequenceImage in consequencesImages)
        {
            if (consequenceImage.Key == key)
            {
                return consequenceImage.Image;
            }
        }
        return null;
    }

    private void GetConsequences(Days currentDay)
    {
        if (currentInfluence >= currentMaxInfluence/2)
        {
            imageComp.sprite = GetConsequenceImage(currentDay.keyFavorable);
            textComp.text = LocalizationManager.Instance.GetTranslation(currentDay.keyFavorable);
        }
        else if (currentInfluence < currentMaxInfluence/2)
        {
            imageComp.sprite = GetConsequenceImage(currentDay.keyUnfavorable);
            textComp.text = LocalizationManager.Instance.GetTranslation(currentDay.keyUnfavorable);
        }
    }
}
