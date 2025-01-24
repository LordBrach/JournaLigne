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
#endif
        [SerializeField] private DialogueBox m_dialogueBox; 
        
        public void Init(List<LGGraphObject> graphObjects)
        {
#if LITTLE_GRAPH
            //Subscribe to all dialogue nodes
            foreach (LGGraphObject graphObject in graphObjects)
            {
                m_dialogueNodes = graphObject.GraphInstance.Nodes.OfType<LDDialogueNode>().ToList();
                foreach (LDDialogueNode dialogueNode in m_dialogueNodes)
                {
                    dialogueNode.Executed += OnDialogueNodeExecuted;
                }
            }
#endif
        }
        
#if LITTLE_GRAPH

        private void OnDialogueNodeExecuted(LGNode node)
        {
            if(!m_dialogueBox) return;

            m_dialogueBox.ShowBox();
            if (node is LDDialogueNode dialogueNode)
            {
                m_dialogueBox.UpdateText(dialogueNode.DialogueText);
                m_dialogueBox.UpdateButtonCallback(0, () =>
                {
                    node.EmitFlow(node.NodeConnections.Find(connection => connection.OutputPort.NodeId == dialogueNode.ID).InputPort.NodeId);
                });
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
#endif
    }
}
