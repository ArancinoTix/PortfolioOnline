using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace VisualInspector
{
    /// <summary>
    ///     Generic interface for all message box types
    /// </summary>
    public interface IMessageBoxAttribute
    {
        /// <summary>
        ///     Optional title of the message box
        /// </summary>
        [CanBeNull]
        string Title { get; set; }
        
        /// <summary>
        ///     Message to display
        /// </summary>
        string Message { get; set; }

        /// <summary>
        ///     Type of message box
        /// </summary>
        HelpBoxMessageType MessageType { get; }
    }

    /// <summary>
    ///     Generic message box, with a message and a type
    /// </summary>
    public class MessageBoxAttribute : VisualAttribute, IMessageBoxAttribute
    {
        public MessageBoxAttribute(string title, string message,
            HelpBoxMessageType messageType = HelpBoxMessageType.Info)
        {
            Title = title;
            Message = message;
            MessageType = messageType;
        }
        
        public MessageBoxAttribute(string message, HelpBoxMessageType messageType = HelpBoxMessageType.Info)
        {
            Message = message;
            MessageType = messageType;
        }

        /// <inheritdoc />
        public string Title { get; set; }

        /// <inheritdoc />
        public string Message { get; set; }

        /// <inheritdoc />
        public HelpBoxMessageType MessageType { get; set; }
    }

    /// <summary>
    ///     Information box to display in the inspector
    /// </summary>
    public class InfoBoxAttribute : VisualAttribute, IMessageBoxAttribute
    {
        public InfoBoxAttribute(string title, string message)
        {
            Title = title;
            Message = message;
        }
        
        public InfoBoxAttribute(string message)
        {
            Message = message;
        }

        /// <inheritdoc />
        public string Title { get; set; }
        
        /// <inheritdoc />
        public string Message { get; set; }

        /// <inheritdoc />
        HelpBoxMessageType IMessageBoxAttribute.MessageType { get; } = HelpBoxMessageType.Info;
    }

    /// <summary>
    ///     Warning box to display in the inspector
    /// </summary>
    public class WarningBoxAttribute : VisualAttribute, IMessageBoxAttribute
    {
        public WarningBoxAttribute(string title, string message)
        {
            Title = title;
            Message = message;
        }
        
        public WarningBoxAttribute(string message)
        {
            Message = message;
        }

        /// <inheritdoc />
        public string Title { get; set; }
        
        /// <inheritdoc />
        public string Message { get; set; }

        /// <inheritdoc />
        HelpBoxMessageType IMessageBoxAttribute.MessageType { get; } = HelpBoxMessageType.Warning;
    }

    /// <summary>
    ///     Error box to display in the inspector
    /// </summary>
    public class ErrorBoxAttribute : VisualAttribute, IMessageBoxAttribute
    {
        public ErrorBoxAttribute(string title, string message)
        {
            Title = title;
            Message = message;
        }
        
        public ErrorBoxAttribute(string message)
        {
            Message = message;
        }

        /// <inheritdoc />
        public string Title { get; set; }
        
        /// <inheritdoc />
        public string Message { get; set; }

        /// <inheritdoc />
        HelpBoxMessageType IMessageBoxAttribute.MessageType { get; } = HelpBoxMessageType.Error;
    }
}