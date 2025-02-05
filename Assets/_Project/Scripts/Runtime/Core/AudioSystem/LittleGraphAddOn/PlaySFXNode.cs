using LittleGraph.Runtime;
using LittleGraph.Runtime.Attributes;
using UnityEngine;
using UnityEngine.Audio;

namespace _Project.Scripts.Runtime.Core.AudioSystem.LittleGraphAddOn
{
    #if LITTLE_GRAPH
    [LGNodeInfo("Play SFX", "Audio System/Play SFX")]
    public class PlaySFXNode : LGNode
    {
        [ExposedProperty] public AudioClip SfxAudioClip;
        [ExposedProperty] public AudioMixerGroup AudioMixerGroup;
        
        protected override void ExecuteNode()
        {
            base.ExecuteNode();
            
            if (AudioManager.Instance)
            {
                AudioManager.Instance.PlayOneShotSound2D(SfxAudioClip, AudioMixerGroup);
            }
            
            
            if (m_nodeConnections.Exists(connection => connection.OutputPort.NodeId == ID))
            {
                LGConnection connection = m_nodeConnections.Find(connection => connection.OutputPort.NodeId == ID);
                EmitFlow(connection.InputPort.NodeId);
            }
        }
    }
    #endif
}
