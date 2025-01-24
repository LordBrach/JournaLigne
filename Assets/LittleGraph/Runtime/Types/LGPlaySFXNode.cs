using LittleGraph.Runtime.Attributes;
using UnityEngine;

namespace LittleGraph.Runtime.Types
{
    [LGNodeInfo("Play SFX", "Audio/Play SFX")]
    public class LGPlaySFXNode : LGNode
    {
        [ExposedProperty()] public AudioSource Source;

        protected override void ExecuteNode()
        {
            Source.Play();
            
            base.ExecuteNode();
        }
    }
}
