using LittleGraph.Runtime;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace LittleGraph.Editor
{
    [CustomEditor(typeof(LGGraph))]
    public class LGGraphEditor : UnityEditor.Editor
    {
        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceId, int index)
        {
            Object asset = EditorUtility.InstanceIDToObject(instanceId);
            if (asset.GetType() == typeof(LGGraph))
            {
                LGEditorWindow.Open((LGGraph)asset);
                return true;
            }

            return false;
        }
        
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Open"))
            {
                LGEditorWindow.Open((LGGraph)target);
            }
        }
    }
}
