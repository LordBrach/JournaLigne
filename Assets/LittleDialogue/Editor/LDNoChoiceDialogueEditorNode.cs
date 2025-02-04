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
    [LGCustomEditorNode(typeof(LDNoChoiceDialogueNode))]
    public class LDNoChoiceDialogueEditorNode : LGEditorNode
    {
        public LDNoChoiceDialogueEditorNode() : base()
        {
            
        }
        
        public LDNoChoiceDialogueEditorNode(LGNode node, SerializedObject graphObject) : base(node, graphObject)
        {
        }
        
        
    }
#endif
}
