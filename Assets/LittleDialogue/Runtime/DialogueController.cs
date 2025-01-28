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
        // FIELDS //
        
#if LITTLE_GRAPH
        //Dialogue Nodes
        private List<LDDialogueNode> m_dialogueNodes;
        private LDDialogueNode m_currentNode;
#endif
        //Dialogue Box UI
        [SerializeField] private DialogueBox m_dialogueBox; 
       
        // COROUTINES & TIMERS //
        // Timer before next node
        private bool m_isTimerBeforeNextNodeOn = false;
        private float m_timerBeforeNextNode;
        
        [Header("Timers")]
        [SerializeField]
        [Tooltip("After the text finished to be written, this is the time (s) to wait before showing the next text.")] 
        private float m_timeBeforeNextNode;

        
        // METHODS //
        
        #region Monobehaviour Methods

        private void Update()
        {
            if (m_isTimerBeforeNextNodeOn)
            {
                m_timerBeforeNextNode -= Time.deltaTime;
                if (m_timerBeforeNextNode <= 0)
                {
                    EndTimerBeforeNextNode();
                }
            }
        }

        #endregion
        
        
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
            m_dialogueBox.OnIdleTextBoxTouched += OnIdleTextBoxTouched;
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
            if (m_currentNode is LDNoChoiceDialogueNode noChoiceDialogueNode)
            {
                StartTimerBeforeNextNode();
                return;
            }
            
            EmitFlowToNextNode();
        }
        
        private void OnIdleTextBoxTouched()
        {
            if (m_currentNode is LDNoChoiceDialogueNode)
            {
                ClearTimerBeforeNextNode();
                EmitFlowToNextNode();
            }
        }

        private void StartTimerBeforeNextNode()
        {
            m_timerBeforeNextNode = m_timeBeforeNextNode;
            m_isTimerBeforeNextNodeOn = true;
        }

        private void EndTimerBeforeNextNode()
        {
            ClearTimerBeforeNextNode();

            EmitFlowToNextNode();
        }
        
        private void ClearTimerBeforeNextNode()
        {
            m_isTimerBeforeNextNodeOn = false;
        }
        
        private void EmitFlowToNextNode()
        {
            if (m_currentNode is LDNoChoiceDialogueNode noChoiceDialogueNode)
            {
                if (m_currentNode.NodeConnections.Exists(connection => connection.OutputPort.NodeId == m_currentNode.ID))
                {
                    LGConnection connection =
                        m_currentNode.NodeConnections.Find(connection => connection.OutputPort.NodeId == m_currentNode.ID);
                    
                    m_currentNode.EmitFlow(connection.InputPort.NodeId);
                }
                
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
                            m_currentNode.EmitFlow(connection.InputPort.NodeId);
                            // m_currentNode.EmitFlow(connection.InputPort.NodeId);
                        });
                        i++;
                    }
                }
                
                return;
            }
        }

#endif
    }
}
