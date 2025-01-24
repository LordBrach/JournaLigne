Little Graph is a package that you can use to make a graph with an execution flow;
You can create your own nodes when heriting from the LGNode base class
You can also create your custom editor node heriting from LGEditorNode base class and using de [LGCustomEditorNode(typeof)] attribute abive your class.

To execute a graph, reference the SO in a gameobject having the LGGraphObject component and either execute through code or using the "Execute" button on the component at Runtime.