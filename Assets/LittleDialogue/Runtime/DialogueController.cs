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
#if LITTLE_GRAPH
        private List<LDDialogueNode> m_dialogueNodes;
        private LDDialogueNode m_currentNode;
#endif
        [SerializeField] private DialogueBox m_dialogueBox; 
       
#if LITTLE_GRAPH
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
            
            //Subscribe to Dialogue Box Events
            m_dialogueBox.OnTextUpdateEnded += OnTextUpdateEnd;
        }
        private void OnDialogueNodeExecuted(LGNode node)
        {
            if(!m_dialogueBox) return;

            if (node is LDDialogueNode dialogueNode)
            {
                m_currentNode = dialogueNode;
                if(m_currentNode == null)return;
                
                m_dialogueBox.ShowBox();
                m_dialogueBox.ClearChoiceButtons();
                
                m_dialogueBox.UpdateText(m_currentNode.DialogueText);
                m_dialogueBox.UpdateInterlocutorImage(m_currentNode.InterlocutorSprite);
                m_dialogueBox.UpdateBackgroundImage(m_currentNode.BackgroundSprite);
            }
        }

        private void OnTextUpdateEnd()
        {
            // if (m_currentNode is LDSingleChoiceDialogueNode singleChoiceDialogueNode)
            // {
            //     if (m_currentNode.NodeConnections.Exists(connection => connection.OutputPort.NodeId == m_currentNode.ID))
            //     {
            //         LGConnection connection =
            //             m_currentNode.NodeConnections.Find(connection => connection.OutputPort.NodeId == m_currentNode.ID);
            //         
            //         m_dialogueBox.AddChoiceButton(singleChoiceDialogueNode.ChoiceText, () =>
            //         {
            //             m_currentNode.EmitFlow(connection.InputPort.NodeId);
            //         });
            //     }
            //     else
            //     {
            //         m_dialogueBox.AddChoiceButton(singleChoiceDialogueNode.ChoiceText);
            //     }
            //         
            // }


            if (m_currentNode is LDNoChoiceDialogueNode noChoiceDialogueNode)
            {
                if (m_currentNode.NodeConnections.Exists(connection =>
                        connection.OutputPort.NodeId == m_currentNode.ID))
                {
                    LGConnection connection = m_currentNode.NodeConnections.Find(connection => connection.OutputPort.NodeId == m_currentNode.ID);
                    
                    EmitFlowToNextNode(connection.InputPort.NodeId);
                }

                //Start a timer before emit
                
                return;
            }
            
            if (m_currentNode is LDMultipleChoiceDialogueNode multipleChoiceDialogueNode)
            {
                if (m_currentNode.NodeConnections.Exists(connection => connection.OutputPort.NodeId == m_currentNode.ID))
                {
                    int i = 0;
                    foreach (LGConnection connection in m_currentNode.NodeConnections.FindAll(connection => connection.OutputPort.NodeId == m_currentNode.ID))
                    {
                        m_dialogueBox.AddChoiceButton(multipleChoiceDialogueNode.ChoiceDatas.Find(x => x.OutputIndex == connection.OutputPort.PortIndex).ChoiceText, () =>
                        {
                            EmitFlowToNextNode(connection.InputPort.NodeId);
                            // m_currentNode.EmitFlow(connection.InputPort.NodeId);
                        });
                        i++;
                    }
                }
                else
                {
                    m_dialogueBox.AddChoiceButton(multipleChoiceDialogueNode.ChoiceDatas[0].ChoiceText);
                }
                
                return;
            }
        }

        private void EmitFlowToNextNode(string nextNodeId)
        {
            m_currentNode.EmitFlow(nextNodeId);
        }
#endif
    }
}
