using System.Collections.Generic;
using LittleDialogue.Runtime.LittleGraphAddOn;
using LittleGraph.Editor;
using LittleGraph.Editor.Attributes;
using LittleGraph.Runtime;
using LittleGraph.Runtime.Attributes;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace LittleDialogue.Editor
{
#if LITTLE_GRAPH
    [LGCustomEditorNode(typeof(LDMultipleChoiceDialogueNode))]
    public class LDMultipleChoiceDialogueEditorNode : LGEditorNode 
    {
        public LDMultipleChoiceDialogueEditorNode() : base()
        {
            
        }

        public LDMultipleChoiceDialogueEditorNode(LGNode node, SerializedObject graphObject) : base(node, graphObject)
        {
            
        }

        protected override void CreateFlowOutputPort(LGNodeInfoAttribute nodeInfo)
        {
            LDMultipleChoiceDialogueNode dialogueNode = (LDMultipleChoiceDialogueNode)m_node;
            
            Port outputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single,
                typeof(PortTypes.FlowPort));
            
            //Add to editor node
            m_outputPorts.Add(outputPort);
            m_ports.Add(outputPort);
            outputContainer.Add(outputPort);
            
            //Set up complementary data
            if (nodeInfo.OutputComplementaryDataType == typeof(string))
            {

                TextField textField = null;

                if (dialogueNode.ChoiceDatas.Exists(x => x.OutputIndex == m_ports.IndexOf(outputPort)))
                {
                    textField = new TextField()
                    {
                        value = dialogueNode.ChoiceDatas.Find(x => x.OutputIndex == m_ports.IndexOf(outputPort)).ChoiceText
                    };
                }
                else
                {
                    textField = new TextField()
                    {
                        value = "Out"
                    };
                    dialogueNode.ChoiceDatas.Add(new ChoiceData(m_ports.IndexOf(outputPort), textField.value));
                }
                
                // Synchroniser la valeur du champ avec le DialogueText du noeud
                textField.RegisterValueChangedCallback(evt =>
                {
                    ChoiceData tmpData = new ChoiceData(m_ports.IndexOf(outputPort), evt.newValue);
                    dialogueNode.ChoiceDatas[dialogueNode.ChoiceDatas.IndexOf(dialogueNode.ChoiceDatas.Find(x=>x.OutputIndex == m_ports.IndexOf(outputPort)))] = tmpData;
                    m_serializedObject.Update();
                    EditorUtility.SetDirty(m_serializedObject.targetObject);
                });
                
                outputPort.Add(textField);
                
                
                
                outputPort.portName = "";
                outputPort.tooltip = "Flow output";
            }
        }

        protected override void OnRemoveOutput(Port outputPort)
        {
            LDMultipleChoiceDialogueNode dialogueNode = (LDMultipleChoiceDialogueNode)m_node;
            dialogueNode.ChoiceDatas.Remove(dialogueNode.ChoiceDatas.Find(x=>x.OutputIndex == m_ports.IndexOf(outputPort)));
            base.OnRemoveOutput(outputPort);
        }
    }
#endif
}
