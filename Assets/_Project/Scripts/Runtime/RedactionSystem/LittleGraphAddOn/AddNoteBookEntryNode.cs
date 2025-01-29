using LittleGraph.Runtime;
using LittleGraph.Runtime.Attributes;

namespace _Project.Scripts.Runtime.RedactionSystem.LittleGraphAddOn
{
#if LITTLE_GRAPH
    
    [LGNodeInfo("Add Notebook Entry", "Redaction System/Add Notebook Entry")]
    public class AddNoteBookEntryNode : LGNode
    {
        [ExposedProperty] public string NotebookText = "Notebook Text";
        [ExposedProperty] public string ArticleText = "Article Text";
        [ExposedProperty] public float PeopleAppreciation = 1.0f;
        [ExposedProperty] public float RebelsAppreciation = 1.0f;
        [ExposedProperty] public float GovernmentAppreciation = 1.0f;
        
        protected override void ExecuteNode()
        {
            if(NoteBook.instance != null)
            {
                NoteBook.instance.AddEntry(NotebookText, ArticleText, RebelsAppreciation, PeopleAppreciation, GovernmentAppreciation);
            }

            if (m_nodeConnections.Exists(connection => connection.OutputPort.NodeId == ID))
            {
                LGConnection connection = m_nodeConnections.Find(connection => connection.OutputPort.NodeId == ID);
                EmitFlow(connection.InputPort.NodeId);
            }
            
            base.ExecuteNode();
        }
    }
#endif
}
