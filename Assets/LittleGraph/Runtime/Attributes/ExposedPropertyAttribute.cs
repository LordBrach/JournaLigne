using System;

namespace LittleGraph.Runtime.Attributes
{
    public class ExposedPropertyAttribute : Attribute
    {
        private ExposedPropertyType m_exposedPropertyType;
        private bool m_editableInGraph;

        public ExposedPropertyType ExposedPropertyType => m_exposedPropertyType;
        public bool EditableInGraph => m_editableInGraph;

        public ExposedPropertyAttribute(ExposedPropertyType exposedPropertyType = ExposedPropertyType.Simple, bool editableInGraph = true)
        {
            m_exposedPropertyType = exposedPropertyType;
            m_editableInGraph = editableInGraph;
        }
        
        
    }

    public enum ExposedPropertyType
    {
        None,
        Simple,
        List
    }
}
