using System.Collections.Generic;
using UnityEngine;

namespace LittleDialogue.Runtime.Localization
{
    [CreateAssetMenu(fileName = "LocalizationDatabase", menuName = "Scriptable Objects/Little Dialogue/LocalizationDatabase")]
    public class LocalizationDatabase : ScriptableObject
    {
        public List<LocalizationData> Datas;

    }
}
