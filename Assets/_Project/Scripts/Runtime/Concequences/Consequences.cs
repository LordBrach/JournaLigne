using System;
using System.Collections.Generic;
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
    [SerializeField] private InfluenceData influence;
    
    public float currentInfluence;
    public float currentMaxInfluence;
    
    [SerializeField] private Image imageComp;
    [SerializeField] private GameObject boxContainer;
    public List<ConsequenceImage> consequencesImages = new List<ConsequenceImage>();

    private Days currentDay;
    public event Action<Days> OnConsequencesHide;

    public void Initialize()
    {
        currentInfluence = influence.influence;
        currentMaxInfluence = influence.maxInfluence;
    }

    public void ShowConsequences(Days day)
    {
        boxContainer.SetActive(true);
        currentDay = day;
        
        currentInfluence = influence.influence;
        currentMaxInfluence = influence.maxInfluence;
        GetConsequences(currentDay);
    }
    
    public void HideConsequences()
    {
        boxContainer.SetActive(false);
        OnConsequencesHide?.Invoke(currentDay);
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

    private void GetConsequences( Days currentDay)
    {
        if (currentInfluence > currentMaxInfluence/2)
        {
            imageComp.sprite = GetConsequenceImage(currentDay.keyFavorable);
        }
        else if (currentInfluence < currentMaxInfluence/2)
        {
            imageComp.sprite = GetConsequenceImage(currentDay.keyUnfavorable);
        }
    }
}
