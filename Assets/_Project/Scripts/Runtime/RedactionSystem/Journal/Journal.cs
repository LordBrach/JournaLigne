using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Factions
{
    Rebels,
    People,
    Government,
    None
}

[System.Serializable]
public class Appreciations
{
    public float rebelsAppreciation;           // Rebeles
    public float peopleAppreciation;          // people
    public float governmentAppreciation;     // government
}

public class Journal : MonoBehaviour
{
    public Appreciations appreciations { get; private set; } = new Appreciations();

    /// <summary>
    /// Increases or decreases the appreciation of a given faction.
    /// </summary>
    /// <param name="faction">Targeted faction.</param>
    /// <param name="amount">Appreciation amount to add or remove.</param>
    /// /// <param name="multiplier">Multiplicative factor applied to the amount.</param>
    public void AddAppreciation(Factions faction, float amount, float multiplier)
    {
        float finalAmount = amount * multiplier;
        switch (faction)
        {
            case Factions.Rebels:
                appreciations.rebelsAppreciation += finalAmount;
                break;
            case Factions.People:
                appreciations.peopleAppreciation += finalAmount;
                break;
            case Factions.Government:
                appreciations.governmentAppreciation += finalAmount;
                break;
        }

        Debug.Log($"{faction} appreciation changed by {finalAmount}. Current values: " +
                  $"Rebels: {appreciations.rebelsAppreciation}, " +
                  $"People: {appreciations.peopleAppreciation}, " +
                  $"Government: {appreciations.governmentAppreciation}");
    }
}
