using LittleGraph.Runtime.Attributes;
using UnityEngine;

namespace LittleGraph.Runtime.Types
{
    [LGNodeInfo("Start", "Process/Start", false, true)]
    public class LGStartNode : LGNode
    {
        protected override void ExecuteNode()
        {
            Debug.Log("Execute Start Node");

            Debug.Log( m_nodeConnections.Count);
            LGConnection connection = m_nodeConnections.Find(x => x.OutputPort.NodeId == ID);
            EmitFlow(connection.InputPort.NodeId);
            base.ExecuteNode();
        }

        public override string OnProcess(LGGraph currentGraph)
        {
            Debug.Log("Start Node");

            return base.OnProcess(currentGraph);
        }
    }
}
