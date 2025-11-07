using System;
using UnityEngine;
using UnityEngine.UIElements;
using VisualInspector.Editor.Core;

namespace VisualInspector.Editor.Drawers
{
    /// <summary>
    ///     Create a generic drawer of message box
    /// </summary>
    [VisualDrawerTarget(typeof(IMessageBoxAttribute))]
    public class MessageBoxDrawer : VisualDrawer
    {
        public override VisualElement CreateInspectorGUI(InspectorData inspectorData)
        {
            var messageBoxAttribute = (IMessageBoxAttribute)Attribute;
            var messageBox = new HelpBox(messageBoxAttribute.Message, messageBoxAttribute.MessageType);
            messageBox.style.marginBottom = 8;

            if (!string.IsNullOrEmpty(messageBoxAttribute.Title))
            {
                var verticalLayout = new VisualElement();
                var header = new Label(messageBoxAttribute.Title);
                header.style.unityFontStyleAndWeight = new StyleEnum<FontStyle>(FontStyle.Bold);
                header.style.fontSize = new StyleLength(16);

                verticalLayout.Add(header);
                verticalLayout.Add(messageBox.Q<Label>());
                messageBox.Add(verticalLayout);
            }
            
            if(TargetVisualElement != null)
            {
                TargetVisualElement.Insert(0, messageBox);
                return null;
            }
            else
            {
                return messageBox;
            }
        }
        
    }
}