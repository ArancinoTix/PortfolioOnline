using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using VisualInspector;

namespace U9.Utils
{
    [RequireComponent(typeof(TMP_Text))]
    public class LinkOpener : MonoBehaviour, IPointerClickHandler
    {
        [MessageBox("If the TMP file contains a link=\"ID\" tag, it will trigger onLinkClicked when that text is clicked on.")]
        public Action<string> onLinkClicked;
        [SerializeField] private bool _idIsLink = false;

        public void OnPointerClick(PointerEventData eventData)
        {
            TMP_Text pTextMeshPro = GetComponent<TMP_Text>();

            // If you are not in a Canvas using Screen Overlay, put your camera instead of null
            int linkIndex = TMP_TextUtilities.FindIntersectingLink(pTextMeshPro, eventData.position, null);

            if (linkIndex != -1)
            { // was a link clicked?
                TMP_LinkInfo linkInfo = pTextMeshPro.textInfo.linkInfo[linkIndex];

                string text = linkInfo.GetLinkID();

                if(!string.IsNullOrEmpty(text))
                    onLinkClicked?.Invoke(text);

                if (_idIsLink)
                    Application.OpenURL(text);
            }
        }
    }
}