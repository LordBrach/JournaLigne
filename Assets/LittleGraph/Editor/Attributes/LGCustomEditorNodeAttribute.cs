using System;

namespace LittleGraph.Editor.Attributes
{
    public class LGCustomEditorNodeAttribute : Attribute
    {
        private Type m_displayedType;

        public Type DisplayedType => m_displayedType;
        
        public LGCustomEditorNodeAttribute(Type displayedType)
        {
            m_displayedType = displayedType;
        }
    }
}
