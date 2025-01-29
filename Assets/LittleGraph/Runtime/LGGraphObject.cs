using UnityEngine;

namespace LittleGraph.Runtime
{
    public class LGGraphObject : MonoBehaviour
    {
        [SerializeField]
        private LGGraph m_graph;

        private LGGraph m_graphInstance;
        public LGGraph GraphInstance => m_graphInstance;
        private void OnEnable()
        {
            // m_graphInstance = Instantiate(m_graph);
            // RegisterEvents();
            //ExecuteAsset();
        }

        public void Init()
        {
            if(m_graph == null) return;
            m_graphInstance = Instantiate(m_graph);
            RegisterEvents();
        }

        private void UnInit()
        {
            UnRegisterEvents();
            if(m_graphInstance != null) Destroy(m_graphInstance);
        }
        
        public void ReplaceGraph(LGGraph newGraph)
        {
            UnInit();
            m_graph = newGraph;
            Init();
        }
        
        private void OnDisable()
        {
            UnRegisterEvents();
        }

        private void RegisterEvents()
        {
            if(m_graphInstance == null) return;
            foreach (LGNode node in m_graphInstance.Nodes)
            {
                node.ReceivedFlow += OnReceivedFlowAction;
                node.Executed += OnExecutedAction;
                node.EmittedFlow += OnEmittedFlowAction;
            }
        }
        
        private void UnRegisterEvents()
        {
            if(m_graphInstance == null) return;
            foreach (LGNode node in m_graphInstance.Nodes)
            {
                node.ReceivedFlow -= OnReceivedFlowAction;
                node.Executed -= OnExecutedAction;
                node.EmittedFlow -= OnEmittedFlowAction;
            }
        }
        
        private void OnReceivedFlowAction()
        {
            //Doing nothing for now
        }

        private void OnExecutedAction(LGNode node)
        {
            //Doing nothing for now
        }

        private void OnEmittedFlowAction(string nextNodeId)
        {
            m_graphInstance.GetNode(nextNodeId).ReceiveFlow();
        }

        
        
        public void ExecuteAsset()
        {
            m_graphInstance.Init();
            
            LGNode startNode = m_graphInstance.GetStartNode();

            startNode.ReceiveFlow();
            
            //ProcessAndMoveToNextNode(startNode);
        }
        
        private void ProcessAndMoveToNextNode(LGNode currentNode)
        {
            string nextNodeId = currentNode.OnProcess(m_graphInstance);

            if (!string.IsNullOrEmpty(nextNodeId))
            {
                LGNode nextNode = m_graphInstance.GetNode(nextNodeId);
                
                ProcessAndMoveToNextNode(nextNode);
            }
        }
    }
}
