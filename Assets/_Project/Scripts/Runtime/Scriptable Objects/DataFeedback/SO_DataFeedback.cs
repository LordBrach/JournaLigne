using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/DataFeedbackPopulation")]
[System.Serializable]
public class SO_DataFeedback : ScriptableObject
{
    // parameters
    public string nameParty;
    public float CurrentGraphValue;
    public Parties party;
    public float maxInfluence;
}
