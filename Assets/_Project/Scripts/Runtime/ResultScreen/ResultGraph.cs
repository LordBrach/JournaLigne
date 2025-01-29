using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultGraph : MonoBehaviour
{
    [Header("Base values for each line")]
    [SerializeField] private SO_DataFeedback DictatorsRef;
    [SerializeField] private SO_DataFeedback RebelsRef;
    [SerializeField] private SO_DataFeedback PeopleRef;
    [Space(10)]
    [Header("References")]
    [SerializeField] GameObject canvas;
    [Space(10)]
    [Header("Other Parameters")]
    // the max value each line may reach (0 to X)
    [SerializeField] int yMax = 100;
        int median => yMax / 2;
    [SerializeField] float sizeNode = 6.0f;
    [SerializeField] float sizeLine = 10.0f;

    // value for each step (length between two nodes on the graph ( o---------o )
    [SerializeField, Range(5,100)] float xSize = 50;

    // Non visible params
    float graphHeight = 1;
    private RectTransform graphContainer;
    // Data
    PartyData civilData = new PartyData();
    PartyData dictData = new PartyData();
    PartyData rebelData = new PartyData();

    #region begin setup
    private void Awake()
    {
        graphContainer = canvas.GetComponent<RectTransform>();
        graphHeight = graphContainer.sizeDelta.y;
        canvas.SetActive(false);
    }
    // Start is called before the first frame update
    void Start()
    {
        SetupFirstNodes();
        canvas.SetActive(true);
    }

    private void SetupFirstNodes()
    {
        AddSingleValue(RebelsRef.currentGraphValue, RebelsRef, rebelData);
        AddSingleValue(DictatorsRef.currentGraphValue, DictatorsRef, dictData);
        AddSingleValue(PeopleRef.currentGraphValue, PeopleRef, civilData);
    }
    #endregion

#if UNITY_EDITOR
    void Update()
    {
        DebugKeys();
    }
#endif


    #region Graph
    // Function to be used to send the value to a party from outside this class
    public void AddValueToAParty(Parties _party, float _val)
    {
        switch (_party)
        {
            case global::Parties.Rebels:
                AddSingleValue(_val, RebelsRef, rebelData);
                break;
            case global::Parties.Dictatorship:
                AddSingleValue(_val, DictatorsRef, dictData);
                break;
            case global::Parties.People:
                AddSingleValue(_val, PeopleRef, civilData);
                break;
            default:
                break;
        }
    }

    // Show the entire line of a saved graph
    void ShowEntireLine(SO_DataFeedback _InParty, PartyData data)
    {
        for (int i = 0; i < data.StoredValues.Count; i++)
        {
            float xPos = i * xSize;
            float yPos = (data.StoredValues[i] / yMax) * graphHeight;
            data.CurrentDecal = i;
            AddSingleValue(data.StoredValues[i], _InParty, data);
        }
    }
    // Add one value to one line of the graph
    void AddSingleValue(float _inValue, SO_DataFeedback _InParty, PartyData data)
    {
        Debug.Log(data.CurrentVal + " + " + _inValue);
        float value = data.CurrentVal += _inValue;
        value = Mathf.Clamp(value, 0, 100);
        Debug.Log("= " + value);
        float xPos = data.CurrentDecal * xSize;
        if (xPos > graphContainer.sizeDelta.x)
            xPos = graphContainer.sizeDelta.x;

        float yPos = (value / yMax) * graphHeight;
        CreateNode(value, _InParty, new Vector2(xPos, yPos), data);
        data.StoredValues.Add(value);
        data.CurrentVal = value;
        data.CurrentDecal= data.CurrentDecal += 1;
    }
    // Create a single graph node (intersections between lines)
    void CreateNode(float _newVal, SO_DataFeedback _inParty, Vector2 _position, PartyData data)
    {
        GameObject go = new GameObject("node", typeof(Image));
        // set the node's values (color, pos, size)
        go.transform.SetParent(canvas.transform, false);
        RectTransform rectTransform = go.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = _position;
        rectTransform.sizeDelta = new Vector2(sizeNode, sizeNode);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        go.GetComponent<Image>().color = _inParty.colorGraph;

        if (data.ReffedObjects.Count > 0)
        {
            // Create connexion beween this new node and the previous
            CreateConnexion(go.GetComponent<RectTransform>().anchoredPosition,
                data.ReffedObjects[data.ReffedObjects.Count - 1].GetComponent<RectTransform>().anchoredPosition, _inParty.colorGraph);
        }
        data.ReffedObjects.Add(go);
        data.CurrentVal = _newVal;
    }

    // connect two graph nodes
    void CreateConnexion(Vector2 _posOrigin, Vector2 _posTarget, Color _color)
    {
        GameObject go = new GameObject("dotConnexion", typeof(Image));
        go.transform.SetParent(canvas.transform, false);
        go.GetComponent<Image>().color = _color;
        RectTransform _rectTransform = go.GetComponent<RectTransform>();
        Vector2 direction = (_posTarget - _posOrigin).normalized;
        float dist = Vector2.Distance(_posOrigin, _posTarget);
        _rectTransform.anchorMin = new Vector2(0, 0);
        _rectTransform.anchorMax = new Vector2(0, 0);
        _rectTransform.sizeDelta = new Vector2(dist, sizeLine);
        _rectTransform.anchoredPosition = _posOrigin + direction * dist * 0.5f;
        _rectTransform.localEulerAngles = new Vector3(0, 0, MakeAngleFromVector(direction));
    }
    #endregion
    #region utilities
    void DebugKeys()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            float a = UnityEngine.Random.Range(-30, 30);
            AddSingleValue(a, RebelsRef, rebelData);
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            float a = UnityEngine.Random.Range(-20, 40);
            AddSingleValue(a, DictatorsRef, dictData);
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            float a = UnityEngine.Random.Range(-10, 10);
            AddSingleValue(a, PeopleRef, civilData);
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            foreach (var item in rebelData.StoredValues)
            {
                Debug.Log("Stored val:" + item);
            }
        }
    }
    float MakeAngleFromVector(Vector2 _inVec)
    {
        float angle = Mathf.Atan2(_inVec.y, _inVec.x);
        var deg = 180 * angle / Mathf.PI;
        return (360 + Mathf.Round(deg)) % 360;
    }
    #endregion
}
public enum Parties
{
    Rebels,
    Dictatorship,
    People
};

public class PartyData
{
    int currentDecal = 0;
    List<float> storedValues = new List<float>();
    List<GameObject> reffedObjects = new List<GameObject>();
    float currentVal = 0;

    public List<float> StoredValues { get => storedValues; set => storedValues = value; }
    public List<GameObject> ReffedObjects { get => reffedObjects; set => reffedObjects = value; }
    public float CurrentVal
    {
        get => currentVal;
        set => currentVal = value;
    }
    public int CurrentDecal { get => currentDecal; set => currentDecal = value; }
}