using TMPro;
using UnityEngine;

public class TranslationComponent : MonoBehaviour
{
    [HideInInspector]
    public string _localizationKey;
    public bool _isTranslatable = true;
    private TMP_Text _textComponent;

    public void RefreshText() => UpdateLocalizedText();

    private void Awake()
    {
        _textComponent = GetComponent<TMP_Text>();
    }

    private void Start()
    {
        UpdateLocalizedText();
    }

    private void OnEnable()
    {
        LocalizationManager.OnLanguageChanged += UpdateLocalizedText;
    }

    private void OnDisable()
    {
        LocalizationManager.OnLanguageChanged -= UpdateLocalizedText;
    }

    public TMP_Text GetTextComponent()
    {
        return _textComponent;
    }

    private void UpdateLocalizedText()
    {
        if (_isTranslatable && Application.isPlaying && !string.IsNullOrEmpty(_localizationKey))
        {
            _textComponent.text = LocalizationManager.Instance.GetTranslation(_localizationKey);
        }
    }
}