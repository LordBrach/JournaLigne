using System;
using System.Collections.Generic;
using System.Reflection;
using LittleGraph.Editor.Utilities;
using LittleGraph.Runtime;
using LittleGraph.Runtime.Attributes;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;

namespace LittleGraph.Editor
{
    public class LGEditorNode : Node
    {
        private LGNode m_node;
        private LGNodeInfoAttribute m_nodeInfos;
        private List<Port> m_outputPorts;
        private List<Port> m_ports;

        private SerializedObject m_serializedObject;
        private SerializedProperty m_serializedProperty;
        
        public LGNode Node => m_node;
        public List<Port> OutputPorts
        {
            get => m_outputPorts;
            set => m_outputPorts = value;
        }
        public List<Port> Ports => m_ports;

        public event Action<LGEditorNode> OutputRemovedAction; 
        
        public LGEditorNode(LGNode node, SerializedObject graphObject)
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
                CreateFlowOutputPort(m_nodeInfos);
            }

            foreach (FieldInfo property in typeInfo.GetFields())
            {
                if (property.GetCustomAttribute<ExposedPropertyAttribute>() is ExposedPropertyAttribute exposedPropertyAttribute)
                {
                    PropertyField field = DrawProperty(property.Name);
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
                titleButtonContainer.Add(new Button().CreateButton("d_Toolbar Minus",OnRemoveOutput));
            }
        }

        private void OnAddOutput()
        {
            CreateFlowOutputPort(m_nodeInfos);
        }

        private void OnRemoveOutput()
        {
            OutputRemovedAction?.Invoke(this);
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

        private void CreateFlowOutputPort(LGNodeInfoAttribute nodeInfo)
        {
            Port outputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single,
                typeof(PortTypes.FlowPort));
            outputPort.portName = "Out";
            outputPort.tooltip = "Flow output";
            
            if (nodeInfo.OutputComplementaryDataType == typeof(string))
            {
                TextField textField = new TextField()
                {
                    value = "Out"
                };
                // Synchroniser la valeur du champ avec le DialogueText du noeud
                textField.RegisterValueChangedCallback(evt =>
                {
                    outputPort.portName = evt.newValue; // Mettre à jour le DialogueText
                });
                outputContainer.Add(textField);
            }
            
            //Add to editor node
            m_outputPorts.Add(outputPort);
            m_ports.Add(outputPort);
            outputContainer.Add(outputPort);
        }

        private PropertyField DrawProperty(string propertyName)
        {
            if (m_serializedProperty == null)
            {
                FetchSerializedProperty();
            }

            SerializedProperty property = m_serializedProperty.FindPropertyRelative(propertyName);
            
            PropertyField field = new PropertyField(property);
            field.bindingPath = property.propertyPath;
            extensionContainer.Add(field);
            return field;
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
