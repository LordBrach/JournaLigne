using System;
using LittleGraph.Runtime;
using LittleGraph.Runtime.Attributes;
using UnityEngine;

namespace LittleDialogue.Runtime.LittleGraphAddOn
{
#if LITTLE_GRAPH
    [LGNodeInfo("Single Choice Dialogue", "Little Dialogue/Single Choice Dialogue", true, true, false, typeof (string))]
    public class LDSingleChoiceDialogueNode : LDDialogueNode
    {
        protected override void ExecuteNode()
        {
            
            base.ExecuteNode();
        }
        
    }
    

    public struct CustomOutputData
    {
        public Type Type;

        public CustomOutputData(Type type)
        {
            Type = type;
        }
    }
#endif
}
