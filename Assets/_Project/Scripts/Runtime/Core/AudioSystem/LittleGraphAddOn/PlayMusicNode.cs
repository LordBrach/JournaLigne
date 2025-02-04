using LittleGraph.Runtime;
using LittleGraph.Runtime.Attributes;
using UnityEngine;

namespace _Project.Scripts.Runtime.Core.AudioSystem.LittleGraphAddOn
{
#if LITTLE_GRAPH
    [LGNodeInfo("Play Music", "Audio System/Play Music")]
    public class PlayMusicNode : LGNode
    {
        [ExposedProperty] public AudioClip MusicAudioClip;
        
        protected override void ExecuteNode()
        {
            base.ExecuteNode();

            if (AudioManager.Instance)
            {
                AudioManager.Instance.PlayMusic(MusicAudioClip);
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
