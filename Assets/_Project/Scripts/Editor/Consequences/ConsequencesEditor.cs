#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Consequences))]
public class ConsequencesEditor : Editor
{
    private SerializedProperty currentInfluence;
    private SerializedProperty currentMaxInfluence;
    private SerializedProperty imageComp;
    private SerializedProperty containerComp;
    private SerializedProperty consequencesImages;
    private SerializedProperty influenceData;

    private void OnEnable()
    {
        currentInfluence = serializedObject.FindProperty("currentInfluence");
        currentMaxInfluence = serializedObject.FindProperty("currentMaxInfluence");
        imageComp = serializedObject.FindProperty("imageComp");
        consequencesImages = serializedObject.FindProperty("consequencesImages");
        influenceData = serializedObject.FindProperty("influence");
        containerComp = serializedObject.FindProperty("boxContainer");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("Influence Settings", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(influenceData);
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("UI Components", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(imageComp);
        EditorGUILayout.PropertyField(containerComp);
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Consequences Images", EditorStyles.boldLabel);

        if (consequencesImages != null)
        {
            for (int i = 0; i < consequencesImages.arraySize; i++)
            {
                SerializedProperty consequence = consequencesImages.GetArrayElementAtIndex(i);
                SerializedProperty imageProp = consequence.FindPropertyRelative("Image");
                SerializedProperty keyProp = consequence.FindPropertyRelative("Key");

                EditorGUILayout.BeginVertical(GUI.skin.box);
                EditorGUILayout.PropertyField(imageProp, new GUIContent("Image"));
                EditorGUILayout.PropertyField(keyProp, new GUIContent("Key"));

                if (GUILayout.Button("Remove Consequence"))
                {
                    consequencesImages.DeleteArrayElementAtIndex(i);
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();
            }
        }
        
        if (GUILayout.Button("Add Consequence"))
        {
            consequencesImages.arraySize++;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif