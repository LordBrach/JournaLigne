namespace LittleGraph.Runtime
{
    [System.Serializable]
    public struct LGConnection
    {
        public LGConnectionPort InputPort;
        public LGConnectionPort OutputPort;

        public LGConnection(LGConnectionPort inputPort, LGConnectionPort outputPort)
        {
            InputPort = inputPort;
            OutputPort = outputPort;
        }

        public LGConnection(string inputNodeId, int inputPortIndex, string outputNodeId, int outputPortIndex)
        {
            InputPort = new LGConnectionPort(inputNodeId, inputPortIndex);
            OutputPort = new LGConnectionPort(outputNodeId, outputPortIndex);
        }
    }
    
    [System.Serializable]
    public struct LGConnectionPort
    {
        public string NodeId;
        public int PortIndex;
        
        
        public LGConnectionPort(string nodeId, int portIndex)
        {
            NodeId = nodeId;
            PortIndex = portIndex;
        }
    }
}
