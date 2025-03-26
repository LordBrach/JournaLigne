using LittleGraph.Runtime;
using LittleGraph.Runtime.Attributes;
using UnityEngine;

namespace _Project.Scripts.Runtime.DaysSystem.LittleGraphAddOn
{
#if LITTLE_GRAPH
    [LGNodeInfo("Next Day", "Days System/Next Day")]
    public class NextDayNode : LGNode
    {
        protected override void ExecuteNode()
        {
            Debug.Log("Execute NextDayNode");
            if (DayManager.instance != null)
            {
                Debug.Log("Found Day Manager");
                DayManager.instance.NextDay();
            }
            
            if (m_nodeConnections.Exists(connection => connection.OutputPort.NodeId == ID))
            {
                LGConnection connection = m_nodeConnections.Find(connection => connection.OutputPort.NodeId == ID);
                EmitFlow(connection.InputPort.NodeId);
            }
            
            base.ExecuteNode();
        }
    }
#endif
}
