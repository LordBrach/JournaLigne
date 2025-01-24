using System;
using Codice.CM.SEIDInfo;

namespace LittleGraph.Runtime.Attributes
{
    public class LGNodeInfoAttribute : Attribute
    {
        private string m_nodeTitle;
        private string m_menuItem;
        private bool m_hasFlowInput;
        private bool m_hasFlowOutput;
        private bool m_hasMultipleOutputs;
        private Type m_outputComplementaryDataType;

        public string Title => m_nodeTitle;
        public string MenuItem => m_menuItem;
        public bool HasFlowInput => m_hasFlowInput;
        public bool HasFlowOutput => m_hasFlowOutput;
        public bool HasMultipleOutputs => m_hasMultipleOutputs;
        public Type OutputComplementaryDataType => m_outputComplementaryDataType;

        public LGNodeInfoAttribute(string title, string menuItem = "", bool hasFlowInput = true, bool hasFlowOutput = true, bool hasMultipleOutputs = false, Type outputComplementaryDataType = null)
        {
            m_nodeTitle = title;
            m_menuItem = menuItem;
            m_hasFlowInput = hasFlowInput;
            m_hasFlowOutput = hasFlowOutput;
            m_hasMultipleOutputs = hasMultipleOutputs;
            m_outputComplementaryDataType = outputComplementaryDataType;
        }
    }
}
