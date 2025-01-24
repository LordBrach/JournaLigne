using LittleGraph.Runtime;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace LittleGraph.Editor
{
    public class LGEditorWindow : EditorWindow
    {
        private LGGraph m_currentGraph;
        private SerializedObject m_serializedObject;
        private LGGraphView m_currentView;

        public LGGraph CurrentGraph => m_currentGraph;
        
        
        // [MenuItem("Tools/LD Graph")]
        // public static void Open()
        // {
        //     LDEditorWindow window = GetWindow<LDEditorWindow>();
        //     window.titleContent = new GUIContent("Little Dialogue Graph");
        // }
        
        public static void Open(LGGraph target)
        {
            LGEditorWindow[] windows = Resources.FindObjectsOfTypeAll<LGEditorWindow>();
            foreach (LGEditorWindow window in windows)
            {
                if (window.CurrentGraph == target)
                {
                    window.Focus();
                    return;
                }
            }

            LGEditorWindow newWindow = CreateWindow<LGEditorWindow>(typeof(LGEditorWindow), typeof(SceneView));
            newWindow.titleContent = new GUIContent($"{target.name}");
            newWindow.Load(target);
        }

        private void OnEnable()
        {
            if (m_currentGraph != null)
            {
                DrawGraph();
            }
        }

        private void OnGUI()
        {
            if (m_currentGraph != null)
            {
                if (EditorUtility.IsDirty(m_currentGraph))
                {
                    this.hasUnsavedChanges = true;
                }
                else
                {
                    this.hasUnsavedChanges = false;
                }
            }
        }

        private void Load(LGGraph target)
        {
            m_currentGraph = target;
            DrawGraph();
        }

        private void DrawGraph()
        {
            m_serializedObject = new SerializedObject(m_currentGraph);
            m_currentView = new LGGraphView(m_serializedObject, this);
            m_currentView.graphViewChanged += OnChange;
            rootVisualElement.Add(m_currentView);
        }

        private GraphViewChange OnChange(GraphViewChange graphviewchange)
        {
            EditorUtility.SetDirty(m_currentGraph);
            return graphviewchange;
        }

    }
}
