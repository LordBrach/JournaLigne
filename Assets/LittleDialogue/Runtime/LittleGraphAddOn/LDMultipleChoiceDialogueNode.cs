using System.Collections.Generic;
using LittleGraph.Runtime;
using LittleGraph.Runtime.Attributes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LittleDialogue.Runtime.LittleGraphAddOn
{
#if LITTLE_GRAPH
    [LGNodeInfo("Multiple Choice Dialogue", "Little Dialogue/Multiple Choice Dialogue", true, true, true, typeof(string))]
    public class LDMultipleChoiceDialogueNode : LDDialogueNode
    {
        public List<ChoiceData> ChoiceDatas;

        public LDMultipleChoiceDialogueNode()
        {
            ChoiceDatas = new List<ChoiceData>();
        }
        protected override void ExecuteNode()
        {
            base.ExecuteNode();
        }
    }

    [System.Serializable]
    public struct ChoiceData
    {
        public int OutputIndex;
        public string ChoiceText;

        public ChoiceData(int outputIndex, string choiceText)
        {
            OutputIndex = outputIndex;
            ChoiceText = choiceText;
        }
    }
#endif
    
}
