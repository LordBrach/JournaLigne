using System;
using System.Collections;
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
    [SerializeField] typeGraph GraphType = typeGraph.Single;
    // the max value each line may reach (0 to X)
    [SerializeField] int yMax = 100;
        int median => yMax / 2;
    [SerializeField] private Color ColorAbvTreshhold = Color.blue;
    [SerializeField] private Color ColorBlwTreshhold = Color.blue;

    [SerializeField] float sizeNode = 6.0f;
    [SerializeField] float sizeLine = 10.0f;
    // value for each step (length between two nodes on the lines graph ( o---------o )
    [SerializeField, Range(5,100)] float xSize = 50;

    // Non visible params
    float graphHeight = 1;
    private float graphWidth;
    private Color setColorSingle = Color.white;

    bool isGraphSetup = false;
    private RectTransform graphContainer;
    // Values for single
    private GameObject storedLine;
    private GameObject storedNode;
    // Data
    PartyData civilData = new PartyData();
    PartyData dictData = new PartyData();
    PartyData rebelData = new PartyData();
    // coroutine ref
    Coroutine coroutine = null;
    bool isCoroutineRunning = false;

    #region Show/Hide
    public void Show()
    {
        canvas.SetActive(true);
    }
    public void Hide()
    {
        canvas.SetActive(false);
    }
    #endregion


    #region begin setup

    public void Init()
    {
        NoteBook.instance.OnNewsPaperValidate += HandleNewsPaperValidation;
    }
    
    private void OnEnable()
    {
        // NoteBook.instance.OnNewsPaperValidate += HandleNewsPaperValidation;
    }


    private void OnDisable()
    {
        NoteBook.instance.OnNewsPaperValidate -= HandleNewsPaperValidation;        
    }

    private void HandleNewsPaperValidation(Appreciations appreciations)
    {
        if(!isGraphSetup)
        {
            SetupFirstNodes();
        }
        canvas.SetActive(true);
        AddSingleValue(appreciations.peopleAppreciation, PeopleRef, civilData);
    }
    private void Awake()
    {
        graphContainer = canvas.GetComponent<RectTransform>();
        graphHeight = graphContainer.sizeDelta.y;
        graphWidth = graphContainer.sizeDelta.x;
    }
    // Start is called before the first frame update
    void Start()
    {
        if (!isGraphSetup)
        {
            SetupFirstNodes();
        }
    }

    private void SetupFirstNodes()
    {
        AddSingleValue(PeopleRef.CurrentGraphValue, PeopleRef, civilData);
        isGraphSetup = true;
        //AddSingleValue(RebelsRef.currentGraphValue, RebelsRef, rebelData);
        //AddSingleValue(DictatorsRef.currentGraphValue, DictatorsRef, dictData);
    }

#if UNITY_EDITOR
    void Update()
    {
        DebugKeys();
    }
#endif
    #endregion


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
        float value = data.CurrentVal + _inValue;
        float xPos = 0;
        float yPos = 0;

        Debug.Log(data.CurrentVal + " + " + _inValue);
        value = Mathf.Clamp(value, -100, 100);
        data.CurrentVal = value;
        Debug.Log("= " + value);

        switch (GraphType)
        {
            case typeGraph.Lines:
                xPos = data.CurrentDecal * xSize;
                if (xPos > graphContainer.sizeDelta.x)
                    xPos = graphContainer.sizeDelta.x;
                yPos = (value / yMax) * graphHeight;
                //setColorSingle = _InParty.colorGraph;
                CreateNode(value, _InParty, new Vector2(xPos, yPos), data, true);
                data.StoredValues.Add(value);
                data.CurrentVal = value;
                data.CurrentDecal = data.CurrentDecal += 1;
                break;
            case typeGraph.Single:
                xPos = graphWidth / 2;
                yPos = ((value / yMax) * graphHeight);
                yPos = Mathf.Clamp(yPos, 0, graphHeight);

                if(yPos > graphHeight/2)
                {
                    setColorSingle = ColorAbvTreshhold;
                } else
                {
                    setColorSingle = ColorBlwTreshhold;
                }

                CreateNode(value, _InParty, new Vector2(xPos, yPos), data, false);
                data.CurrentVal = value;
                break;
            default:
                break;
        }
    }

    // Create a single graph node (intersections between lines)
    void CreateNode(float _newVal, SO_DataFeedback _inParty, Vector2 _position, PartyData data, bool saveNode)
    {
        GameObject go = new GameObject("node", typeof(Image));
        SetupNodeValues(_inParty, _position, go);

        if (data.ReffedObjects.Count > 0)
        {
            // Create connexion beween this new node and the previous
            CreateConnexion(
                data.ReffedObjects[data.ReffedObjects.Count - 1].GetComponent<RectTransform>().anchoredPosition,
                go.GetComponent<RectTransform>().anchoredPosition
                );
        }
        // delete previous node if needed (ie: when only wanting a single line)
        if(saveNode == false && data.ReffedObjects.Count > 0)
        {
            Destroy(data.ReffedObjects[data.ReffedObjects.Count - 1]);
            data.ReffedObjects.RemoveAt(data.ReffedObjects.Count - 1);
        } else
        {
            data.ReffedObjects.Add(go);
        }
        data.CurrentVal = _newVal;
    }
    // connect two graph nodes
    void CreateConnexion(Vector2 _posOrigin, Vector2 _posTarget)
    {
        switch (GraphType)
        {
            case typeGraph.Single:
                if(storedLine == null) {
                    storedLine = BaseConnexionTwoNodes(_posOrigin, _posTarget, false);
                } else {
                    UpdateConnectionSingle(_posOrigin, _posTarget);
                }
                break;

            default:
                BaseConnexionTwoNodes(_posOrigin, _posTarget, true);

                break;
        }
    }

    private GameObject BaseConnexionTwoNodes(Vector2 _posOrigin, Vector2 _posTarget, bool Angled)
    {
        GameObject go = new GameObject("dotConnexion", typeof(Image));
        RectTransform _rectTransform = go.GetComponent<RectTransform>();
        float dist = SetupConnexionValues(_posOrigin, _posTarget, go, _rectTransform, Angled);

        if (isCoroutineRunning)
        {
            isCoroutineRunning = false;
            StopCoroutine(coroutine);
            _rectTransform.sizeDelta = new Vector2(dist, sizeLine);
            // finish drawing the line
        }
        coroutine = StartCoroutine(DrawLine(_rectTransform, dist));
        return go;
    }

    private void UpdateConnectionSingle(Vector2 _posOrigin, Vector2 _posTarget)
    {
        RectTransform _rectTransform = storedLine.GetComponent<RectTransform>();
        float dist = SetupConnexionValues(_posOrigin, _posTarget, storedLine, _rectTransform, false);
        if (isCoroutineRunning)
        {
            isCoroutineRunning = false;
            StopCoroutine(coroutine);
            _rectTransform.sizeDelta = new Vector2(dist, sizeLine);
            // finish drawing the line
        }
        coroutine = StartCoroutine(UpdateLine(_rectTransform, dist));
    }

    // set the node's values (color, pos, size)
    private void SetupNodeValues(SO_DataFeedback _inParty, Vector2 _position, GameObject go)
    {
        go.transform.SetParent(canvas.transform, false);
        RectTransform rectTransform = go.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = _position;
        rectTransform.sizeDelta = new Vector2(sizeNode, sizeNode);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        go.GetComponent<Image>().color = setColorSingle;
    }

    private float SetupConnexionValues(Vector2 _posOrigin, Vector2 _posTarget, GameObject go, RectTransform _rectTransform, bool Angled)
    {
        go.transform.SetParent(canvas.transform, false);
        go.GetComponent<Image>().color = setColorSingle;
        // Calculate direction and distance
        Vector2 direction = (_posTarget - _posOrigin).normalized;
        float dist = Vector2.Distance(_posOrigin, _posTarget);
        if (Angled)
            AngleConnexion(_posOrigin, _rectTransform, direction);
        return dist;
    }

    private void AngleConnexion(Vector2 _posOrigin, RectTransform _rectTransform, Vector2 direction)
    {
        // Set the pivot to the left side (0, 0.5) for horizontal scaling
        _rectTransform.pivot = new Vector2(0, 0.5f);
        // Set the anchors to the same point (0, 0) to avoid anchor-related positioning issues
        _rectTransform.anchorMin = new Vector2(0, 0);
        _rectTransform.anchorMax = new Vector2(0, 0);
        // Position the RectTransform at the origin point
        _rectTransform.anchoredPosition = _posOrigin;
        // Rotate the RectTransform to align with the direction
        _rectTransform.localEulerAngles = new Vector3(0, 0, MakeAngleFromVector(direction));
    }

    IEnumerator DrawLine(RectTransform _inRectTransform, float dist)
    {
        isCoroutineRunning = true;
        _inRectTransform.pivot = new Vector2(0f, 0.5f);
        float duration = 1.0f;
        float timestep = 0;
        while(timestep <= duration)
        {
            timestep += Time.deltaTime;
            float step = Mathf.Clamp01(timestep / duration);
            _inRectTransform.sizeDelta = new Vector2(Mathf.Lerp(0, dist, step), sizeLine);
            yield return null;
        }
        _inRectTransform.sizeDelta = new Vector2(dist, sizeLine);
    }
    IEnumerator UpdateLine(RectTransform _inRectTransform, float dist)
    {
        isCoroutineRunning = true;
        _inRectTransform.pivot = new Vector2(0.5f, 0f);
        float duration = 1.0f;
        float timestep = 0;
        while (timestep <= duration)
        {
            timestep += Time.deltaTime;
            float step = Mathf.Clamp01(timestep / duration);
            _inRectTransform.sizeDelta = new Vector2(sizeLine, Mathf.Lerp(0, dist, step));
            yield return null;
        }
        _inRectTransform.sizeDelta = new Vector2(sizeLine, dist);
    }
    #endregion
    #region utilities
    void DebugKeys()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            float a = UnityEngine.Random.Range(-30, 60);
            AddSingleValue(a, PeopleRef, civilData);
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            foreach (var item in civilData.StoredValues)
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

[Serializable]
public enum typeGraph
{
    Lines,
    Single,
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