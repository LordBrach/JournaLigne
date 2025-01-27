using LittleDialogue.Runtime.LittleGraphAddOn;
using LittleGraph.Editor;
using LittleGraph.Editor.Attributes;
using LittleGraph.Runtime;
using LittleGraph.Runtime.Attributes;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace LittleDialogue.Editor
{
#if LITTLE_GRAPH
    [LGCustomEditorNode(typeof(LDSingleChoiceDialogueNode))]
    public class LDSingleChoiceDialogueEditorNode : LGEditorNode
    {
        public LDSingleChoiceDialogueEditorNode() : base()
        {
            
        }
        
        public LDSingleChoiceDialogueEditorNode(LGNode node, SerializedObject graphObject) : base(node, graphObject)
        {
        }

        protected override void CreateFlowOutputPort(LGNodeInfoAttribute nodeInfo)
        {
            LDSingleChoiceDialogueNode dialogueNode = (LDSingleChoiceDialogueNode)m_node;
            
            Port outputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single,
                typeof(PortTypes.FlowPort));
            //Set up complementary data
            if (nodeInfo.OutputComplementaryDataType == typeof(string))
            {

                TextField textField = null;

                if (string.IsNullOrEmpty(dialogueNode.ChoiceText))
                {
                    textField= new TextField()
                    {
                        value = "Out"
                    };
                    dialogueNode.ChoiceText = textField.value;
                }
                else
                {
                    textField = new TextField()
                    {
                        value = dialogueNode.ChoiceText
                    };
                }
                
                // Synchroniser la valeur du champ avec le DialogueText du noeud
                textField.RegisterValueChangedCallback(evt =>
                {
                    dialogueNode.ChoiceText = evt.newValue;
                    m_serializedObject.Update();
                    EditorUtility.SetDirty(m_serializedObject.targetObject);
                });
                
                outputPort.Add(textField);
                outputPort.portName = "";
                outputPort.tooltip = "Flow output";
            }
            
            //Add to editor node
            m_outputPorts.Add(outputPort);
            m_ports.Add(outputPort);
            outputContainer.Add(outputPort);
        }
    }
#endif
}
