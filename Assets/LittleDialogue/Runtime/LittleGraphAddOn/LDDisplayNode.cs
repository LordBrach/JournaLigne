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

            if (m_nodeConnections.Exists(connection => connection.OutputPort.NodeId == ID))
            {
                LGConnection connection = m_nodeConnections.Find(connection => connection.OutputPort.NodeId == ID);
                EmitFlow(connection.InputPort.NodeId);
            }
        }
    }
#endif
}
