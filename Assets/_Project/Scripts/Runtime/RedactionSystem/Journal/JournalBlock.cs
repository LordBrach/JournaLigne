using TMPro;
using UnityEngine;

public class JournalBlock : MonoBehaviour
{
    public float multiplier;
    public TextMeshProUGUI text;

    private void Awake()
    {
        text.text = "x " + multiplier;
    }
}
