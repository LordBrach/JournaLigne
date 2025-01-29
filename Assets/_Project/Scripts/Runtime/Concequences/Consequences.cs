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
    public float currentInfluence;
    public float currentMaxInfluence;
    
    [SerializeField] private Image imageComp;
    public List<ConsequenceImage> consequencesImages = new List<ConsequenceImage>();

    public void Initialize()
    {
        
    }

    public void ShowConsequences(float influence, float maxInfluence, string key)
    {
        gameObject.SetActive(true);
        
        currentInfluence = influence;
        currentMaxInfluence = maxInfluence;
        GetConsequences(currentInfluence, currentMaxInfluence, key);
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

    private void GetConsequences(float influence, float maxInfluence, string key)
    {
        if (influence > maxInfluence/2)
        {
            imageComp.sprite = GetConsequenceImage(key);
        }
        else if (influence < maxInfluence/2)
        {
            imageComp.sprite = GetConsequenceImage(key);
        }
    }
}
