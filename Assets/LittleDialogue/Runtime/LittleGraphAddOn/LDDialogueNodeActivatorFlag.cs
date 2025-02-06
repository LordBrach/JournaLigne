using System;
using UnityEngine;

namespace LittleDialogue.Runtime.LittleGraphAddOn
{
#if LITTLE_GRAPH
    [Flags]
    public enum LDDialogueNodeActivatorFlag
    {
        None = 0x00,
        Starter = 0x01,
        Stopper = 0x02
    }

    public enum LDDialogueDisplay
    {
        None,
        Show,
        Hide
    }
#endif
}
