using System;
using LittleGraph.Runtime;
using UnityEditor;
using UnityEngine;

namespace LittleGraph.Editor
{
    [CustomEditor(typeof(LGGraphObject))]
    public class LGGraphObjectEditor : UnityEditor.Editor
    {
        private LGGraphObject m_targetGraphObject;
        
        private void OnEnable()
        {
            m_targetGraphObject = (LGGraphObject)serializedObject.targetObject;
            
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
    
            if (Application.isPlaying && GUILayout.Button("Execute Graph"))
            {
                m_targetGraphObject.ExecuteAsset();
            }
        }
    }
}
