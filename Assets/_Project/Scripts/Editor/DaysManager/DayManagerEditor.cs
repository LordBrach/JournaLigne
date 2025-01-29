using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(DayManager))]
public class DayManagerEditor : Editor
{
    private SerializedProperty daysInWeek;
    private SerializedProperty daysList;
    private SerializedProperty graphs;

    private void OnEnable()
    {
        daysInWeek = serializedObject.FindProperty("daysInWeek");
        daysList = serializedObject.FindProperty("daysList");
        graphs = serializedObject.FindProperty("graphs");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("Day Manager Settings", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(daysInWeek, new GUIContent("Days in a Week"));
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Days List", EditorStyles.boldLabel);

        for (int i = 0; i < daysList.arraySize; i++)
        {
            SerializedProperty dayElement = daysList.GetArrayElementAtIndex(i);
            SerializedProperty dayNumber = dayElement.FindPropertyRelative("day");
            SerializedProperty dayType = dayElement.FindPropertyRelative("dayType");
            SerializedProperty currentGraph = dayElement.FindPropertyRelative("currentGraph");
            
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Day " + (i + 1), EditorStyles.boldLabel);
            
            dayNumber.intValue = EditorGUILayout.IntField("Day Number", dayNumber.intValue);
            dayType.enumValueIndex = EditorGUILayout.Popup("Day Type", dayType.enumValueIndex, dayType.enumDisplayNames);
            EditorGUILayout.PropertyField(currentGraph, new GUIContent("Graph"));

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Remove Day"))
            {
                daysList.DeleteArrayElementAtIndex(i);
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.EndVertical();
        }
        
        if (GUILayout.Button("Add New Day"))
        {
            daysList.arraySize++;
        }
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Graphs", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(graphs, true);
        
        serializedObject.ApplyModifiedProperties();
    }
}