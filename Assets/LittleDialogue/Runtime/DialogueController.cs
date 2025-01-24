using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LittleDialogue.Runtime.LittleGraphAddOn;
using LittleGraph.Runtime;
using LittleGraph.Runtime.Attributes;
using UnityEditor;
using UnityEngine;

namespace LittleDialogue.Runtime
{
    public class DialogueController : MonoBehaviour
    {
        private List<LDDialogueNode> m_dialogueNodes;

        [SerializeField] private DialogueBox m_dialogueBox; 
        
        public void Init(List<LGGraphObject> graphObjects)
        {
            //Subscribe to all dialogue nodes
            foreach (LGGraphObject graphObject in graphObjects)
            {
                m_dialogueNodes = graphObject.GraphInstance.Nodes.OfType<LDDialogueNode>().ToList();
                foreach (LDDialogueNode dialogueNode in m_dialogueNodes)
                {
                    dialogueNode.Executed += OnDialogueNodeExecuted;
                }
            }
        }
        private void OnDialogueNodeExecuted(LGNode node)
        {
            if(!m_dialogueBox) return;

            
            m_dialogueBox.ShowBox();
            m_dialogueBox.ClearChoiceButtons();
            if (node is LDDialogueNode dialogueNode)
            {
                m_dialogueBox.UpdateText(dialogueNode.DialogueText);

                if (dialogueNode is LDSingleChoiceDialogueNode singleChoiceDialogueNode)
                {
                    if (node.NodeConnections.Exists(connection => connection.OutputPort.NodeId == dialogueNode.ID))
                    {
                        LGConnection connection =
                            node.NodeConnections.Find(connection => connection.OutputPort.NodeId == dialogueNode.ID);
                    
                        m_dialogueBox.AddChoiceButton(singleChoiceDialogueNode.ChoiceText, () =>
                        {
                            node.EmitFlow(connection.InputPort.NodeId);
                        });
                    }
                    else
                    {
                        m_dialogueBox.AddChoiceButton(singleChoiceDialogueNode.ChoiceText);
                    }
                    
                }

                if (dialogueNode is LDMultipleChoiceDialogueNode multipleChoiceDialogueNode)
                {
                    if (node.NodeConnections.Exists(connection => connection.OutputPort.NodeId == dialogueNode.ID))
                    {
                        int i = 0;
                        foreach (LGConnection connection in node.NodeConnections.FindAll(connection => connection.OutputPort.NodeId == dialogueNode.ID))
                        {
                            m_dialogueBox.AddChoiceButton(multipleChoiceDialogueNode.ChoiceDatas.Find(x => x.OutputIndex == connection.OutputPort.PortIndex).ChoiceText, () =>
                            {
                                node.EmitFlow(connection.InputPort.NodeId);
                            });
                            i++;
                        }
                    }
                    else
                    {
                        m_dialogueBox.AddChoiceButton(multipleChoiceDialogueNode.ChoiceDatas[0].ChoiceText);
                    }
                }
                
            }
            // if (node is LDSingleChoiceDialogueNode singleChoiceNode)
            // {
            //     
            //     
            // }
            // else if(node is LDMultipleChoiceDialogueNode multipleChoiceNode)
            // {
            //     m_dialogueBox.UpdateText(multipleChoiceNode.DialogueText);
            // }
        }
    }
}
