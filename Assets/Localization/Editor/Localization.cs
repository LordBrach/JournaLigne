using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using static UnityEngine.GUILayout;

public class Localization : EditorWindow
{
    private static Dictionary<string, Dictionary<string, string>> _translations = new Dictionary<string, Dictionary<string, string>>();
    private List<string> _languages = new List<string> { "en", "fr", "ja" };
    private string[] _languageLabels = new string[] { "English (en)", "French (fr)", "Japanese (ja)" };
    private Vector2 _scrollPosition;
    
    private int _selectedTab = 0;
    private Vector2 _textMeshProScrollPosition;
    private List<TextMeshProUGUI> _translateObjects;
    public List<TextMeshProUGUI> GetTexts() => _translateObjects;

    [MenuItem("Tools/Localization Editor")]
    public static void ShowWindow()
    {
        GetWindow<Localization>("Localization Editor");
    }
    
    private void OnEnable()
    {
        LoadTranslations();
        FindAllTranslateObjects();
    }
    
    private void OnGUI()
    {
        _selectedTab = GUILayout.Toolbar(_selectedTab, new string[] { "Localization Table", "Text Objects" });

        switch (_selectedTab)
        {
            case 0:
                DrawLocalizationTable();
                break;
            case 1:
                DrawTextMeshProObjects();
                break;
        }
        
    }
    
    private void DrawLocalizationTable()
    {
        Label("Localization Table", EditorStyles.boldLabel);
        _scrollPosition = BeginScrollView(_scrollPosition, false, true);
        
        Dictionary<int, float> langWidths = new Dictionary<int, float>();

        foreach (var entry in _translations)
        {
            foreach (var langEntry in entry.Value)
            {
                float textWidth = GUI.skin.textField.CalcSize(new GUIContent(langEntry.Value)).x + 20;

                if (!langWidths.ContainsKey(entry.Value.Keys.ToList().IndexOf(langEntry.Key)))
                {
                    langWidths[entry.Value.Keys.ToList().IndexOf(langEntry.Key)] = textWidth;
                }
                else
                {
                    langWidths[entry.Value.Keys.ToList().IndexOf(langEntry.Key)] = Mathf.Max(langWidths[entry.Value.Keys.ToList().IndexOf(langEntry.Key)], textWidth);
                }
            }
        }
        
        float maxKeyWidth = 200;
        foreach (var entry in _translations)
        {
            maxKeyWidth = Mathf.Max(maxKeyWidth, GUI.skin.label.CalcSize(new GUIContent(entry.Key)).x + 20);
            
        }
        
        BeginHorizontal();
        GUILayout.Label("Key", Width(maxKeyWidth + 20));
        for (int i = 0; i < _languages.Count; i++)
        {
            _languages[i] = EditorGUILayout.TextField(_languages[i], GUILayout.Width(langWidths[i] - 23));
    
            if (GUILayout.Button("X", Width(20)))
            {
                RemoveLanguage(_languages[i]);
                break;
            }
        }
        EndHorizontal();
        
        Dictionary<string, string> keysToRename = new Dictionary<string, string>();
        List<string> keysToRemove = new List<string>();
        Dictionary<string, Dictionary<string, string>> newTranslations = new Dictionary<string, Dictionary<string, string>>();
        
        List<string> keys = new List<string>(_translations.Keys);

        foreach (var key in keys)
        {
            BeginHorizontal();

            // Bouton pour supprimer la clé
            if (GUILayout.Button("X", GUILayout.Width(20)))
            {
                keysToRemove.Add(key);
            }

            // Saisie pour modifier la clé
            string newKey = EditorGUILayout.TextField(key, GUILayout.Width(maxKeyWidth));
            if (newKey != key && !keysToRename.ContainsKey(key) && !_translations.ContainsKey(newKey))
            {
                keysToRename[key] = newKey;
            }

            // Affichage des traductions
            for (int j = 0; j < _languages.Count; j++)
            {
                string lang = _languages[j];
                if (!_translations[key].ContainsKey(lang))
                {
                    _translations[key][lang] = "";
                }
                _translations[key][lang] = EditorGUILayout.TextField(_translations[key][lang], GUILayout.Width(langWidths[j]));
            }

            EndHorizontal();
        }

        // Suppression des clés marquées
        foreach (var keyToRemove in keysToRemove)
        {
            _translations.Remove(keyToRemove);
        }

        // Application des renommages en recréant un dictionnaire propre
        foreach (var kvp in _translations)
        {
            string actualKey = keysToRename.ContainsKey(kvp.Key) ? keysToRename[kvp.Key] : kvp.Key;
            newTranslations[actualKey] = new Dictionary<string, string>(kvp.Value);
        }

        // Remplacement du dictionnaire pour éviter des conflits d'ordre
        _translations = newTranslations;

        EndScrollView();
        
        // Section : Gestion des Clés et Langues
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Keys & Languages", EditorStyles.boldLabel);
        if (Button("Add New Key")) AddNewKey();
        if (Button("Add New Language")) AddNewLanguage();
        EditorGUILayout.EndVertical();

        Space(10);

        // Section : Import / Export
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Import / Export", EditorStyles.boldLabel);
        if (Button("Import CSV")) ImportCsv();
        if (Button("Export CSV")) ExportCsv();
        EditorGUILayout.EndVertical();

        Space(10);

        // Section : Sauvegarde et Restauration
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Save & Restore", EditorStyles.boldLabel);
        if (Button("Save All")) SaveTranslations();

        EditorGUILayout.BeginHorizontal();
        if (Button("Refresh")) RevertAllUnsaved();
        if (Button("Revert All")) RevertAll();
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
    }
    
    // Revert
    #region Revert

    private void RevertAll()
    {
        _translations.Clear();
    }

    private void RevertAllUnsaved()
    {
        _translations.Clear();
        LoadTranslations();
    }

    #endregion

    // Text Component
    #region Text Component

        /*- Draw Window -*/
        private void DrawTextMeshProObjects()
        {
            _scrollPosition = BeginScrollView(_scrollPosition, false, true);

            foreach (var translateObject in _translateObjects)
            {
                if (!translateObject)
                {
                    FindAllTranslateObjects();
                    EndScrollView();
                    return;
                }
                
                BeginHorizontal();
                var translateComponent = translateObject?.GetComponent<TranslationComponent>();
                var textMeshProComponent = translateObject?.GetComponent<TextMeshProUGUI>();

                if (translateComponent && textMeshProComponent)
                {
                    List<string> keys = new List<string>(_translations.Keys);

                    bool currentIsTranslatable = translateComponent._isTranslatable;

                    bool isTranslatable = EditorGUILayout.Toggle(textMeshProComponent.name, currentIsTranslatable, GUILayout.Width(150));
                    if (isTranslatable != currentIsTranslatable)
                    {
                        translateComponent._isTranslatable = isTranslatable;
                        EditorUtility.SetDirty(translateComponent);
                    }

                    if (isTranslatable)
                    {
                        Space(20);
                        int currentIndex = keys.IndexOf(translateComponent._localizationKey);
                        if (currentIndex < 0) currentIndex = 0;
                    
                        int selectedIndex = EditorGUILayout.Popup(
                            "",
                            currentIndex,
                            keys.ToArray(),
                            GUILayout.Width(250)
                        );

                        if (selectedIndex != currentIndex)
                        {
                            string selectedKey = keys[selectedIndex];
                            translateComponent._localizationKey = selectedKey;
                            textMeshProComponent.text = GetTranslation(translateComponent._localizationKey);
                            EditorUtility.SetDirty(translateComponent);
                        }
                    }
                }
                else
                {
                    Debug.LogWarning("Missing TranslationComponent or TextMeshProUGUI on the object.");
                }
                EndHorizontal();
            }
            
            EndScrollView();
            
            if (Button("Refresh Text"))
            {
                FindAllTranslateObjects();
            }
        }
        
        /*- Find all text -*/
        private void FindAllTranslateObjects()
        {
            _translateObjects = new List<TextMeshProUGUI>();

            Dictionary<string, TextMeshProUGUI> prefabTexts = new Dictionary<string, TextMeshProUGUI>();

        #if UNITY_EDITOR
            // Trouver les textes dans les prefabs
            string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab");

            foreach (string guid in prefabGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

                if (prefab != null)
                {
                    bool prefabModified = false;
                    TextMeshProUGUI[] textsInPrefab = prefab.GetComponentsInChildren<TextMeshProUGUI>(true);

                    foreach (var text in textsInPrefab)
                    {
                        if (!prefabTexts.ContainsKey(text.text))
                        {
                            prefabTexts[text.text] = text;
                        }
                        
                        if (PrefabUtility.IsPartOfPrefabInstance(text.gameObject) == false)
                        {
                            RemoveDuplicateComponents<TranslationComponent>(text.gameObject);
                        }

                        if (!text.gameObject.GetComponent<TranslationComponent>())
                        {
                            TranslationComponent comp = text.gameObject.AddComponent<TranslationComponent>();
                            comp._isTranslatable = false;
                            prefabModified = true;
                        }

                        if (!_translateObjects.Contains(text))
                        {
                            _translateObjects.Add(text);
                        }
                    }

                    // Sauvegarde les modifications du prefab
                    if (prefabModified)
                    {
                        EditorUtility.SetDirty(prefab);
                        AssetDatabase.SaveAssets();
                    }
                }
            }
        #endif

            // Trouver les textes dans la scène
            var allObjects = Resources.FindObjectsOfTypeAll<TextMeshProUGUI>();

            foreach (var obj in allObjects)
            {
                if (obj == null || obj.gameObject == null)
                {
                    Debug.LogWarning("Found a null or destroyed object while searching for TextMeshProUGUI.");
                    continue;
                }

                if (prefabTexts.ContainsKey(obj.text))
                {
                    // Si le texte existe aussi dans un prefab, ne pas ajouter le component dans la scène
                    continue;
                }
                
                RemoveDuplicateComponents<TranslationComponent>(obj.gameObject);

                if (!obj.gameObject.GetComponent<TranslationComponent>())
                {
                    TranslationComponent comp = obj.gameObject.AddComponent<TranslationComponent>();
                    comp._isTranslatable = false;
                }

                if (!_translateObjects.Contains(obj))
                {
                    _translateObjects.Add(obj);
                }
            }
        }
        
        private void RemoveDuplicateComponents<T>(GameObject obj) where T : Component
        {
            if (PrefabUtility.IsPartOfPrefabAsset(obj)) return;

            T[] components = obj.GetComponents<T>();
            if (components.Length > 1)
            {
                for (int i = 1; i < components.Length; i++)
                {
                    GameObject.DestroyImmediate(components[i]);
                }
            }
        }

    #endregion

    // Languages
    #region Languages

        /*- New Language -*/
        private void AddNewLanguage()
        {
            string newLanguageKey = "New Language";
            if (!_languages.Contains(newLanguageKey))
            {
                _languages.Add(newLanguageKey);
                Debug.Log($"Added new language: {newLanguageKey}");
            }
            else
            {
                Debug.LogWarning($"The language '{newLanguageKey}' already exists in the dictionary.");
            }
        }
        
        /*- Remove Language -*/
        private void RemoveLanguage(string langToRemove)
        {
            if (_languages.Contains(langToRemove))
            {
                _languages.Remove(langToRemove);
    
                foreach (var key in _translations.Keys)
                {
                    _translations[key].Remove(langToRemove);
                }
    
                Debug.Log($"{langToRemove} removed from translations.");
            }
            else
            {
                Debug.LogWarning("Language to remove does not exist.");
            }
        }

    #endregion

    // Keys
    #region Keys

        /*- New Key -*/
        private void AddNewKey()
        {
            string newKey = "New Key";
            if (!_translations.ContainsKey(newKey))
            {
                _translations.Add(newKey, new Dictionary<string, string>());
                foreach (var lang in _languages)
                {
                    _translations[newKey].Add(lang, "");
                }
            }
        }
        
        /*- Remove Key -*/
        private void RemoveKey(string key)
        {
            if (_translations.ContainsKey(key))
            {
                _translations.Remove(key);
                Debug.Log($"Key '{key}' removed successfully.");
            }
            else
            {
                Debug.LogWarning($"Key '{key}' does not exist.");
            }
        }

    #endregion

    // Save
    #region Save
    
    /*- Saving -*/
    private void SaveTranslations()
    {
        string resourcesPath = Path.Combine(Application.dataPath, "Resources");
        if (!Directory.Exists(resourcesPath))
        {
            Directory.CreateDirectory(resourcesPath);
        }

        string filePath = Path.Combine(resourcesPath, "translations.json");
        if (!File.Exists(filePath))
        {
            File.Create(filePath).Close();
        }

        TranslationList translationList = new TranslationList();
        foreach (var keyValuePair in _translations)
        {
            translationList.translations.Add(new TranslationData(keyValuePair.Key, keyValuePair.Value));
        }

        string json = JsonUtility.ToJson(translationList, true);
        File.WriteAllText(filePath, json);

        #if UNITY_EDITOR
        AssetDatabase.Refresh();
        #endif

        Debug.Log("Translations saved to: " + filePath);
    }
        
        /*- Load Translation -*/
        private void LoadTranslations()
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
            TranslationList translationList = JsonUtility.FromJson<TranslationList>(json);
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
    
    #endregion
    
    // Csv
    #region CSV
    
    /*- Import CSV -*/
    private void ImportCsv()
    {
        string path = EditorUtility.OpenFilePanel("Import Localization CSV", "", "csv");
        if (string.IsNullOrEmpty(path)) return;

        try
        {
            // Lire tout le fichier en une seule chaîne
            string fileContent = File.ReadAllText(path);

            // Remplacer les sauts de ligne dans les guillemets par un caractère spécial temporaire
            fileContent = fileContent.Replace("\r\n", "\n");  // Normalise les fins de ligne pour éviter les problèmes entre différentes plateformes
            fileContent = HandleLineBreaksInsideQuotes(fileContent);

            // Diviser le contenu en lignes, chaque ligne étant un texte complet
            string[] lines = fileContent.Split(new string[] { "\n" }, StringSplitOptions.None);

            if (lines.Length < 2)
            {
                Debug.LogError("Invalid CSV format. It must contain headers and at least one row.");
                return;
            }

            string[] headers = lines[0].Split(',');
            if (headers.Length < 2)
            {
                Debug.LogError("Invalid CSV headers. It must contain at least a Key column and one language.");
                return;
            }

            List<string> newLanguages = new List<string>();
            for (int i = 1; i < headers.Length; i++)
            {
                string lang = headers[i].Split('(')[0].Trim();
                if (!_languages.Contains(lang))
                {
                    _languages.Add(lang);
                }
                newLanguages.Add(lang);
            }

            // Parse rows
            for (int i = 1; i < lines.Length; i++)
            {
                string[] columns = ParseCsvLineWithLineBreaks(lines[i]);

                if (columns.Length < 2) continue;

                string key = columns[0];
                if (!_translations.ContainsKey(key))
                {
                    _translations[key] = new Dictionary<string, string>();
                }

                for (int j = 1; j < columns.Length; j++)
                {
                    string lang = newLanguages[j - 1];
                    string translation = columns[j].Trim('"');
                    _translations[key][lang] = translation;
                }
            }

            Debug.Log("CSV imported successfully.");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error importing CSV: {ex.Message}");
        }
    }

    private string HandleLineBreaksInsideQuotes(string fileContent)
    {
        var regex = new System.Text.RegularExpressions.Regex("\"([^\"]*)\"");
        fileContent = regex.Replace(fileContent, match =>
        {
            string field = match.Groups[1].Value;
            field = field.Replace("\n", " [LINE_BREAK] ");
            return $"\"{field}\"";
        });

        return fileContent;
    }

    private string[] ParseCsvLineWithLineBreaks(string line)
    {
        List<string> result = new List<string>();
        bool insideQuote = false;
        string currentField = "";

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (c == '"' && (i == 0 || line[i - 1] != '\\'))
            {
                insideQuote = !insideQuote;
            }
            else if (c == ',' && !insideQuote)
            {
                result.Add(currentField);
                currentField = "";
            }
            else
            {
                currentField += c;
            }
        }

        if (!string.IsNullOrEmpty(currentField))
        {
            result.Add(currentField);
        }

        for (int i = 0; i < result.Count; i++)
        {
            result[i] = result[i].Replace(" [LINE_BREAK] ", "\n");
        }

        return result.ToArray();
    }


    private void ExportCsv()
    {
        string path = EditorUtility.SaveFilePanel("Export Localization CSV", "", "Localization.csv", "csv");
        if (string.IsNullOrEmpty(path)) return;

        try
        {
            // Préparer l'en-tête du CSV
            List<string> lines = new List<string>();
            string header = "Key," + string.Join(",", _languages);
            lines.Add(header);

            // Ajouter chaque clé et ses traductions
            foreach (var entry in _translations)
            {
                string key = entry.Key;
                List<string> row = new List<string> { key };

                foreach (string language in _languages)
                {
                    if (entry.Value.TryGetValue(language, out string translation))
                    {
                        // Traiter la traduction pour ajouter des guillemets autour de chaque texte, même s'il n'y a pas de caractères spéciaux
                        row.Add(AddQuotesToTranslation(translation));
                    }
                    else
                    {
                        row.Add(""); // Valeur vide si la traduction est absente
                    }
                }

                // Ajouter la ligne au CSV
                lines.Add(string.Join(",", row));
            }

            // Écrire dans le fichier
            File.WriteAllLines(path, lines);
            Debug.Log($"CSV exported successfully to {path}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error exporting CSV: {ex.Message}");
        }
    }

// Fonction pour ajouter des guillemets autour du texte, et échapper les guillemets internes
    private string AddQuotesToTranslation(string translation)
    {
        translation = "\"" + translation.Replace("\"", "\"\"") + "\"";
        return translation;
    }

    #endregion
    
    public static List<string> GetAllTranslationKeys()
    {
        List<string> keys = new List<string>(_translations.Keys);
        return keys;
    }
    
    public static string GetTranslation(string key)
    {
        if (_translations.ContainsKey(key) && _translations[key].ContainsKey("en"))
        {
            return _translations[key]["en"];
        }

        return $"Missing[{key}]";
    }

    // Data
   #region Data
   
        [System.Serializable]
        public class TranslationData
        {
            public string key;
            public List<LanguageData> translations;
        
            public TranslationData(string key, Dictionary<string, string> translationsDict)
            {
                this.key = key;
                translations = new List<LanguageData>();
                foreach (var lang in translationsDict)
                {
                    translations.Add(new LanguageData(lang.Key, lang.Value));
                }
            }
        }
        
        [System.Serializable]
        public class LanguageData
        {
            public string language;
            public string translation;
        
            public LanguageData(string language, string translation)
            {
                this.language = language;
                this.translation = translation;
            }
        }
        
        [System.Serializable]
        public class TranslationList
        {
            public List<TranslationData> translations = new List<TranslationData>();
        }
   
   #endregion
}