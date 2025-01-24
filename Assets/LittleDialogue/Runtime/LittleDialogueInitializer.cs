using System.Collections.Generic;
using System.Linq;
using LittleGraph.Runtime;
using UnityEngine;

namespace LittleDialogue.Runtime
{
#if LITTLE_GRAPH
    public class LittleDialogueInitializer : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            List<LGGraphObject> graphObjects = GameObject.FindObjectsByType<LGGraphObject>((FindObjectsSortMode)FindObjectsInactive.Include).ToList();

            foreach (LGGraphObject graphObject in graphObjects)
            {
                graphObject.Init();
            }

            DialogueController dialogueController = FindFirstObjectByType<DialogueController>();
            if (dialogueController != null)
            {
                dialogueController.Init(graphObjects);
            }
        }
    }
#endif

}
