using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LittleGraph.Editor.Attributes;
using LittleGraph.Runtime;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace LittleGraph.Editor
{
    public class LGGraphView : GraphView
    {
        private LGGraph m_graph;
        private SerializedObject m_serializedObject;
        private LGEditorWindow m_window;
        
        public LGEditorWindow Window => m_window;

        private List<Type> m_customEditorNodeTypes;
        
        private List<LGEditorNode> m_graphNodes;
        private Dictionary<string, LGEditorNode> m_nodeDictionary;
        private Dictionary<Edge, LGConnection> m_connectionDictionary;

        private LGWindowSearchProvider m_searchProvider;
        
        public LGGraphView(SerializedObject serializedObject, LGEditorWindow window)
        {
            m_serializedObject = serializedObject;
            m_graph = (LGGraph)serializedObject.targetObject;
            m_window = window;

            m_customEditorNodeTypes = new List<Type>();
            
            m_graphNodes = new List<LGEditorNode>();
            m_nodeDictionary = new Dictionary<string, LGEditorNode>();
            m_connectionDictionary = new Dictionary<Edge, LGConnection>();

            m_searchProvider = ScriptableObject.CreateInstance<LGWindowSearchProvider>();
            m_searchProvider.GraphView = this;
            this.nodeCreationRequest = ShowSearchWindow;
            
            StyleSheet style =
                AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/LittleGraph/Editor/USS/LGEditor.uss");
            styleSheets.Add(style);
                
            GridBackground background = new GridBackground();
            background.name = "Grid";
            Add(background);
            background.SendToBack();
            
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new ClickSelector());

            GetCustomEditorNodeTypes();
            
            DrawNodes();
            DrawConnections();

            graphViewChanged += OnGraphViewChangedEvent;
        }

        private void GetCustomEditorNodeTypes()
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                foreach (Type type in assembly.GetTypes().Where(wType => wType.GetCustomAttribute<LGCustomEditorNodeAttribute>() != null))
                {
                    m_customEditorNodeTypes.Add(type);
                }
            }
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> allPorts = new List<Port>();
            List<Port> ports = new List<Port>();

            foreach (LGEditorNode editorNode in m_graphNodes)
            {
                allPorts.AddRange(editorNode.Ports);
            }

            foreach (Port port in allPorts)
            {
                if(port == startPort) continue;
                if(port.node == startPort.node) continue;
                if(port.direction == startPort.direction) continue;
                if (port.portType == startPort.portType)
                {
                    ports.Add(port);
                }
            }
            
            return ports;
        }

        private GraphViewChange OnGraphViewChangedEvent(GraphViewChange graphViewChange)
        {
            if (graphViewChange.movedElements != null)
            {
                Undo.RecordObject(m_serializedObject.targetObject, "Moved Elements");

                foreach (LGEditorNode editorNode in graphViewChange.movedElements.OfType<LGEditorNode>())
                {
                    editorNode.SavePosition();
                }
            }
            
            if (graphViewChange.elementsToRemove != null)
            {
                Undo.RecordObject(m_serializedObject.targetObject, "Removed Stuff From Graph");
                
                List<LGEditorNode> editorNodes = graphViewChange.elementsToRemove.OfType<LGEditorNode>().ToList();
                if (editorNodes.Count > 0)
                {
                    for (int i = editorNodes.Count - 1; i >= 0; i--)
                    {
                        RemoveNode(editorNodes[i]);
                    }
                }

                foreach (Edge edge in graphViewChange.elementsToRemove.OfType<Edge>())
                {
                    RemoveConnection(edge);
                }
            }

            if (graphViewChange.edgesToCreate != null)
            {
                Undo.RecordObject(m_serializedObject.targetObject, "Added Connections");
                foreach (Edge edge in graphViewChange.edgesToCreate)
                {
                    CreateEdge(edge);
                }
            }
            
            return graphViewChange;
        }

        private void CreateEdge(Edge edge)
        {
            LGEditorNode inputNode = (LGEditorNode)edge.input.node;
            int inputIndex = inputNode.Ports.IndexOf(edge.input);
            
            LGEditorNode outputNode = (LGEditorNode)edge.output.node;
            int outputIndex = outputNode.Ports.IndexOf(edge.output);

            LGConnection connection = new LGConnection(inputNode.Node.ID, inputIndex, outputNode.Node.ID, outputIndex);
            m_graph.Connections.Add(connection);
            m_connectionDictionary.Add(edge, connection);
            
            inputNode.Node.NodeConnections.Add(connection);
            outputNode.Node.NodeConnections.Add(connection);
        }

        private void RemoveNode(LGEditorNode editorNode)
        {
            m_graph.Nodes.Remove(editorNode.Node);
            m_nodeDictionary.Remove(editorNode.Node.ID);
            editorNode.OutputRemovedAction -= OnRemoveOutput;
            m_graphNodes.Remove(editorNode);
            m_serializedObject.Update();
        }

        private void RemoveConnection(Edge edge)
        {
            if (m_connectionDictionary.TryGetValue(edge, out LGConnection connection))
            {
                m_graph.Connections.Remove(connection);
                m_connectionDictionary.Remove(edge);
                
                LGEditorNode inputNode = (LGEditorNode)edge.input.node;
                LGEditorNode outputNode = (LGEditorNode)edge.output.node;
                
                inputNode.Node.NodeConnections.Remove(connection);
                outputNode.Node.NodeConnections.Remove(connection);
                
                m_serializedObject.Update();
            }
        }
        
        public void OnRemoveOutput(Port outputPort)
        {
            DeleteElements(outputPort.connections);
        }
        
        private void DrawNodes()
        {
            foreach (LGNode node in m_graph.Nodes)
            {
                AddNodeToGraph(node);
            }
            
            BindObject();
        }

        private void DrawConnections()
        {
            if(m_graph.Connections == null) return;

            foreach (LGConnection connection in m_graph.Connections)
            {
                DrawConnection(connection);
            }
        }

        private void DrawConnection(LGConnection connection)
        {
            LGEditorNode inputNode = GetNode(connection.InputPort.NodeId);
            LGEditorNode outputNode = GetNode(connection.OutputPort.NodeId);
            
            if(inputNode == null || outputNode == null) return;

            Port inputPort = inputNode.Ports[connection.InputPort.PortIndex];
            Port outputPort = outputNode.Ports[connection.OutputPort.PortIndex];

            Edge edge = inputPort.ConnectTo(outputPort);
            AddElement(edge);
            m_connectionDictionary.Add(edge, connection);
        }

        private LGEditorNode GetNode(string nodeId)
        {
            LGEditorNode node = null;
            m_nodeDictionary.TryGetValue(nodeId, out node);
            return node;
        }

        private void ShowSearchWindow(NodeCreationContext obj)
        {
            m_searchProvider.Target = (VisualElement)focusController.focusedElement;
            SearchWindow.Open(new SearchWindowContext(obj.screenMousePosition), m_searchProvider);
        }

        public void Add(LGNode node)
        {
            Undo.RecordObject(m_serializedObject.targetObject, "Added Node");
            m_graph.Nodes.Add(node);
            m_serializedObject.Update();

            AddNodeToGraph(node);
            BindObject();
            
            EditorUtility.SetDirty(m_serializedObject.targetObject);
        }

        private void AddNodeToGraph(LGNode node)
        {
            node.TypeName = node.GetType().AssemblyQualifiedName;

            foreach (Type type in m_customEditorNodeTypes)
            {
                LGCustomEditorNodeAttribute attribute = type.GetCustomAttribute<LGCustomEditorNodeAttribute>();
                if (attribute.DisplayedType == node.GetType())
                {
                    Debug.Log("Found Custom Type");
                    LGEditorNode customEditorNode = (LGEditorNode)Activator.CreateInstance(type);
                    customEditorNode.InitEditorNode(node, m_serializedObject);
                    
                    customEditorNode.SetPosition(node.Position);
                    m_graphNodes.Add(customEditorNode);
                    m_nodeDictionary.Add(node.ID, customEditorNode);
                    customEditorNode.OutputRemovedAction += OnRemoveOutput;
            
                    AddElement(customEditorNode);
                    return;
                }
            }
            
            LGEditorNode editorNode = new LGEditorNode(node, m_serializedObject);
            editorNode.SetPosition(node.Position);
            m_graphNodes.Add(editorNode);
            m_nodeDictionary.Add(node.ID, editorNode);
            editorNode.OutputRemovedAction += OnRemoveOutput;
            
            AddElement(editorNode);
        }

        private void BindObject()
        {
            m_serializedObject.Update();
            this.Bind(m_serializedObject);
        }
    }
}
