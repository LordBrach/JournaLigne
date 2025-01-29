using System;
using System.Collections.Generic;
using UnityEngine;

public class DayManager : MonoBehaviour
{
    public static DayManager instance;

    public int CurrentDay { get; private set; } = 1;
    [SerializeField] private int daysInWeek = 2;
    
    private bool _isSkippingDays = false;

    private Dictionary<int, Action> _narrativeEvents = new Dictionary<int, Action>();
    
    public event Action<int> OnDayChanged;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        Debug.Log("Le jeu commence au jour : " + CurrentDay);

        AddEvent(7, () => Debug.Log("Événement de fin de semaine déclenché !"));
        AddEvent(30, () => Debug.Log("Fin du mois, événement spécial !"));
    }

    /// <summary>
    /// Go to new day.
    /// </summary>
    public void NextDay()
    {
        CurrentDay++;
        
        OnDayChanged?.Invoke(CurrentDay);
        CheckForEvent();

        if (CurrentDay % daysInWeek == 1)
        {
            Debug.Log("Nouvelle semaine !");
        }
    }

    /// <summary>
    /// Skip to Needed days.
    /// </summary>
    /// <param name="daysToSkip">Days to skip.</param>
    public void SkipDays(int daysToSkip)
    {
        if (_isSkippingDays) return;

        _isSkippingDays = true;
        Debug.Log($"Saut de {daysToSkip} jour(s)...");

        for (int i = 0; i < daysToSkip; i++)
        {
            NextDay();
        }

        _isSkippingDays = false;
    }

    /// <summary>
    /// Adds a narrative event that triggers on a given day.
    /// </summary>
    /// <param name="day">Day the event triggers.</param>
    /// <param name="eventAction">Action to perform.</param>
    public void AddEvent(int day, Action eventAction)
    {
        if (!_narrativeEvents.ContainsKey(day))
        {
            _narrativeEvents.Add(day, eventAction);
        }
    }

    /// <summary>
    /// Checks if an event should be triggered for the current day.
    /// </summary>
    private void CheckForEvent()
    {
        if (_narrativeEvents.ContainsKey(CurrentDay))
        {
            Debug.Log($"Un événement se déclenche au jour : {CurrentDay}");
            _narrativeEvents[CurrentDay]?.Invoke();
        }
    }
    
    /// <summary>
    /// Allows another script to subscribe to the day change.
    /// </summary>
    /// <param name="callback">Method to call when a day changes.</param>
    public void SubscribeToDayChange(Action<int> callback)
    {
        OnDayChanged += callback;
    }
    
    /// <summary>
    /// Allows another script to unsubscribe from day change.
    /// </summary>
    /// <param name="callback">Method to remove from event.</param>
    public void UnsubscribeFromDayChange(Action<int> callback)
    {
        OnDayChanged -= callback;
    }
}
