using LittleDialogue.Runtime.Localization;
using UnityEditor;
using UnityEngine;

namespace LittleDialogue.Editor.Localization
{
    [CustomEditor(typeof(LocalizationDatabase))]
    public class LocalizationDatabaseEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            if(GUILayout.Button("Load localization data"))
            {
                
            }
        }
    }
}
