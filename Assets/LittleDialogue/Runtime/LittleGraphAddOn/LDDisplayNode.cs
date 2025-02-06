using LittleGraph.Runtime;
using LittleGraph.Runtime.Attributes;
using UnityEngine;

namespace LittleDialogue.Runtime.LittleGraphAddOn
{
#if LITTLE_GRAPH
    [LGNodeInfo("Dialogue Display", "Little Dialogue/Dialogue Display")]
    public class LDDisplayNode : LGNode
    {
        [ExposedProperty] public LDDialogueDisplayType InterlocutorDisplayType;
        [ExposedProperty] public LDDialogueDisplayType BackgroundDisplayType;
        [ExposedProperty] public LDDialogueDisplayType ForegroundDisplayType;

        protected override void ExecuteNode()
        {
            base.ExecuteNode();

            if (NodeConnections.Exists(x => x.OutputPort.NodeId == ID))
            {
                LGConnection connection = NodeConnections.Find(x => x.OutputPort.NodeId == ID);
                EmitFlow(connection.InputPort.NodeId);
            }
        }
    }
#endif
}
