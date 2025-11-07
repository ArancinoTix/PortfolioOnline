using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class OpenHyperlinks : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private TextMeshProUGUI m_Text;

    public void OnPointerClick(PointerEventData eventData)
    {
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(m_Text, eventData.position, eventData.pressEventCamera);
        if (linkIndex != -1)
        { // was a link clicked?
            TMP_LinkInfo linkInfo = m_Text.textInfo.linkInfo[linkIndex];

            string selectedLink = linkInfo.GetLinkID();
            if (selectedLink != "")
            {
                //Debug.LogFormat("Open link {0}", selectedLink);
                Application.OpenURL(selectedLink);
            }
        }
    }
}
