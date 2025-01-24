using LittleGraph.Runtime;
using LittleGraph.Runtime.Attributes;
using UnityEngine;

namespace LittleDialogue.Runtime.LittleGraphAddOn
{
#if LITTLE_GRAPH
    public abstract class LDDialogueNode : LGNode
    {
        [ExposedProperty()]
        public string DialogueText;
    }
#endif
}
