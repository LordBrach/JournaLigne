using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

[DefaultExecutionOrder(-1000)]
public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance { get; private set; }
    public static event Action OnLanguageChanged;
    
    private Dictionary<string, Dictionary<string, string>> _translations = new Dictionary<string, Dictionary<string, string>>();
    private string _currentLanguage = "en";
    
    private void Awake()
    {
        Initialization();
        LoadTranslations();
    }

    private void Start()
    {
        LoadTranslations();
    }

    public void Initialization()
    {
        if (Instance == null)
        {
            Instance = this;
            if (Application.isPlaying)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }
    
    public void SetLanguage(string language)
    {
        _currentLanguage = language;
        Debug.Log("Language set to: " + language);
        OnLanguageChanged?.Invoke();
    }
    
    public List<string> GetAllTranslationKeys()
    {
        List<string> keys = new List<string>(_translations.Keys);
        return keys;
    }

    public string GetTranslation(string key)
    {
        if (_translations.ContainsKey(key) && _translations[key].ContainsKey(_currentLanguage))
        {
            return _translations[key][_currentLanguage];
        }

        return $"Missing[{key}]";
    }

    public string GetCurrentLanguage()
    {
        return _currentLanguage;
    }
    
    public void LoadTranslations()
    {
        TextAsset jsonAsset = Resources.Load<TextAsset>("translations");

        if (jsonAsset != null)
        {
            Debug.Log("Loading translations from Resources.");
            ApplyTranslations(jsonAsset.text);
        }
        else
        {
            Debug.LogError("Translation file not found in Resources.");
        }
    }

    private void ApplyTranslations(string json)
    {
        if (string.IsNullOrEmpty(json))
        {
            Debug.LogError("Translation JSON is empty or null.");
            return;
        }

        try
        {
            LocalizationData.TranslationList translationList = JsonUtility.FromJson<LocalizationData.TranslationList>(json);

            if (translationList == null || translationList.translations == null)
            {
                Debug.LogError("Translation data is not in the expected format.");
                return;
            }

            _translations.Clear();

            foreach (var translationData in translationList.translations)
            {
                Dictionary<string, string> translationDict = new Dictionary<string, string>();
                foreach (var langData in translationData.translations)
                {
                    translationDict[langData.language] = langData.translation;
                }
                _translations[translationData.key] = translationDict;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error parsing translation JSON: " + ex.Message);
        }
    }
}