using System;
using System.Collections.Generic;
using System.Reflection;
using LittleGraph.Editor.Utilities;
using LittleGraph.Runtime;
using LittleGraph.Runtime.Attributes;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;

namespace LittleGraph.Editor
{
    public class LGEditorNode : Node
    {
        protected LGNode m_node;
        protected LGNodeInfoAttribute m_nodeInfos;
        protected List<Port> m_outputPorts;
        protected List<Port> m_ports;

        protected SerializedObject m_serializedObject;
        protected SerializedProperty m_serializedProperty;
        
        public LGNode Node => m_node;
        public List<Port> OutputPorts
        {
            get => m_outputPorts;
            set => m_outputPorts = value;
        }
        public List<Port> Ports => m_ports;

        public event Action<Port> OutputRemovedAction;

        public LGEditorNode()
        {
            
        }
        public LGEditorNode(LGNode node, SerializedObject graphObject)
        {
            InitEditorNode(node, graphObject);
        }

        public void InitEditorNode(LGNode node, SerializedObject graphObject)
        {
            this.AddToClassList("ld-node");

            m_serializedObject = graphObject;
            LGGraph graph = (LGGraph)graphObject.targetObject;
            
            m_node = node;
            Type typeInfo = node.GetType();
            m_nodeInfos = typeInfo.GetCustomAttribute<LGNodeInfoAttribute>();

            title = m_nodeInfos.Title;

            m_outputPorts = new List<Port>();
            m_ports = new List<Port>();
            
            string[] depths = m_nodeInfos.MenuItem.Split('/');
            foreach (string depth in depths)
            {
                this.AddToClassList(depth.ToLower().Replace(' ', '-'));
            }
            
            this.name = typeInfo.Name;

            DrawTitleButtons(m_nodeInfos, graph);
            
            if (m_nodeInfos.HasFlowInput)
            {
                CreateFlowInputPort();
            }
            
            //So output is always index 0 (to be changed later)
            if (m_nodeInfos.HasFlowOutput)
            {
                // info.OutputComplementaryDataType.GetFields()
                for (int i = 0; i < m_node.OutputPortAmount; i++)
                {
                    CreateFlowOutputPort(m_nodeInfos);
                }
            }

            foreach (FieldInfo property in typeInfo.GetFields())
            {
                if (property.GetCustomAttribute<ExposedPropertyAttribute>() is ExposedPropertyAttribute exposedPropertyAttribute)
                {
                    Debug.Log(property.Name);
                    PropertyField field = DrawProperty(property.Name, exposedPropertyAttribute);
                    //field.RegisterValueChangeCallback(OnFieldChangeCallback);
                }
            }
            
            
            
            RefreshExpandedState();
        }

        private void DrawTitleButtons(LGNodeInfoAttribute info, LGGraph graph)
        {
            if (info.HasMultipleOutputs)
            {
                titleButtonContainer.Add(new Button().CreateButton("d_Toolbar Plus",OnAddOutput));
                titleButtonContainer.Add(new Button().CreateButton("d_Toolbar Minus", () =>
                {
                    OnRemoveOutput(m_outputPorts[^1]);
                }));
            }
        }

        private void OnAddOutput()
        {
            m_node.OutputPortAmount += 1;
            CreateFlowOutputPort(m_nodeInfos);
        }

        protected virtual void OnRemoveOutput(Port outputPort)
        {
            //Create function for removing output
            if (OutputPorts.Count <= 1) return;

            Node.OutputPortAmount -= 1;
            
            OutputPorts.Remove(outputPort);
            Ports.Remove(outputPort);
            
            OutputRemovedAction?.Invoke(outputPort);
            outputContainer.Remove(outputPort);
        }
        
        private void CreateFlowInputPort()
        {
            Port inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single,
                typeof(PortTypes.FlowPort));
            inputPort.portName = "In";
            inputPort.tooltip = "Flow input";
            m_ports.Add(inputPort);
            inputContainer.Add(inputPort); 
        }

        protected virtual void CreateFlowOutputPort(LGNodeInfoAttribute nodeInfo)
        {
            
            Port outputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single,
                typeof(PortTypes.FlowPort));
            
            //Add to editor node
            m_outputPorts.Add(outputPort);
            m_ports.Add(outputPort);
            outputContainer.Add(outputPort);
            
            outputPort.portName = "Out";
            outputPort.tooltip = "Flow output";
        }

        private PropertyField DrawProperty(string propertyName, ExposedPropertyAttribute expositionInfo)
        {
            if (m_serializedProperty == null)
            {
                FetchSerializedProperty();
            }

            SerializedProperty property = m_serializedProperty.FindPropertyRelative(propertyName);

            switch (expositionInfo.ExposedPropertyType)
            {
                case (ExposedPropertyType.Simple):
                    PropertyField field = new PropertyField(property); 
                    field.bindingPath = property.propertyPath;

                    field.SetEnabled(expositionInfo.EditableInGraph);
        
                    extensionContainer.Add(field);
                    return field;
                
                case ExposedPropertyType.None:
                    Debug.Log("No Property Type");
                    return null;
                default:
                    return null;
            }
        }

        private void FetchSerializedProperty()
        {
            //Get the nodes
            SerializedProperty nodes = m_serializedObject.FindProperty("m_nodes");
            if (nodes.isArray)
            {
                int size = nodes.arraySize;
                for (int i = 0; i < size; i++)
                {
                    SerializedProperty element = nodes.GetArrayElementAtIndex(i);
                    SerializedProperty elementId = element.FindPropertyRelative("m_guid");
                    if (elementId.stringValue == m_node.ID)
                    {
                        m_serializedProperty = element;
                    }
                }
            }
        }
        
        public void SavePosition()
        {
            m_node.SetPosition(GetPosition());
        }
    }
}
