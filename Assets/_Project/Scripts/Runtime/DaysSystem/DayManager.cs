using System;
using System.Collections.Generic;
using LittleGraph.Runtime;
using UnityEngine;
using UnityEngine.Serialization;

public enum DayType
{
    None,
    Interview,
    Review,
    Article,
    EndGame
}

[System.Serializable]
public class Days
{
    public int day = 1;
    public DayType dayType = DayType.None;
    public LGGraph currentGraph;
}

public class DayManager : MonoBehaviour
{
    public static DayManager instance;

    [SerializeField] private int daysInWeek = 2;
    public Days CurrentDay { get; private set; }
    public List<Days> daysList = new List<Days>();
    
    private int _currentDayIndex = 0;
    private bool _isSkippingDays = false;

    private Dictionary<int, Action> _narrativeEvents = new Dictionary<int, Action>();

    public List<LGGraph> graphs = new List<LGGraph>();

    public event Action<Days> OnDayStarted;
    public event Action<Days> OnDayEnded;
    public event Action<Days> OnDayChanged;

    private void Awake()
    {
        Initialize();
    }

    public void Initialize()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (daysList.Count > 0)
        {
            CurrentDay = daysList[0];
            _currentDayIndex = 0;
        }
        else
        {
            Debug.LogError("DaysList est vide ! Ajoutez des jours avant de démarrer.");
        }
    }

    private void Start()
    {
        if (CurrentDay != null)
        {
            Debug.Log($"Le jeu commence au jour {CurrentDay.day}");
            StartNewDay();
        }
    }

    /// <summary>
    /// Commence un nouveau jour.
    /// </summary>
    private void StartNewDay()
    {
        if (CurrentDay == null) return;

        OnDayStarted?.Invoke(CurrentDay);
        Debug.Log($"Début du jour {CurrentDay.day}");

        CheckForEvent();
    }

    /// <summary>
    /// Termine la journée en cours.
    /// </summary>
    private void EndCurrentDay()
    {
        if (CurrentDay == null) return;

        OnDayEnded?.Invoke(CurrentDay);
        Debug.Log($"Fin du jour {CurrentDay.day}");
    }

    /// <summary>
    /// Passe au jour suivant en prenant le prochain élément de DaysList.
    /// </summary>
    public void NextDay()
    {
        EndCurrentDay();

        if (_currentDayIndex + 1 < daysList.Count)
        {
            _currentDayIndex++;
            CurrentDay = daysList[_currentDayIndex];

            OnDayChanged?.Invoke(CurrentDay);

            StartNewDay();

            if (CurrentDay.day % daysInWeek == 1)
            {
                Debug.Log("Nouvelle semaine !");
            }
        }
        else
        {
            Debug.Log("Plus de jours disponibles dans la liste !");
        }
    }

    /// <summary>
    /// Saute plusieurs jours en avançant dans DaysList.
    /// </summary>
    /// <param name="daysToSkip">Nombre de jours à sauter.</param>
    public void SkipDays(int daysToSkip)
    {
        if (_isSkippingDays) return;

        _isSkippingDays = true;
        Debug.Log($"Saut de {daysToSkip} jour(s)...");

        for (int i = 0; i < daysToSkip; i++)
        {
            if (_currentDayIndex + 1 < daysList.Count)
            {
                NextDay();
            }
            else
            {
                Debug.Log("Impossible de sauter plus de jours, fin de la liste atteinte !");
                break;
            }
        }

        _isSkippingDays = false;
    }

    /// <summary>
    /// Ajoute un événement narratif à un jour donné.
    /// </summary>
    /// <param name="day">Jour où l'événement se déclenche.</param>
    /// <param name="eventAction">Action à exécuter.</param>
    public void AddEvent(int day, Action eventAction)
    {
        if (!_narrativeEvents.ContainsKey(day))
        {
            _narrativeEvents.Add(day, eventAction);
        }
    }

    /// <summary>
    /// Vérifie s'il y a un événement à déclencher pour le jour actuel.
    /// </summary>
    private void CheckForEvent()
    {
        if (CurrentDay != null && _narrativeEvents.ContainsKey(CurrentDay.day))
        {
            Debug.Log($"Un événement se déclenche au jour {CurrentDay.day}");
            _narrativeEvents[CurrentDay.day]?.Invoke();
        }
    }
}