using LittleGraph.Runtime.Attributes;
using UnityEngine;

namespace LittleGraph.Runtime.Types
{
    [LGNodeInfo("Debug Log", "Debug/Debug Log")]
    public class LGDebugLogNode : LGNode
    {
        [ExposedProperty()]
        public string LogMessage;

        protected override void ExecuteNode()
        {
            Debug.Log(LogMessage);
            
            LGConnection connection = NodeConnections.Find(x => x.OutputPort.NodeId == ID);
            EmitFlow(connection.InputPort.NodeId);
            
            base.ExecuteNode();
        }

        public override string OnProcess(LGGraph currentGraph)
        {
            Debug.Log(LogMessage);
            
            return base.OnProcess(currentGraph);
        }
    }
}
