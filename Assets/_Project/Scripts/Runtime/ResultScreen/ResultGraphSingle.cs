using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ResultGraphSingle : MonoBehaviour
{
    [SerializeField] private SO_DataFeedback PeopleRef;
    [Space(10)]
    [SerializeField] GameObject canvas;
    [Space(10)]
    // the max value each line may reach (0 to X)
    [SerializeField] int yMax = 100;
    [SerializeField] private Color ColorAbvTreshhold = Color.blue;
    [SerializeField] private Color ColorBlwTreshhold = Color.blue;
    [SerializeField] float sizeLine = 100.0f;

    // Non visible params
    float graphHeight = 1;
    private float graphWidth = 1;
    private Color graphColor = Color.white;
    Vector2 originPos;
    float dist;
    // coroutine ref
    Coroutine coroutine = null;
    bool isCoroutineRunning = false;

    // Data graph
    float CurrentVal = 0;

    // References
    GameObject lineReference;
    RectTransform graphContainer;

    // Event Dispatcher
    public static Action OnButtonClickedDispatcher;

    public void OnButtonClicked()
    {
        OnButtonClickedDispatcher();
    }
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
        originPos = new Vector2(graphWidth / 2, 0);
        SetupGraph();
    }

    private void OnDisable()
    {
        NoteBook.instance.OnNewsPaperValidate -= HandleNewsPaperValidation;
    }

    private void Start()
    {
        Init();
    }
    private void HandleNewsPaperValidation(Appreciations appreciations)
    {
        canvas.SetActive(true);
        AddValue(appreciations.peopleAppreciation);
    }
    private void Awake()
    {
        graphContainer = canvas.GetComponent<RectTransform>();
        graphHeight = graphContainer.sizeDelta.y;
        graphWidth = graphContainer.sizeDelta.x;
    }
    #endregion

    private void SetupGraph()
    {
        if (PeopleRef != null)
            CurrentVal = PeopleRef.BaseGraphValue;
        CreateLine();
    }

    private void CreateLine()
    {
        lineReference = new GameObject("dotConnexion", typeof(UnityEngine.UI.Image));
        RectTransform _rectTransform = lineReference.GetComponent<RectTransform>();
        lineReference.transform.SetParent(canvas.transform, false);
        lineReference.GetComponent<UnityEngine.UI.Image>().color = graphColor;

        _rectTransform.pivot = new Vector2(0, 0.5f);
        _rectTransform.anchorMin = new Vector2(0, 0);
        _rectTransform.anchorMax = new Vector2(0, 0);
        _rectTransform.anchoredPosition = originPos;
        _rectTransform.sizeDelta = new Vector2(sizeLine, 0);
        //_rectTransform.localEulerAngles = new Vector3(0, 0, MakeAngleFromVector(direction));
    }

    private void AddValue(float _inValue)
    {
        float value = CurrentVal + _inValue;
        float xPos = 0;
        float yPos = 0;

        Debug.Log(CurrentVal + " + " + _inValue);
        value = Mathf.Clamp(value, -100, 100);
        CurrentVal = value;
        Debug.Log("= " + value);
        xPos = graphWidth / 2;
        yPos = ((value / yMax) * graphHeight);
        yPos = Mathf.Clamp(yPos, 0, graphHeight);

        // Change color based on the value
        graphColor = ColorBlwTreshhold;
        if (yPos > graphHeight / 2)
        {
            graphColor = ColorAbvTreshhold;
        }

        UpdateGraph(new Vector2(xPos, yPos));
    }

    private void UpdateGraph(Vector2 targetPos)
    {
        RectTransform _rectTransform = lineReference.GetComponent<RectTransform>();
        if (isCoroutineRunning)
        {
            isCoroutineRunning = false;
            StopCoroutine(coroutine);
            _rectTransform.sizeDelta = new Vector2(sizeLine, dist);
            // finish drawing the line
        }

        Vector2 direction = (targetPos - originPos).normalized;
        dist = Vector2.Distance(originPos, targetPos);
        lineReference.GetComponent<UnityEngine.UI.Image>().color = graphColor;

        coroutine = StartCoroutine(DrawLine(_rectTransform, dist));
    }

    IEnumerator DrawLine(RectTransform _inRectTransform, float dist)
    {
        isCoroutineRunning = true;
        _inRectTransform.pivot = new Vector2(0.5f, 0f);
        float duration = 1.0f;
        float timestep = 0;
        float y = _inRectTransform.sizeDelta.y;
        Debug.Log("y: " + y);
        Debug.Log("target pos: " + dist);
        while (timestep <= duration)
        {
            timestep += Time.deltaTime;
            float step = Mathf.Clamp01(timestep / duration);
            _inRectTransform.sizeDelta = new Vector2(sizeLine, Mathf.Lerp(y, dist, step));
            yield return null;
        }
        _inRectTransform.sizeDelta = new Vector2(sizeLine, dist);
    }

#if UNITY_EDITOR
    void Update()
    {
        DebugKeys();
    }
#endif
    void DebugKeys()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            float value = UnityEngine.Random.Range(-30, 60);
            AddValue(value);
        }
    }
}


/*
 private void CreateNode(Vector2 nodePosition)
    {
        originNodeReference = new GameObject("node", typeof(UnityEngine.UI.Image));
        originNodeReference.transform.SetParent(canvas.transform, false);
        RectTransform rectTransform = originNodeReference.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = nodePosition;
        rectTransform.sizeDelta = new Vector2(0, 0);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        originNodeReference.GetComponent<UnityEngine.UI.Image>().color = graphColor;

    }
public void Setup()
{
    private void SetupFirstNodes()
    {
        Debug.Log("setting up");
        lineObject = new GameObject("Line");

        // Add LineRenderer component
        lr = lineObject.AddComponent<LineRenderer>();

        // Configure the LineRenderer
        lr.positionCount = 2;
        lr.SetPosition(0, new Vector3(0, 0, 0)); // Start point
        lr.SetPosition(1, new Vector3(0, 0, 0)); // End point

        lr.startWidth = sizeLine;
        lr.endWidth = sizeLine;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        isGraphSetup = true;
    }
}



public void UpdateLine(float _inValue)
    {
        currentValue += _inValue;
        currentValue = Mathf.Clamp(currentValue, 0, 100);

        if (isCoroutineRunning)
        {
            isCoroutineRunning = false;
            StopCoroutine(coroutine);
            // finish drawing the line
        }
        coroutine = StartCoroutine(LineDraw(currentValue));
    }

    IEnumerator LineDraw(float newValue)
    {
        isCoroutineRunning = true;
        float t = 0;
        float time = 2;
        Vector3 orig = lr.GetPosition(0);
        Vector3 orig2 = new Vector3(0, newValue, 0);
        lr.SetPosition(1, orig);
        Vector3 newpos;
        for (; t < time; t += Time.deltaTime)
        {
            newpos = Vector3.Lerp(orig, orig2, t / time);
            lr.SetPosition(1, newpos);
            yield return null;
        }
        lr.SetPosition(1, orig2);
    }*/