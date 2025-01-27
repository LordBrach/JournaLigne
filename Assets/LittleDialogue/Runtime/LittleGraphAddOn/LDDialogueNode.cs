using System.Collections.Generic;
using LittleDialogue.Runtime.Localization;
using LittleGraph.Runtime;
using LittleGraph.Runtime.Attributes;
using UnityEngine;

namespace LittleDialogue.Runtime.LittleGraphAddOn
{
#if LITTLE_GRAPH
    public abstract class LDDialogueNode : LGNode
    {
        [ExposedProperty]
        public string DialogueText = "Dialogue";
        
        
        // [ExposedProperty()] 
        // public LocalizationDatabase LocalizationDatabase;
        //
        // [ExposedProperty(false)] 
        // public string CurrentLocalizationKey;
    }
#endif
}
