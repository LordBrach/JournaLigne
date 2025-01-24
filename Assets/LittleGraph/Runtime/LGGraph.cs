using System.Collections.Generic;
using System.Linq;
using LittleGraph.Runtime.Types;
using UnityEngine;

namespace LittleGraph.Runtime
{
    [CreateAssetMenu(fileName = "LGGraph", menuName = "Scriptable Objects/Little Graph/LGGraph")]
    public class LGGraph : ScriptableObject
    {
        //We serialize so that an instance can receive the data
        [SerializeReference]
        private List<LGNode> m_nodes;

        private Dictionary<string, LGNode> m_nodeDictionary;
        
        //We serialize so that an instance can receive the data
        [SerializeField] private List<LGConnection> m_connections;
        
        public List<LGNode> Nodes => m_nodes;
        public List<LGConnection> Connections => m_connections;
        
        public LGGraph()
        {
            m_nodes = new List<LGNode>();
            m_connections = new List<LGConnection>();
        }

        public void Init()
        {
            m_nodeDictionary = new Dictionary<string, LGNode>();
            foreach (LGNode node in m_nodes)
            {
                m_nodeDictionary.Add(node.ID, node);
            }
        }
        
        public LGNode GetStartNode()
        {
            LGStartNode[] startNodes = Nodes.OfType<LGStartNode>().ToArray();

            if (startNodes.Length == 0)
            {
                Debug.LogError("There is no start node in this graph");
                return null;
            }

            return startNodes[0];
        }

        public LGNode GetNode(string nodeId)
        {
            if (m_nodeDictionary.TryGetValue(nodeId, out LGNode node))
            {
                return node;
            }

            return null;
        }

        public LGNode GetNodeFromOutput(string outputNodeId, int outputPortIndex)
        {
            foreach (LGConnection connection in m_connections)
            {
                if (connection.OutputPort.NodeId == outputNodeId && connection.OutputPort.PortIndex == outputPortIndex)
                {
                    string nodeId = connection.InputPort.NodeId;
                    LGNode inputNode = m_nodeDictionary[nodeId];

                    return inputNode;
                }
            }
            return null;
        }
    }
}
