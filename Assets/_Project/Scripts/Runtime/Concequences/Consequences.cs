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
    public List<ConsequenceImage> consequencesImages = new List<ConsequenceImage>();

    public void Initialize()
    {
        currentInfluence = influence.influence;
        currentMaxInfluence = influence.maxInfluence;
    }

    public void ShowConsequences(Days currentDay)
    {
        gameObject.SetActive(true);
        
        currentInfluence = influence.influence;
        currentMaxInfluence = influence.maxInfluence;
        GetConsequences(currentDay);
    }
    
    public void HideConsequences()
    {
        gameObject.SetActive(false);
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
