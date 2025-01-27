using System;
using System.Collections.Generic;
using System.Reflection;
using LittleGraph.Runtime.Attributes;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace LittleGraph.Runtime
{
    [System.Serializable]
    public abstract class LGNode
    {
        [SerializeField] private string m_guid;
        [SerializeField] private Rect m_position;

        [SerializeField] private int m_outputPortAmount;
        [SerializeField/*, HideInInspector*/] protected List<LGConnection> m_nodeConnections;
        
        public string TypeName;
        public string ID => m_guid;
        public Rect Position => m_position;

        public int OutputPortAmount
        {
            get => m_outputPortAmount;
            set => m_outputPortAmount = value;
        }
        
        public List<LGConnection> NodeConnections => m_nodeConnections;

        public Action ReceivedFlow;
        public Action<LGNode> Executed;
        public Action<string> EmittedFlow;
        
        public LGNode()
        {
            NewGUID();
            m_nodeConnections = new List<LGConnection>();
            m_outputPortAmount = 1;
        }

        private void NewGUID()
        {
            m_guid = Guid.NewGuid().ToString();
        }

        public void SetPosition(Rect position)
        {
            m_position = position;
        }
        public virtual void ReceiveFlow()
        {
            ExecuteNode();
            ReceivedFlow?.Invoke();
        }

        protected virtual void ExecuteNode()
        {
            Executed?.Invoke(this);
        }
        
        public virtual void EmitFlow(string nextNodeId)
        {
            if(string.IsNullOrEmpty(nextNodeId)) return;
            EmittedFlow?.Invoke(nextNodeId);
        }
        
        public virtual string OnProcess(LGGraph currentGraph)
        {
            if(m_nodeConnections.Count <= 0) return string.Empty;
            
            LGNodeInfoAttribute nodeInfoAttribute = TypeName.GetType().GetCustomAttribute<LGNodeInfoAttribute>();
            if (nodeInfoAttribute.HasMultipleOutputs)
            {
                return string.Empty;
            }
            else
            {
                LGConnection connection = m_nodeConnections.Find(x => x.OutputPort.NodeId == ID);
                LGNode nextNodeInFlow = currentGraph.GetNodeFromOutput(m_guid, connection.OutputPort.PortIndex);
                if (nextNodeInFlow != null)
                {
                    return nextNodeInFlow.ID;
                }
            }
            // LDNode nextNodeInFlow = currentGraph.GetNodeFromOutput(m_guid, 0);
            // if (nextNodeInFlow != null)
            // {
            //     return nextNodeInFlow.ID;
            // }
            return string.Empty;
        }
    }
}
