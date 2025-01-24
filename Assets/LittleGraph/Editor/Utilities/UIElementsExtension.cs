using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace LittleGraph.Editor.Utilities
{
    public static class UIElementsExtension
    {
        public static Button CreateButton(this Button button, Texture texture, Action callback = null)
        {
            button.clickable = new(callback);

            button.Add(new Image()
            {
                image = texture
            });

            return button;
        }
        
        public static Button CreateButton(this Button button, string textureName, Action callback = null)
        {
            return CreateButton(button, EditorGUIUtility.IconContent(textureName).image, callback);
        }
    }
}
