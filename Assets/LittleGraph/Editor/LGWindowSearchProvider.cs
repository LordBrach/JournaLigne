using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LittleGraph.Runtime;
using LittleGraph.Runtime.Attributes;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace LittleGraph.Editor
{
    public struct SearchContextElement
    {
        public object Target { get; private set; }
        public string Title { get; private set; }

        public SearchContextElement(object target, string title)
        {
            Target = target;
            Title = title;
        }
    }
    public class LGWindowSearchProvider : ScriptableObject, ISearchWindowProvider
    {
        public LGGraphView GraphView;
        public VisualElement Target;

        public static List<SearchContextElement> elements;
        
        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            //Create tree
            List<SearchTreeEntry> tree = new List<SearchTreeEntry>();
            //Create Category
            tree.Add(new SearchTreeGroupEntry(new GUIContent("Nodes"), 0));

            //Create list elements
            elements = new List<SearchContextElement>();

            //Go through all assemblies, look at each type and check if
            //they have the "LDNodeInfo" custom attribute. If they do
            //and menu item of the attribute is note empty, then add an
            //element to list corresponding to this type
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (Assembly assembly in assemblies)
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (type.CustomAttributes.ToList() != null)
                    {
                        var attribute = type.GetCustomAttribute(typeof(LGNodeInfoAttribute));
                        if (attribute != null)
                        {
                            LGNodeInfoAttribute att = (LGNodeInfoAttribute)attribute;
                            var node = Activator.CreateInstance(type);
                            if(string.IsNullOrEmpty(att.MenuItem)){continue;}
                            elements.Add(new SearchContextElement(node, att.MenuItem));
                        }
                    }
                }
            }
            
            //Sort elements by name
            elements.Sort((entry1, entry2) =>
            {
                string[] splits1 = entry1.Title.Split('/');
                string[] splits2 = entry2.Title.Split('/');
                for (int i = 0; i < splits1.Length; i++)
                {
                    if (i >= splits2.Length) return 1;
                    int value = splits1[i].CompareTo(splits2[i]);
                    if (value != 0)
                    {
                        //Make sure that leaves go before nodes
                        if (splits1.Length != splits2.Length && (i == splits1.Length - 1 || i == splits2.Length - 1))
                            return splits1.Length < splits2.Length ? 1 : -1;
                        return value;
                    }
                }

                return 0;
            });

            //Make SubGroups for each category found in LDNodeInfo attribute's MenuItem property
            List<string> groups = new List<string>();

            foreach (SearchContextElement element in elements)
            {
                string[] entryTitle = element.Title.Split('/');

                string groupName = "";

                for (int i = 0; i < entryTitle.Length - 1; i++)
                {
                    groupName += entryTitle[i];
                    if (!groups.Contains(groupName))
                    {
                        tree.Add(new SearchTreeGroupEntry(new GUIContent(entryTitle[i]), i + 1));
                        groups.Add(groupName);
                    }

                    groupName += "/";
                }

                SearchTreeEntry entry = new SearchTreeEntry(new GUIContent(entryTitle.Last()));
                entry.level = entryTitle.Length;
                entry.userData = new SearchContextElement(element.Target, element.Title);
                tree.Add(entry);
            }

            return tree;
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            var windowMousePosition = GraphView.ChangeCoordinatesTo(GraphView, context.screenMousePosition - GraphView.Window.position.position);
            var graphMousePosition = GraphView.contentViewContainer.WorldToLocal(windowMousePosition);

            SearchContextElement element = (SearchContextElement)SearchTreeEntry.userData;

            LGNode node = (LGNode)element.Target;
            node.SetPosition(new Rect(graphMousePosition, new Vector2()));
            GraphView.Add(node);
            
            return true;
        }
    }
}
