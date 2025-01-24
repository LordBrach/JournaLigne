using LittleGraph.Runtime;
using LittleGraph.Runtime.Attributes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LittleDialogue.Runtime.LittleGraphAddOn
{
#if LITTLE_GRAPH
    [LGNodeInfo("Multiple Choice Dialogue", "Little Dialogue/Multiple Choice Dialogue", true, true, true)]
    public class LDMultipleChoiceDialogueNode : LDDialogueNode
    {
        protected override void ExecuteNode()
        {
            base.ExecuteNode();
        }
    }
#endif
    
}
